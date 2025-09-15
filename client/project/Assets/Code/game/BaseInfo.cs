using System;

namespace Code.game
{
    // 玩家的基础信息组件
    public class BaseInfo : BasePlayerComponent
    {
        public const string ComponentName = "BaseInfo";
        public Gserver.BaseInfo data;
        public BaseInfo(Player player) : base(ComponentName, player)
        {
        }

        // 同步数据
        public void OnBaseInfoSync(Gserver.BaseInfoSync res)
        {
            data = res.Data;
            Console.WriteLine("OnBaseInfoSync:" + data);
            // 先写临时代码,后续改成通用代码
            ViewMgr.ViewMgr.Instance.OnDataUpdate("Player.Level");
            ViewMgr.ViewMgr.Instance.OnDataUpdate("Player.Exp");
        }
    }
}
