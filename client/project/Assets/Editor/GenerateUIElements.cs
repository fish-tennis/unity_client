using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

public class GenerateUIElements
{
    private const string rootPath = "/MVVMFrame/";

    private static Dictionary<string, string> dict = new Dictionary<string, string>()
    {
        //组件类型,定义名
        {"Text", "Text_"},
        {"Button", "Btn_"},
        {"Image", "Img_"},
    };

    private static bool starCreate;

    [MenuItem("GameObject/GenerateUIElements/Create", priority = 0)]
    static void CreateUIElements()
    {
        // List<Transform> allSelect = new List<Transform>();
        foreach (var trans in Selection.transforms)
        {
            var dirPath = Application.dataPath + rootPath + trans.name;
            if (!Directory.Exists(dirPath))
            {
                Debug.LogError("未找到文件目录:" + dirPath);
                continue;
            }

            // allSelect.Add(trans);
            CreateClass(dirPath, trans);
            Debug.Log("Create success: " + dirPath);
        }

        Debug.Log("Create Finish.");
        //重新编译代码
        UnityEditor.Compilation.CompilationPipeline.RequestScriptCompilation();
        AssetDatabase.Refresh();
        BindUIElements();
    }

    [MenuItem("GameObject/GenerateUIElements/RefreshBind")]
    static void BindUIElements()
    {
        foreach (var trans in Selection.transforms)
        {
            var className = trans.name + "UIElements";
            var type = Type.GetType(className + "," + typeof(UIElements).Assembly.FullName);
            if (type == null)
            {
                Debug.LogError($"未找到{className},请先执行Create");
                continue;
            }

            var uiElements = trans.GetComponent(className);
            if (uiElements == null)
            {
                uiElements = trans.gameObject.AddComponent(type);
            }

            foreach (var filed in type.GetFields())
            {
                var objName = GetObjNameByField(filed);
                var element = FindByName(trans, objName)?.GetComponent(filed.FieldType);
                if (element)
                    filed.SetValue(uiElements, element);
                else
                    filed.SetValue(uiElements, null);
            }
        }

        AssetDatabase.Refresh();
    }

    //创建脚本文件
    static void CreateClass(string dirPath, Transform trans)
    {
        var filePath = dirPath + "/" + trans.name + "UIElements.cs";
        if (File.Exists(filePath))
        {
            var sw = new StreamWriter(filePath);
            WriteElements(sw, trans);
            sw.Flush();
            sw.Close();
        }
        else
        {
            FileStream fs = new FileStream(filePath, FileMode.CreateNew);
            var sw = new StreamWriter(fs);
            WriteElements(sw, trans);
            sw.Flush();
            sw.Close();
        }
    }

    //写入脚本代码
    static void WriteElements(StreamWriter sw, Transform trans)
    {
        string content = $"using UnityEngine.UI;\npublic class {trans.name + "UIElements"} : UIElements\n{{\n";
        var allTrans = trans.GetComponentsInChildren<Transform>(true);
        foreach (var tran in allTrans)
        {
            foreach (var kvp in dict)
            {
                if (tran.name.StartsWith(kvp.Value))
                {
                    var fieldName = tran.name.Replace(kvp.Value, "");
                    content += $"\tpublic {kvp.Key} {fieldName};\n";
                    break;
                }
            }
        }

        content += "}";
        sw.Write(content);
    }

    //根据成员变量获取对应节点名
    static string GetObjNameByField(FieldInfo filed)
    {
        if (dict.ContainsKey(filed.FieldType.Name))
        {
            return dict[filed.FieldType.Name] + filed.Name;
        }

        return "";
    }

    static Transform FindByName(Transform transform, string name)
    {
        var allTrans = transform.GetComponentsInChildren<Transform>(true);
        foreach (var tran in allTrans)
        {
            if (tran.name == name)
            {
                return tran;
            }
        }

        return null;
    }
}