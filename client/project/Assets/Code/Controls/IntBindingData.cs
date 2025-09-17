using Code.cfg;
using Code.game;
using Gserver;
using UnityEngine;
using UnityEngine.UI;

namespace Code.Controls
{
    // 通用的绑定一个int的绑定数据
    public class IntBindingData : MonoBehaviour, IBindingData<int,int>
    {
        public int BindingData { get; set; }

        public int GetCfgId()
        {
            return 0;
        }

        public void UpdateUI()
        {
            
        }
    }
}