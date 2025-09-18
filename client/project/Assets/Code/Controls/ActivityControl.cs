using System;
using Code.cfg;
using Code.game;
using Gserver;
using UnityEngine;
using UnityEngine.UI;

namespace Code.Controls
{
    // 用于活动标签的绑定数据(toggle)
    public class ActivityControl : MonoBehaviour, IControlScript<Activity,int>
    {
        [SerializeField] private Text m_ActivityName;

        public void Start()
        {
            UpdateUI();
        }

        public Activity BindingData { get; set; }

        public int GetKey()
        {
            return BindingData?.Id ?? 0;
        }
        
        public void UpdateUI()
        {
            if (BindingData == null)
            {
                return;
            }
            var activityCfg = DataMgr.ActivityCfgs[BindingData.Id];
            m_ActivityName.text = activityCfg.Name;
        }

    }
}