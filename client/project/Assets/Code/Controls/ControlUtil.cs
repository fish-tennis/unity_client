using System;
using System.Collections.Generic;
using Code.ViewMgr;
using UnityEngine;

namespace Code.Controls
{
    // 通用的控件接口
    public static class ControlUtil
    {
        // 通用的刷新列表操作(如任务列表)
        public static void UpdateListView<TData,TBindingData>(Transform content, GameObject template, Dictionary<int,TData> dataDict,
            Func<TData,int> cfgIdGetter) where TBindingData:IBindingData<TData>
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
                Debug.Log($"UpdateListView add:{bindingData.GetCfgId()}");
            }
        }
        
        // TODO: 刷新列表里的单个项
    }
}