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
        [SerializeField] private Transform m_Content;
        [SerializeField]  private GameObject m_Template;
        private GameObject m_TemplateInstance;
        [SerializeField] private Button m_ButtonBack;

        public void Start()
        {
            base.Start();
            m_ButtonBack.onClick.AddListener(OnClickBack);
            m_TemplateInstance = Instantiate(m_Template);
            Destroy(m_Template);
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
                var newNode = Instantiate(m_TemplateInstance, m_Content);
                UpdateQuest(newNode, item);
            }
        }

        public void UpdateQuest(GameObject obj, Gserver.QuestData questData)
        {
            var questCfg = DataMgr.Quests[questData.CfgId];
            obj.transform.Find("Text_Name").GetComponent<Text>().text = questCfg.Name;
        }

        public void OnClickBack()
        {
            ViewMgr.ViewMgr.Instance.GetViewByType<MainView.MainView>()?.ShowView("MainView");
        }
    }
}