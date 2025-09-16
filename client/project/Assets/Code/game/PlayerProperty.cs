using System.Collections.Generic;

namespace Code.game
{
    using PlayerPropertyGetter = System.Func<Player, string, int>;
    using PlayerPropertyGetterString = System.Func<Player, string, string>;
    
    public static class PlayerProperty
    {
        // 玩家属性值接口 提供一个统一的属性值查询接口
        // 可以在这边把不同模块的值整合到统一的接口里
        // 由Player.GetPropertyInt32调用
        public static Dictionary<string,PlayerPropertyGetter> Getters = new Dictionary<string, PlayerPropertyGetter>
        {
            {"Level",(player,_)=> player.BaseInfo.data?.Level??0}, // 等级
            {"Exp",(player,_)=> player.BaseInfo.data?.Exp??0}, // 等级
            {"TotalPay",(player,_)=> player.BaseInfo.data?.TotalPay??0}, // 总支付金额
            {"FinishQuestCount",(player,_)=> player.GetQuest().Finished.Count}, // 完成任务数量
        };
        
        public static Dictionary<string,PlayerPropertyGetterString> StringGetters = new Dictionary<string, PlayerPropertyGetterString>
        {
            {"Name",(player,_)=> player.Name}, // 玩家名
        };
    }
}