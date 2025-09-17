using System;
using System.Collections.Generic;

namespace Code.game
{
    // 活动的集合组件
    public class Activities : BasePlayerComponent
    {
        public const string ComponentName = "Activities";

        public Dictionary<int, Activity> m_Activities; // 所有活动

        public Activities(Player player) : base(ComponentName, player)
        {
            m_Activities = new Dictionary<int, Activity>();
        }
        
        public Activity GetActivity(int activityId)
        {
            return m_Activities.GetValueOrDefault(activityId);
        }

        // 活动数据同步
        private void OnActivitySync(Gserver.ActivitySync res)
        {
            Console.WriteLine("OnActivitySync:" + res);
            if (m_Activities.TryGetValue(res.ActivityId, out var activity))
            {
                activity.m_Data = res.BaseData;
            }
            else
            {
                activity = new Activity(this, res.ActivityId, res.BaseData);
                m_Activities[res.ActivityId] = activity;
            }
        }

        private void OnActivityRemoveRes(Gserver.ActivityRemoveRes res)
        {
            Console.WriteLine("OnActivityRemoveRes:" + res);
            m_Activities.Remove(res.ActivityId);
        }
        
    }
}
