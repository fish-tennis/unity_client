using System.Linq;
using Code.cfg;
using Code.Controls;
using Code.game;
using Code.ViewMgr;
using UnityEngine;
using UnityEngine.Events;
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
        [SerializeField] private Text m_Detail;

        public void Awake()
        {
            base.Awake();
            m_ButtonBack.onClick.AddListener(OnClickBack);
            m_QuestTemplateInstance = Instantiate(m_Template_Quest);
            Destroy(m_Template_Quest);
            m_ExchangeTemplateInstance = Instantiate(m_Template_Exchange);
            Destroy(m_Template_Exchange);
            m_ToggleTemplateInstance = Instantiate(m_Template_Toggle);
            Destroy(m_Template_Toggle);
        }
        
        public void Start()
        {
            UpdateActivityTabs();
            UpdateSelectedActivity();
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
            ControlUtil.UpdateToggleGroup<Activity,ActivityControl>(m_ToggleGroup_Names, m_ToggleTemplateInstance,
                activities.m_Activities,x=>x.Id, OnToggleValueChanged);
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

            m_Detail.text = "活动详情:" + activity.m_Cfg.Detail;
            // 该活动的任务
            var filteredQuests = activity.GetQuests(true);
            ControlUtil.UpdateContainer<int,Gserver.QuestData,QuestControl>(m_Content_Quest, m_QuestTemplateInstance,
                filteredQuests,x=>x.CfgId);
            
            // 该活动的兑换礼包
            var filteredExchanges = activity.GetExchangeRecords(true);
            ControlUtil.UpdateContainer<int,Gserver.ExchangeRecord,ExchangeControl>(m_Content_Exchange, m_ExchangeTemplateInstance,
                filteredExchanges,x=>x.CfgId);
        }

        // 当前选择的活动id
        public int GetSelectedActivityId()
        {
            foreach (var toggle in m_ToggleGroup_Names.GetComponentsInChildren<Toggle>())
            {
                if(toggle.isOn)
                {
                    var activityId = int.Parse(toggle.transform.name);
                    return activityId;
                }
            }
            return 0;
        }
        
        // 刷新当前选择的活动
        public void UpdateSelectedActivity()
        {
            var activityId = GetSelectedActivityId();
            if (activityId > 0)
            {
                UpdateActivity(activityId);
            }
        }
        
        // 监听活动更新
        private void OnActivitySync(Gserver.ActivitySync res)
        {
            Debug.Log($"OnActivitySync {res}");
            UpdateActivityTabs();
            if (res.ActivityId == GetSelectedActivityId())
            {
                UpdateActivity(res.ActivityId);
            }
        }

        // 监听活动删除
        private void OnActivityRemoveRes(Gserver.ActivityRemoveRes res)
        {
            Debug.Log($"OnActivityRemoveRes {res}");
            UpdateActivityTabs();
        }
        
        public void OnFinishQuestRes(Gserver.FinishQuestRes res, int error)
        {
            Debug.Log($"OnFinishQuestRes {res} {error}");
            UpdateSelectedActivity(); // 暂时不细化,整体刷新
        }

        public void OnQuestRemoveRes(Gserver.QuestRemoveRes res, int error)
        {
            Debug.Log($"OnQuestRemoveRes {res} {error}");
            UpdateSelectedActivity(); // 暂时不细化,整体刷新
        }
        
        private void OnExchangeUpdate(Gserver.ExchangeUpdate res)
        {
            Debug.Log($"OnExchangeUpdate {res}");
            UpdateSelectedActivity(); // 暂时不细化,整体刷新
        }

        private void OnExchangeRemove(Gserver.ExchangeRemove res)
        {
            Debug.Log($"OnExchangeRemove {res}");
            UpdateSelectedActivity(); // 暂时不细化,整体刷新
        }

        private void OnExchangeRes(Gserver.ExchangeRes res, int error)
        {
            Debug.Log($"OnExchangeRes {res} {error}");
            UpdateSelectedActivity(); // 暂时不细化,整体刷新
        }
        
    }
}