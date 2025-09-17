using Code.cfg;
using Code.Controls;
using Code.game;
using Code.ViewMgr;
using UnityEngine;
using UnityEngine.UI;

namespace Code.Views
{
    // 活动界面
    public class ActivityView : ViewBase
    {
        [SerializeField] private Button m_ButtonBack;
        [SerializeField] private Transform m_Content_Quest;
        [SerializeField]  private GameObject m_Template_Quest;
        private GameObject m_QuestTemplateInstance;
        [SerializeField] private Transform m_Content_Exchange;
        [SerializeField]  private GameObject m_Template_Exchange;
        private GameObject m_ExchangeTemplateInstance;
        [SerializeField] private ToggleGroup m_ToggleGroup_Names;
        [SerializeField] private GameObject m_Template_Toggle;
        private GameObject m_ToggleTemplateInstance;
        
        public void Start()
        {
            m_ButtonBack.onClick.AddListener(OnClickBack);
            m_QuestTemplateInstance = Instantiate(m_Template_Quest);
            Destroy(m_Template_Quest);
            m_ExchangeTemplateInstance = Instantiate(m_Template_Exchange);
            Destroy(m_Template_Exchange);
            m_ToggleTemplateInstance = Instantiate(m_Template_Toggle);
            Destroy(m_Template_Toggle);

            UpdateActivityTabs();
        }
        
        public void OnClickBack()
        {
            Debug.Log("OnClickBack");
            ViewMgr.ViewMgr.Instance.GetViewByName<MainView>("MainView")?.ShowView("MainView");
        }

        // 更新左侧活动标签
        public void UpdateActivityTabs()
        {
            Debug.Log("UpdateActivityTabs");
            var activities = Client.Instance.Player.GetActivities();
            Toggle firstToggle = null;
            foreach (var activity in activities.m_Activities)
            {
                var toggleObject = Instantiate(m_ToggleTemplateInstance, m_ToggleGroup_Names.transform);
                toggleObject.transform.name = activity.Key.ToString();
                // toggleObject.GetComponent<ActivityBindingData>().BindingData = activity.Key;
                var toggle = toggleObject.GetComponent<Toggle>();
                toggle.GetComponentInChildren<Text>().text = activity.Value.m_Cfg.Name;
                toggle.onValueChanged.AddListener((isOn) => OnToggleValueChanged(toggle, isOn));
                if (firstToggle == null)
                {
                    firstToggle =  toggle;
                }
            }

            if (firstToggle != null)
            {
                OnToggleValueChanged(firstToggle, true);
            }
        }

        // 左侧标签页切换
        void OnToggleValueChanged(Toggle changedToggle, bool isOn)
        {
            Debug.Log($"OnToggleValueChanged {changedToggle.name} {changedToggle.transform.name} {isOn}");
            if (isOn)
            {
                var activityId = int.Parse(changedToggle.transform.name);
                UpdateActivity(activityId);
            }
        }

        // 更新一个活动的内容
        public void UpdateActivity(int activityId)
        {
            var activity = Client.Instance.Player.GetActivities().GetActivity(activityId);
            if (activity == null)
            {
                return;
            }
            // 该活动的任务
            var filteredQuests = activity.GetQuests();
            ControlUtil.UpdateListView<Gserver.QuestData,QuestBindingData>(m_Content_Quest, m_QuestTemplateInstance,
                filteredQuests,x=>x.CfgId);
            
            // 该活动的兑换礼包
            var filteredExchanges = activity.GetExchangeRecords(true);
            ControlUtil.UpdateListView<Gserver.ExchangeRecord,ExchangeBindingData>(m_Content_Exchange, m_ExchangeTemplateInstance,
                filteredExchanges,x=>x.CfgId);
        }
        
    }
}