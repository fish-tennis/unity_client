using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using cshap_client.game;
using UnityEngine;
using UnityEngine.UI;
public class ConsoleToUnityLog : TextWriter
{
    // 重写编码（可选，默认UTF-8）
    public override Encoding Encoding => Encoding.UTF8;

    // 重写WriteLine（核心逻辑）
    public override void WriteLine(string value)
    {
        // 可选：添加日志级别标记（如[Console]）
        Debug.Log($"[Console] {value}");
    }

    // 重写Write（处理无换行的情况）
    public override void Write(string value)
    {
        Debug.Log($"[Console] {value}");
    }

    // 其他可能需要重写的方法（根据原库使用情况）
    public override void WriteLine() => Debug.Log("");
    public override void Write(object value) => Debug.Log(value?.ToString());
}
public class LoginPanel : MonoBehaviour
{
    public InputField host;
    public InputField account;
    public InputField password;

    public Button btn;
    
    // Start is called before the first frame update
    private void Awake()
    {
        var originalOut = Console.Out;
    
        // 重定向到Unity日志
        Console.SetOut(new ConsoleToUnityLog());
        Client.Instance.Init(Application.dataPath + "/gen/message_command_mapping.json");
    }

    void Start()
    {
        btn.onClick.AddListener(OnClick);
    }

    private void Update()
    {
        Client.Instance.Run();
    }

    void OnClick()
    {
        Login.s_AccountName = account.text;
        Login.s_Password = password.text;
        Client.Instance.m_Connection.Connect(host.text);
       // Client.Instance.Shutdown();
    }
    // Update is called once per frame
}
