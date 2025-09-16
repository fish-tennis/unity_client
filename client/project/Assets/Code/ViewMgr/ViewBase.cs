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

        public void OnDestroy()
        {
            ViewMgr.Instance.RemoveView(this);
        }
        
    }
}