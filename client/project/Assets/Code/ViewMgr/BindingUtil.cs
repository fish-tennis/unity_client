using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Code.ViewMgr
{
    public static class BindingUtil
    {
        // 通用的刷新列表操作(如任务列表)
        public static void UpdateListView<TData,TBindingData>(Transform content, GameObject template, Dictionary<int,TData> dataDict,
            Func<TData,int> cfgIdGetter) where TBindingData:IBindingData
        {
            Debug.Log($"UpdateListView Count:{dataDict.Count}");
            var existsBindingData = new HashSet<int>();
            foreach (Transform child in content)
            {
                var bindingData = child.GetComponent<TBindingData>();
                if (!dataDict.ContainsKey(bindingData.GetCfgId()))
                {
                    Debug.Log($"UpdateListView remove exists:{bindingData.GetCfgId()}");
                    UnityEngine.Object.Destroy(child.gameObject); // 移除已经不在当前列表的数据
                    continue;
                }
                existsBindingData.Add(bindingData.GetCfgId());
                bindingData.UpdateUI(); // NOTE: 暂时没细化 不管数据是否更新了都刷新一次
            }
            foreach (var data in dataDict.Values)
            {
                if (existsBindingData.Contains(cfgIdGetter(data)))
                {
                    continue;
                }
                // 新增项
                var newNode = UnityEngine.Object.Instantiate(template, content);
                var bindingData = newNode.GetComponent<TBindingData>();
                bindingData.BindingData = data;
                bindingData.UpdateUI();
                Debug.Log($"UpdateListView add:{bindingData.GetCfgId()}");
            }
        }
        
        // TODO: 刷新列表里的单个项
    }
}