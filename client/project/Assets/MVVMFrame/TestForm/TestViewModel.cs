using System;
using UnityEngine;
public class TestViewModel : ViewModelBase<TestModel>
{
    public BindableProperty<Color> Color = new BindableProperty<Color>(UnityEngine.Color.white);
    public BindableProperty<string> Time = new BindableProperty<string>("");

    public override void OnInitialize()
    {
        
    }

    public void OnChanged_SeverTime(long oldVal, long newVal)
    {
        Time.Value = GetDateTimeMilliseconds(newVal).ToString();
    }

    public static DateTime GetDateTimeMilliseconds(long timestamp)
    {
        long time_tricks = timestamp * 10000000 + 621355968000000000;
        DateTime dt = new DateTime(time_tricks); //转化为DateTime
        return dt;
    }
}

public class TestModel : ModelBase
{
    public BindableProperty<long> SeverTime = new BindableProperty<long>(0);

    public override void OnInitialization()
    {
        //添加网络消息监听,接收数据
    }

    public void OnCommond_SetTime(long time)
    {
        SeverTime.Value = time;
    }
}
