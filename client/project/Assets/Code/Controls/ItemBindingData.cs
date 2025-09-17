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
    public class ItemBindingData : MonoBehaviour, IBindingData<ItemShowData,long>
    {
        [SerializeField] private Text m_ItemName;
        [SerializeField] private Text m_Num;

        public void Start()
        {
            UpdateUI();
        }
        
        public ItemShowData BindingData { get; set; }

        public long GetCfgId()
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
                m_Num.text = "";
            }
        }
        
    }
}