using System;
using cshap_client.game;
using UnityEngine;
using UnityEngine.UI;

namespace Code.ViewMgr
{
    // 挂在控件上,在unity中编辑控件的自定义属性
    public class ElementProperties : MonoBehaviour
    {
        // 属性名 如Player.Level
        public string PropertyName; // 先实现单个属性的
        // 格式化 如Lv.{0}
        public string Format;

        public void Awake()
        {
            ViewMgr.Instance.AddElement(this);
        }
        
        public void OnDestroy()
        {
            ViewMgr.Instance.RemoveElement(this);
        }

        // 更新显示
        public void UpdateShow()
        {
            Debug.Log("UpdateShow:" + gameObject.name);
            // 先写死代码,实际是注册不同的控件不同的显示逻辑
            if (!gameObject.activeSelf)
            {
                Debug.LogWarning("UpdateShow !gameObject.activeSelf PropertyName:" + PropertyName);
                return;
            }
            // Text控件的显示逻辑
            var text = gameObject.GetComponent<Text>();
            if (text != null)
            {
                if (string.IsNullOrEmpty(this.Format))
                {
                    text.text = GetPropertyValue(PropertyName).ToString();
                }
                else
                {
                    text.text = string.Format(this.Format, GetPropertyValue(PropertyName));
                }
            }
        }

        // 获取属性值
        public static object GetPropertyValue(string fullPropertyName)
        {
            // 暂时写死代码,后续会改成注册查询
            const string playerProperty = "Player.";
            if (fullPropertyName.StartsWith(playerProperty))
            {
                var propertyName = fullPropertyName.Substring(playerProperty.Length);
                return Client.Instance.Player.GetProperty(propertyName);
            }
            else
            {
                // TODO: 其他模块的属性
                Debug.LogError($"ElementProperties GetPropertyValue {fullPropertyName} not found");
                return 0;
            }
        }
    }
}