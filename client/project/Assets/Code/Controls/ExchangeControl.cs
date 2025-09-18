using Code.cfg;
using Code.game;
using Code.util;
using Gserver;
using UnityEngine;
using UnityEngine.UI;

namespace Code.Controls
{
    // 兑换绑定数据
    public class ExchangeControl : MonoBehaviour, IControlScript<Gserver.ExchangeRecord,int>
    {
        [SerializeField] private Text m_ExchangeName;
        [SerializeField] private Text m_ExchangeCount;
        [SerializeField] private Text m_Consumes;
        [SerializeField] private Text m_Rewards;
        [SerializeField] private Button m_Exchange;

        public void Start()
        {
            m_Exchange.onClick.AddListener(OnClickExchange);
            UpdateUI();
        }

        public ExchangeRecord BindingData { get; set; }

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
            var exchangeCfg = DataMgr.ExchangeCfgs[BindingData.CfgId];
            m_ExchangeName.text = exchangeCfg.Detail;
            m_ExchangeCount.text = $"{BindingData.Count}/{exchangeCfg.CountLimit}";
            m_Rewards.text = "礼包内容:" + ItemCfgHelper.GetItemStrings(exchangeCfg.Rewards, " ");
            if(exchangeCfg.Consumes.Count > 0)
            {
                m_Consumes.text = "价格:" + Util.GetRequestItemStrings(exchangeCfg.Consumes, " ");
            }
            else
            {
                m_Consumes.text = "免费";
            }
            m_Exchange.gameObject.SetActive(BindingData.Count < exchangeCfg.CountLimit);
        }

        public void OnClickExchange()
        {
            Client.Send(new Gserver.ExchangeReq()
            {
                IdCounts = { new Gserver.IdCount()
                {
                    Id = BindingData.CfgId,
                    Count = 1,
                } }
            });
        }
        
    }
}