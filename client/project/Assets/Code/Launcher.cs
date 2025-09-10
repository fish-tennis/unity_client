
using System;
using System.IO;
using System.Text;
using UnityEngine;
using cshap_client.game;

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
public class Launcher : MonoBehaviour
{
    private void Awake()
    {
        Console.SetOut(new ConsoleToUnityLog());
        Client.Instance.Init(Application.dataPath);
    }

    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        Client.Instance.Update(); // 逻辑刷新
    }
}
