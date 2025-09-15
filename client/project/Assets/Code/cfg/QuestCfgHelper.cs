using System.Collections.Generic;

namespace Code.cfg
{
    using QuestDict = Dictionary<int, Gserver.QuestCfg>;
    
    public static class QuestCfgHelper
    {
        public static Dictionary<int,QuestDict> QuestsByQuestType; // 任务类型索引
        public static Dictionary<int,QuestDict> QuestsByCategory; // 任务分类索引
        public static List<Gserver.QuestCfg> Achievement; // 成就任务
        
        public static void AfterLoad()
        {
            QuestsByQuestType = Helper.CreateDictionaryIndex(DataMgr.Quests, (questCfg) => questCfg.QuestType);
            QuestsByCategory = Helper.CreateDictionaryIndex(DataMgr.Quests, (questCfg) => questCfg.Category);
            Achievement = Helper.CreateList(DataMgr.Quests,  (questCfg) => questCfg.QuestType == (int)Gserver.QuestType.Achievement, null);
            Achievement.Sort((a, b) => a.CfgId.CompareTo(b.CfgId));
        }
    }
}