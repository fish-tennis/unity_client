using System;
using System.Collections.Generic;
using Code.cfg;

namespace Code.game
{
    // 单个活动的实现,所有活动使用同一个类来实现
    // 差异化部分通过动态属性和自定义回调来处理
    // 设计思路: 活动本身只是任务和兑换的组合,具体的任务数据和兑换数据都在玩家的Quest组件和Exchange组件里
    public class Activity : IPropertyInt32
    {
        public readonly Activities m_Activities;
        public readonly int Id;
        public readonly Gserver.ActivityCfg m_Cfg; // 配置数据
        public Gserver.ActivityDefaultBaseData m_Data; // 活动数据

        public Activity(Activities activities, int activityId, Gserver.ActivityDefaultBaseData data)
        {
            m_Activities = activities;
            Id = activityId;
            m_Data = data;
            DataMgr.ActivityCfgs.TryGetValue(Id, out var value);
            m_Cfg = value;
        }

        // 获取活动数据上的int32属性值(先查找m_Data,如果没有再查找该活动配置的Properties)
        // IPropertyInt32的实现
        public int GetPropertyInt32(string property)
        {
            // 1.先找注册的属性接口
            if (ActivityProperty.Getters.TryGetValue(property, out var getter))
            {
                return getter(this, property);
            }
            // 2.再找活动数据上的属性值
            // 3.再找活动配置上的属性值
            return m_Data.PropertiesInt32.TryGetValue(property, out var value) ? value : GetCfgPropertyInt32(property);
        }

        // 获取活动配置的int32属性值
        public int GetCfgPropertyInt32(string property)
        {
            if(!m_Cfg.Properties.TryGetValue(property, out var value))
            {
                return 0;
            }
            int.TryParse(value, out var i);
            return i;
        }

        // 获取活动配置的string属性值
        public string GetCfgPropertyString(string property)
        {
            m_Cfg.Properties.TryGetValue(property, out var value);
            return value;
        }

        // 当前是参加这个活动的第几天,从1开始
        public int GetDayCount()
        {
            var now = DateTime.Now.Date;
            var joinDate = DateTimeOffset.FromUnixTimeSeconds(m_Data.JoinTime).Date;
            return (now - joinDate).Days + 1;
        }
        
        // 获取活动的子任务
        // includeFinished: 是否包含已完成的任务
        public Dictionary<int,Gserver.QuestData> GetQuests(bool includeFinished)
        {
            // 活动的子任务放在玩家的任务模块里,从任务模块里筛选出该活动的子任务
            var quest = m_Activities.GetPlayer().GetQuest();
            var allQuests = quest.Filter(q => q.ActivityId == Id);
            if (includeFinished)
            {
                foreach (var questCfgId in m_Cfg.QuestIds)
                {
                    if (!allQuests.ContainsKey(questCfgId))
                    {
                        allQuests[questCfgId] = new Gserver.QuestData
                        {
                            CfgId = questCfgId,
                            ActivityId = Id,
                            Progress = -1, // 特殊值表示该任务是已完成的任务
                        };
                    }
                }
            }
            return allQuests;
        }
        
        // 获取活动的礼包记录
        // includeNotExchanged: 是否包含未兑换的礼包记录
        public Dictionary<int,Gserver.ExchangeRecord> GetExchangeRecords(bool includeNotExchanged)
        {
            // 活动的兑换记录放在玩家的兑换模块里,从兑换模块里筛选出该活动的兑换记录
            var exchange = m_Activities.GetPlayer().GetExchange();
            var allExchanges = exchange.Filter(e => ActivityCfgHelper.GetActivityIdByExchangeId(e.CfgId) == Id);
            // 有兑换记录的礼包才会放在玩家的兑换模块里,所以这里遍历活动配置的所有礼包,把没兑换过的礼包也加进来
            if (includeNotExchanged)
            {
                foreach (var exchangeId in m_Cfg.ExchangeIds)
                {
                    if (!allExchanges.ContainsKey(exchangeId))
                    {
                        allExchanges[exchangeId] = new Gserver.ExchangeRecord
                        {
                            CfgId = exchangeId,
                        };
                    }
                }
            }
            return allExchanges;
        }
    }
}
