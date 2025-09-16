using System;
using System.Collections.Generic;
using System.Linq;
using Code.game;
using UnityEngine;
using UnityEngine.UI;

namespace Code.ViewMgr
{
    public class FormatInfo
    {
        public string Format; // Lv.{0}
        public List<FormatKey> Properties = new List<FormatKey>();
    }

    public class FormatKey
    {
        public string Property; // 如Player.Level
        public string Arg; // 可选参数
    }
    
    // 挂在控件上,在unity中编辑控件的自定义属性
    public class ElementProperties : MonoBehaviour
    {
        // 格式化 如Lv.{如Player.Level} 或 Lv.{Player.Level:Arg}
        public string Format;
        // 预处理后的格式化信息
        private FormatInfo m_FormatInfo;

        public void Awake()
        {
            if (m_FormatInfo == null)
            {
                m_FormatInfo = new FormatInfo();
                ParseFormat();
            }
            ViewMgr.Instance.AddElement(this);
        }

        public void Start()
        {
            UpdateUI();
        }
        
        public void OnDestroy()
        {
            ViewMgr.Instance.RemoveElement(this);
        }

        // 预处理Format,如Lv.{Player.Level} 预处理后,Format为Lv.{0}
        private void ParseFormat()
        {
            int i = 0;
            int n = this.Format.Length;
            var charFormat = new List<char>();
            int formatIndex = 0;

            while (i < n)
            {
                // 查找左大括号
                if (this.Format[i] == '{')
                {
                    int start = i + 1; // 跳过'{'
                    int end = -1;
                    int colonIndex = -1;

                    // 继续查找右大括号和冒号
                    for (int j = start; j < n; j++)
                    {
                        if (this.Format[j] == '}')
                        {
                            end = j; // 记录右大括号位置
                            if (j - start > 0)
                            {
                                // 替换{Player.Level}为{0}
                                charFormat.AddRange("{"+formatIndex+"}");
                                formatIndex++;
                            }
                            else
                            {
                                charFormat.AddRange("{}"); // 无效的{}
                                Debug.LogError("ParseFormat:" + gameObject.name + this.Format);
                            }
                            break;
                        }
                        if (this.Format[j] == ':' && colonIndex == -1)
                        {
                            colonIndex = j; // 记录第一个冒号位置
                        }
                    }
                    // 没找到匹配的}
                    if (end == -1)
                    {
                        charFormat.AddRange(this.Format.Substring(i));
                        break; // break while
                    }

                    string key = "";
                    string arg = "";
                    // Lv.{Player.Level}
                    if (colonIndex == -1)
                    {
                        key =  this.Format.Substring(start,end-start);
                    }
                    // 如果找到冒号和右大括号，且冒号在右大括号之前
                    else if (colonIndex > start && colonIndex < end)
                    {
                        key = this.Format.Substring(start, colonIndex - start);
                        arg = this.Format.Substring(colonIndex + 1, end - colonIndex - 1);
                    }
                    m_FormatInfo.Properties.Add(new FormatKey()
                    {
                        Property = key,
                        Arg = arg,
                    });
                    i = end + 1; // 移动到右大括号之后
                    continue;
                }
                else
                {
                    charFormat.Add(this.Format[i]);
                }
                i++;
            }
            m_FormatInfo.Format = new string(charFormat.ToArray());
            Debug.LogFormat("ParseFormat:{0} {1} {2}", gameObject.name, m_FormatInfo.Format, m_FormatInfo.Properties.Count);
        }
        
        public FormatInfo GetFormatInfo()
        {
            return m_FormatInfo;
        }

        // 更新显示
        public void UpdateUI()
        {
            Debug.Log("UpdateShow:" + gameObject.name + " " + this.Format);
            if (!gameObject.activeSelf)
            {
                Debug.LogWarning("UpdateShow !gameObject.activeSelf :" + gameObject.name);
                return;
            }
            // 临时代码,实际是注册不同的控件不同的显示逻辑
            // Text控件的显示逻辑
            var text = gameObject.GetComponent<Text>();
            if (text != null)
            {
                Debug.LogFormat("UpdateShow:{0} {1}", gameObject.name, m_FormatInfo.Format);
                if (m_FormatInfo.Properties.Count == 0)
                {
                    var propertyValue = GetPropertyValue(m_FormatInfo.Format);
                    if (propertyValue != null)
                    {
                        text.text = propertyValue.ToString();
                    }
                }
                else
                {
                    var propertyValues = new List<object>();
                    foreach (var key in m_FormatInfo.Properties)
                    {
                        var propertyValue = GetPropertyValue(key.Property);
                        if (propertyValue == null)
                        {
                            return;
                        }
                        propertyValues.Add(propertyValue);
                    }
                    text.text = string.Format(m_FormatInfo.Format, propertyValues.ToArray());
                }
            }
        }

        // 获取属性值
        public static object GetPropertyValue(string fullPropertyName)
        {
            // 临时代码,后续会改成注册查询
            const string playerProperty = "Player.";
            if (fullPropertyName.StartsWith(playerProperty))
            {
                if (Client.Instance.Player == null)
                {
                    return null;
                }
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