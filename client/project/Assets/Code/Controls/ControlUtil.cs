using System;
using System.Collections.Generic;
using System.Linq;
using Code.ViewMgr;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Code.Controls
{
    // 通用的控件接口
    public static class ControlUtil
    {
        // 通用的刷新列表操作(如任务列表)
        public static void UpdateListView<TKey,TData,TBindingData>(Transform content, GameObject template,
            Dictionary<TKey,TData> dataDict, Func<TData,TKey> cfgIdGetter) where TBindingData:IBindingData<TData,TKey>
        {
            Debug.Log($"UpdateListView Count:{dataDict.Count}");
            var existsBindingData = new HashSet<TKey>();
            foreach (Transform child in content)
            {
                var bindingData = child.GetComponent<TBindingData>();
                var cfgId = bindingData.GetCfgId();
                if (!dataDict.ContainsKey(cfgId))
                {
                    Debug.Log($"UpdateListView remove exists:{cfgId}");
                    UnityEngine.Object.Destroy(child.gameObject); // 移除已经不在当前列表的数据
                    continue;
                }
                existsBindingData.Add(cfgId);
                bindingData.BindingData = dataDict[cfgId];
                bindingData.UpdateUI(); // NOTE: 暂时没细化 不管数据是否更新了都刷新一次
            }
            foreach (var data in dataDict.Values)
            {
                var cfgId = cfgIdGetter(data);
                if (existsBindingData.Contains(cfgId))
                {
                    continue;
                }
                // 新增项
                var newNode = UnityEngine.Object.Instantiate(template, content);
                newNode.transform.name = cfgId.ToString();
                var bindingData = newNode.GetComponent<TBindingData>();
                bindingData.BindingData = data;
                bindingData.UpdateUI();
                Debug.Log($"UpdateListView add:{cfgId}");
            }
        }
        
        // TODO: 刷新列表里的单个项
        
        // 更新ToggleGroup
        public static void UpdateToggleGroup<TData,TBindingData>(ToggleGroup toggleGroup, GameObject template,
            Dictionary<int,TData> dataDict, Func<TData,int> cfgIdGetter, Action<Toggle,bool> onToggleValueChanged)
            where TBindingData:IBindingData<TData,int>
        {
            Debug.Log($"UpdateToggleGroup Count:{dataDict.Count}");
            var existsBindingData = new HashSet<int>();
            var setDefaultToggleOn = toggleGroup.transform.childCount == 0;
            foreach (Transform child in toggleGroup.transform)
            {
                var bindingData = child.GetComponent<TBindingData>();
                var cfgId = bindingData?.GetCfgId() ?? 0;
                if (!dataDict.ContainsKey(cfgId))
                {
                    Debug.Log($"UpdateToggleGroup remove exists:{cfgId}");
                    var toggle = child.GetComponent<Toggle>();
                    if (toggle.isOn)
                    {
                        // 如果删除了一个处于选中状态的toggle,则需要重新设置一个默认选中的toggle
                        setDefaultToggleOn = true;
                    }
                    UnityEngine.Object.Destroy(child.gameObject); // 移除已经不在当前列表的数据
                    continue;
                }
                existsBindingData.Add(cfgId);
                bindingData.BindingData = dataDict[cfgId];
                bindingData.UpdateUI(); // NOTE: 暂时没细化 不管数据是否更新了都刷新一次
            }
            foreach (var data in dataDict.Values)
            {
                var cfgId = cfgIdGetter(data);
                if (existsBindingData.Contains(cfgId))
                {
                    continue;
                }
                var toggleObject = UnityEngine.Object.Instantiate(template, toggleGroup.transform);
                toggleObject.transform.name = cfgId.ToString();
                var bindingData = toggleObject.GetComponent<TBindingData>();
                bindingData.BindingData = data;
                bindingData.UpdateUI();
                Debug.Log($"UpdateToggleGroup add:{cfgId}");
                var toggle = toggleObject.GetComponent<Toggle>();
                toggle.onValueChanged.AddListener((isOn) => onToggleValueChanged(toggle, isOn));
            }
            if (setDefaultToggleOn)
            {
                foreach (Transform child in toggleGroup.transform)
                {
                    var toggle = child.GetComponent<Toggle>();
                    toggleGroup.NotifyToggleOn(toggle, true);
                    break;
                }
            }
        }
    }
}