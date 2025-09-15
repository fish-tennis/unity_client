using Code.cfg;
using Code.game;
using Code.ViewMgr;
using UnityEngine;
using UnityEngine.UI;

namespace Code.views.QuestView
{
    // 任务界面
    public class QuestView : ViewBase
    {
        private Transform m_Content;
        private GameObject m_Template;
        public void Start()
        {
            base.Start();
            m_Content = transform.Find("ScrollView_Quest").Find("Content");
            m_Template = Instantiate(transform.Find("Template").gameObject);
            UpdateQuests();
        }
        
        public void UpdateQuests()
        {
            foreach (Transform child in m_Content)
            {
                Destroy(child.gameObject);
            }
            var quest = Client.Instance.Player.GetQuest();
            foreach (var item in quest.Quests.Values)
            {
                var newNode = Instantiate(m_Template, m_Content);
                UpdateQuest(newNode, item);
            }
        }

        public void UpdateQuest(GameObject obj, Gserver.QuestData questData)
        {
            var questCfg = DataMgr.Quests[questData.CfgId];
            obj.transform.Find("Text_Name").GetComponent<Text>().text = questCfg.Name;
        }
    }
}