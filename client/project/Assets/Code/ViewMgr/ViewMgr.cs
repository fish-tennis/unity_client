using System.Collections.Generic;
using Code.ViewMgr;
using UnityEngine;

namespace Code.ViewMgr
{
    // view管理器
    public class ViewMgr
    {
        private static ViewMgr _ins;
        public static ViewMgr Instance => _ins ??= new ViewMgr();

        // 所有view
        public List<ViewBase> views = new();
        // 所有需要监听数据更新的控件
        public Dictionary<string,List<ElementProperties>> m_Elements = new();
        
        public void AddView(ViewBase view)
        {
            views.Add(view);
            Debug.Log("AddView:" + view.name);
        }

        public void RemoveView(ViewBase view)
        {
            views.Remove(view);
            Debug.Log("RemoveView:" + view.name);
        }
        
        public void AddElement(ElementProperties element)
        {
            if (m_Elements.TryGetValue(element.PropertyName, out var list))
            {
                list.Add(element);
            }
            else
            {
                m_Elements.Add(element.PropertyName, new List<ElementProperties>() { element });
            }
            Debug.Log("AddElement:" + element.name);
        }

        public void RemoveElement(ElementProperties element)
        {
            if (m_Elements.TryGetValue(element.PropertyName, out var list))
            {
                list.Remove(element);
                if(list.Count == 0)
                {
                    m_Elements.Remove(element.PropertyName);
                }
                Debug.Log("RemoveElement:" + element.name);
            }
        }

        // 响应数据更新
        public void OnDataUpdate(string propertyName)
        {
            Debug.Log("OnDataUpdate:" + propertyName);
            if (m_Elements.TryGetValue(propertyName, out var list))
            {
                foreach (var element in list)
                {
                    element.UpdateShow();
                }
            }
        }
        
    }
}