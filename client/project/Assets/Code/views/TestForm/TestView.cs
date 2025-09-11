using System;
using System.Collections.Generic;
using cshap_client.game;
using UnityEngine;

public class TestView : ViewBase<TestViewModel, TestModel>
{
    public TestFormUIElements UI => UIElements as TestFormUIElements;

    private long time;

    private void Start()
    {
        BindingContext = new TestViewModel();
        time = (DateTime.Now.Ticks - 621355968000000000) / 10000000;
    }

    public void OnChanged_Color(Color oldVal, Color newVal)
    {
        UI.Image1.color = newVal;
    }

    public void OnChanged_Time(string oldVal, string newVal)
    {
        UI.T2.text = newVal;
    }

    public void OnChanged_BaseInfo(Gserver.BaseInfo oldVal, Gserver.BaseInfo newVal)
    {
        Debug.LogWarning("TestView.OnChanged_BaseInfo");
        UI.PlayerName.text = Client.Instance.Player.Name;
        UI.PlayerLevel.text = newVal.Level.ToString();
        UI.PlayerExp.text = newVal.Exp.ToString();
        // List<BaseInfo> Tab = new List<BaseInfo>(){};
        // List<TaskItem> tasks = new List<TaskItem>();
        // List<TaskItem> cachae = new List<TaskItem>();
        // for (int i = 0; i < Tab.Count; i++)
        // {
        //     var a = GetATaskItem(i);
        //     // update ui
        //     a:RefreshRate(Tab[i])
        // }
    }
    
    public void OnClick_B3()
    {
        BindingContext.Color.Value -= new Color(0.1f, 0.15f, 0.1f, 0);
        time++;
        SendCommand("SetTime", new object[] {time});
    }
    
}