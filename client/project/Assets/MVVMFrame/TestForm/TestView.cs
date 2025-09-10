using System;
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

    public void OnClick_B3()
    {
        BindingContext.Color.Value -= new Color(0.1f, 0.15f, 0.1f, 0);
        time++;
        SendCommond("SetTime", new object[] {time});
    }
    
}