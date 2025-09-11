using System;
using UnityEditor.PackageManager;
using UnityEngine;
using Client = cshap_client.game.Client;

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
    public BindableProperty<Gserver.BaseInfo> BaseInfo = new BindableProperty<Gserver.BaseInfo>(null);

    public override void OnInitialization()
    {
        Debug.Log("TestModel.OnInitialization");
        BaseInfo.Value = Client.Instance.Player.BaseInfo.data;
    }

    public void OnCommand_SetTime(long time)
    {
        SeverTime.Value = time;
    }
    
    // 同步玩家基础信息
    public void OnBaseInfoSync(Gserver.BaseInfoSync res)
    {
        Debug.Log("TestViewModel.OnBaseInfoSync");
        BaseInfo.Value = res.Data;
    }
}
