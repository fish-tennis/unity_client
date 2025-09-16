using System;
using System.Collections.Generic;
using UnityEngine;

namespace Code.ViewMgr
{
    // View基类
    public class ViewBase : MonoBehaviour
    {
        public void Awake()
        {
            ViewMgr.Instance.AddView(this);
        }

        public void Start()
        {
            // View上所有的绑定了ElementProperties的控件初始化显示
            var scripts = this.gameObject.GetComponentsInChildren<ElementProperties>(true);
            foreach (var script in scripts)
            {
                script.UpdateUI();
            }
        }

        public void OnDestroy()
        {
            ViewMgr.Instance.RemoveView(this);
        }
        
    }
}