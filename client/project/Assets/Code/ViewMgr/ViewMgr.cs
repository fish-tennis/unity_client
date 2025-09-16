using System;
using System.Collections.Generic;
using Code.network;
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
        public Dictionary<string,ViewBase> m_Views = new();
        // 所有需要监听数据更新的控件
        public Dictionary<string,List<ElementProperties>> m_Elements = new();
        // 已经注册过消息回调的view name
        public HashSet<string> m_ViewNamesHasRegistered = new();
        
        public void AddView(ViewBase view)
        {
            if (m_Views.ContainsKey(view.name))
            {
                m_Views.Remove(view.name);
                Debug.Log("Remove exists view:" + view.name);
            }
            m_Views.Add(view.name, view);
            if (!m_ViewNamesHasRegistered.Contains(view.name))
            {
                // 注册消息回调
                HandlerRegister.RegisterMethodsForClass(view.GetType(), view.name);
                m_ViewNamesHasRegistered.Add(view.name);
            }
            Debug.Log("AddView:" + view.name);
        }

        public void RemoveView(ViewBase view)
        {
            m_Views.Remove(view.name);
            Debug.Log("RemoveView:" + view.name);
        }
        
        public void AddElement(ElementProperties element)
        {
            foreach (var property in element.GetFormatInfo().Properties)
            {
                if (m_Elements.TryGetValue(property.Property, out var list))
                {
                    list.Add(element);
                }
                else
                {
                    m_Elements.Add(property.Property, new List<ElementProperties>() { element });
                    Debug.Log("AddElement:" + element.name + " "  + property.Property);
                }
            }
        }

        public void RemoveElement(ElementProperties element)
        {
            foreach (var property in element.GetFormatInfo().Properties)
            {
                if (m_Elements.TryGetValue(property.Property, out var list))
                {
                    list.Remove(element);
                    if(list.Count == 0)
                    {
                        m_Elements.Remove(property.Property);
                    }
                    Debug.Log("RemoveElement:" + element.name + " "  + property.Property);
                } 
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
                    element.UpdateUI();
                }
            }
        }

        // 根据name查找view
        public ViewBase GetViewByName(string name)
        {
            return m_Views.GetValueOrDefault(name, null);
        }
        
        public T GetViewByName<T>(string name) where T : ViewBase
        {
            if (m_Views.TryGetValue(name, out var view))
            {
                return view as T;
            }

            return null;
        }
        
        // 根据类型查找第一个view(NOTE:有可能有多个类型相同但是name不同的view)
        public T GetViewByType<T>() where T : ViewBase
        {
            foreach (var view in m_Views.Values)
            {
                if (view is T)
                {
                    return view as T;
                }
            }
            return null;
        }
        
    }
}