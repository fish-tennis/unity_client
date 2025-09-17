using System.Collections.Generic;
using System.Linq;
using Code.cfg;
using Code.Controls;
using Code.game;
using Code.ViewMgr;
using Gserver;
using UnityEngine;
using UnityEngine.UI;

namespace Code.Views
{
    // 背包界面
    public class BagView : ViewBase
    {
        [SerializeField] private Button m_ButtonBack;
        [SerializeField] private Transform m_Content;
        [SerializeField]  private GameObject m_Template;
        private GameObject m_TemplateInstance;
        
        [SerializeField] private ToggleGroup m_ToggleGroup_BagTypes;
        [SerializeField] private GameObject m_Template_Toggle;
        private GameObject m_ToggleTemplateInstance;
        
        public void Start()
        {
            m_ButtonBack.onClick.AddListener(OnClickBack);
            m_TemplateInstance = Instantiate(m_Template);
            Destroy(m_Template);
            m_ToggleTemplateInstance = Instantiate(m_Template_Toggle);
            Destroy(m_Template_Toggle);

            UpdateBagTabs();
            UpdateBag(ContainerType.CountItem);
        }
        
        public void OnClickBack()
        {
            Debug.Log("OnClickBack");
            ViewMgr.ViewMgr.Instance.GetViewByName<MainView>("MainView")?.ShowView("MainView");
        }
        
        // 更新左侧背包类型标签
        public void UpdateBagTabs()
        {
            Debug.Log("UpdateBagTabs");
            var bagTypes = new List<ContainerType>() {ContainerType.CountItem, ContainerType.UniqueItem, ContainerType.Equip};
            var bagNames = new List<string>() {"普通物品", "限时道具", "装备"};
            for (int i = 0; i < bagTypes.Count; i++)
            {
                var toggleObject = Object.Instantiate(m_ToggleTemplateInstance, m_ToggleGroup_BagTypes.transform);
                toggleObject.transform.name = ((int)bagTypes[i]).ToString();
                var toggle = toggleObject.GetComponent<Toggle>();
                toggle.GetComponentInChildren<Text>().text = bagNames[i];
                toggle.onValueChanged.AddListener((isOn) => OnBagTypeChanged(toggle, isOn));
            }
        }
        
        void OnBagTypeChanged(Toggle changedToggle, bool isOn)
        {
            Debug.Log($"OnBagTypeChanged {changedToggle.name} {changedToggle.transform.name} {isOn}");
            if (isOn)
            {
                var bagType = int.Parse(changedToggle.transform.name);
                UpdateBag((ContainerType)bagType);
            }
        }

        public void UpdateBag(Gserver.ContainerType containerType)
        {
            var bag = Client.Instance.Player.GetBags().GetBagByType(containerType);
            if(bag == null)
            {
                return;
            }
            Debug.Log($"UpdateBag {containerType}");
            // 组装显示数据
            Dictionary<long,ItemShowData> itemDatas = new Dictionary<long, ItemShowData>();
            switch (bag)
            {
                case CountContainer countContainer:
                    foreach (var item in countContainer.m_Elems)
                    {
                        itemDatas[item.Key] = new ItemShowData()
                        {
                            ItemCfg = DataMgr.ItemCfgs[item.Key],
                            Num = item.Value,
                            UniqueId = item.Value
                        };
                    }
                    break;
                case UniqueItemBag uniqueItemBag:
                    foreach (var item in uniqueItemBag.m_Elems.Values)
                    {
                        itemDatas[item.UniqueId] = new ItemShowData()
                        {
                            ItemCfg = DataMgr.ItemCfgs[item.CfgId],
                            Num = 1,
                            UniqueId = item.UniqueId,
                            Data = item
                        };
                    }
                    break;
                case EquipBag equipBag:
                    foreach (var item in equipBag.m_Elems.Values)
                    {
                        itemDatas[item.UniqueId] = new ItemShowData()
                        {
                            ItemCfg = DataMgr.ItemCfgs[item.CfgId],
                            Num = 1,
                            UniqueId = item.UniqueId,
                            Data = item
                        };
                    }
                    break;
            }
            ControlUtil.UpdateListView<long,ItemShowData,ItemBindingData>(m_Content, m_TemplateInstance,
                itemDatas,x=>x.UniqueId);
        }
        
        // 当前选择的背包类型
        public ContainerType GetSelectedBagType()
        {
            foreach (var toggle in m_ToggleGroup_BagTypes.GetComponentsInChildren<Toggle>())
            {
                if(toggle.isOn)
                {
                    Debug.Log($"GetSelectedBagType {toggle.name} {toggle.transform.name} {toggle.isOn}");
                    return (ContainerType)int.Parse(toggle.transform.name);
                }
            }
            return 0;
        }

        private void OnElemContainerUpdate(Gserver.ElemContainerUpdate res)
        {
            Debug.Log($"OnElemContainerUpdate:{res.ElemOps.Count} {GetSelectedBagType()}");
            UpdateBag(GetSelectedBagType());
        }
    }
}