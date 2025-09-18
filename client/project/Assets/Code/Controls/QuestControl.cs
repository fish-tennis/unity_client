using Code.cfg;
using Code.game;
using Gserver;
using UnityEngine;
using UnityEngine.UI;

namespace Code.Controls
{
    // 任务绑定数据
    public class QuestControl : MonoBehaviour, IControlScript<Gserver.QuestData,int>
    {
        [SerializeField] private Text m_QuestName;
        [SerializeField] private Text m_Detail;
        [SerializeField] private Text m_Rewards;
        [SerializeField] private Text m_Progress;
        [SerializeField] private Button m_Finish;

        public void Start()
        {
            m_Finish.onClick.AddListener(OnClickFinish);
            UpdateUI();
        }
        
        public QuestData BindingData { get; set; }

        public int GetKey()
        {
            return BindingData?.CfgId ?? 0;
        }
        
        public void UpdateUI()
        {
            if (BindingData == null)
            {
                return;
            }
            var questCfg = DataMgr.Quests[BindingData.CfgId];
            m_QuestName.text = questCfg.Name;
            m_Detail.text = questCfg.Detail;
            var progressStr = "";
            if(BindingData.Progress == -1)
            {
                progressStr = "已完成";
            }
            else if (questCfg.Progress != null)
            {
                progressStr = $"{BindingData.Progress}/{questCfg.Progress.Total}";
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
            if (questCfg.Rewards.Count > 0)
            {
                m_Rewards.text = "奖励:" + ItemCfgHelper.GetItemStrings(questCfg.Rewards, " ");
            }
            m_Finish.gameObject.SetActive(Client.Instance.Player.GetQuest().CanFinish(BindingData, questCfg));
        }

        public void OnClickFinish()
        {
            Client.Send(new Gserver.FinishQuestReq()
            {
                QuestCfgIds = { BindingData.CfgId },
            });
        }
    }
}