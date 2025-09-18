using System;
using System.Collections.Generic;
using Code.cfg;
using Code.game;
using Gserver;
using UnityEngine;
using UnityEngine.UI;

namespace Code.Controls
{
    // 物品格子显示所需要的数据,兼容多种物品
    public class ItemShowData
    {
        public Gserver.ItemCfg ItemCfg; // 配置数据
        public int Num; // 数量
        public long UniqueId; // 唯一id,如果是普通道具,UniqueId就是CfgId
        public object Data; // 实际数据,如Gserver.Equip
    }
    
    // 物品绑定数据
    public class ItemControl : MonoBehaviour, IControlScript<ItemShowData,long>
    {
        [SerializeField] private Text m_ItemName;
        [SerializeField] private Text m_Num;

        public void Start()
        {
            UpdateUI();
        }
        
        public ItemShowData BindingData { get; set; }

        public long GetKey()
        {
            return BindingData?.UniqueId ?? 0;
        }
        
        public void UpdateUI()
        {
            if (BindingData == null)
            {
                return;
            }
            m_ItemName.text = BindingData.ItemCfg.Name;
            if (BindingData.Data == null)
            {
                m_Num.text = $"{BindingData.Num}"; // 普通道具才显示数量
            }
            else
            {
                UpdateEndTime();
                // m_Num.text = "";
            }
        }

        // 更新限时物品的倒计时
        public void UpdateEndTime()
        {
            if (BindingData == null || BindingData.Data == null)
            {
                return;
            }
            int endTimestamp = 0;
            switch (BindingData.Data)
            {
                case Gserver.UniqueCountItem uniqueCountItem:
                    endTimestamp = uniqueCountItem.Timeout;
                    break;
                case Gserver.Equip equip:
                    endTimestamp = equip.Timeout;
                    break;
            }
            if (endTimestamp > 0)
            {
                m_Num.text = DateTimeOffset.FromUnixTimeSeconds(endTimestamp).ToLocalTime().ToString("yyyy-MM-dd HH:mm:ss");
            }
            else
            {
                m_Num.text = "";
            }
        }

        // 如果是倒计时显示,就需要在这里更新
        // public void Update()
        // {
        //     UpdateEndTime();
        // }
    }
}