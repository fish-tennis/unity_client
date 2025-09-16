using Code.cfg;
using Code.game;
using Code.ViewMgr;
using UnityEngine;
using UnityEngine.UI;

namespace Code.Views
{
    // 任务绑定数据
    public class QuestBindingData : MonoBehaviour, IBindingData
    {
        private Gserver.QuestData m_BindingData => BindingData as Gserver.QuestData;
        
        [SerializeField] private Text m_QuestName;
        [SerializeField] private Text m_Progress;
        [SerializeField] private Button m_Finish;

        public void Start()
        {
            m_Finish.onClick.AddListener(OnClickFinish);
            UpdateUI();
        }

        public object BindingData { get; set; }

        public int GetCfgId()
        {
            return m_BindingData?.CfgId ?? 0;
        }
        
        public void UpdateUI()
        {
            if (m_BindingData == null)
            {
                return;
            }
            var questCfg = DataMgr.Quests[m_BindingData.CfgId];
            m_QuestName.text = questCfg.Name;
            var progressStr = "";
            if (questCfg.Progress != null)
            {
                progressStr = $"{m_BindingData.Progress}/{questCfg.Progress.Total}";
            }
            else if(questCfg.Collects.Count > 0)
            {
                // 收集类任务,显示收集的物品数量
                foreach (var item in questCfg.Collects)
                {
                    var itemCfg = DataMgr.ItemCfgs[item.CfgId];
                    if (itemCfg != null)
                    {
                        var num = Client.Instance.Player.GetBags().GetItemCount(item.CfgId); 
                        progressStr += $"{itemCfg.Name}:{num}/{item.Num}";
                    }
                }
            }
            m_Progress.text = progressStr;
            m_Finish.gameObject.SetActive(Client.Instance.Player.GetQuest().CanFinish(m_BindingData, questCfg));
        }

        public void OnClickFinish()
        {
            Client.Send(new Gserver.FinishQuestReq()
            {
                QuestCfgIds = { m_BindingData.CfgId },
            });
        }
    }
}