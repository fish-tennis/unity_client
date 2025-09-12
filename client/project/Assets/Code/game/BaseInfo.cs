using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Code.ViewMgr;
using Microsoft.SqlServer.Server;


namespace cshap_client.game
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
            ViewMgr.Instance.OnDataUpdate("Player.Level");
            ViewMgr.Instance.OnDataUpdate("Player.Exp");
        }
    }
}
