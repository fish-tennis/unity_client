using System.Collections.Generic;

namespace Code.cfg
{
    public static class ItemCfgHelper
    {
        // 返回值: 物品A*1 物品B*2
        public static string GetItemStrings(IEnumerable<Gserver.ItemNum> items, string separator)
        {
            if (items == null)
            {
                return "";
            }
            var itemStrings = new List<string>();
            foreach (var itemNum in items)
            {
                if (!DataMgr.ItemCfgs.TryGetValue(itemNum.CfgId, out var itemCfg))
                {
                    continue;
                }
                itemStrings.Add($"{itemCfg.Name}x{itemNum.Num}");
            }
            return string.Join(separator, itemStrings);
        }

        // 返回值: 物品A*1 物品B*2
        public static string GetItemStrings(IEnumerable<Gserver.AddElemArg> items, string separator)
        {
            if (items == null)
            {
                return "";
            }
            var itemStrings = new List<string>();
            foreach (var itemNum in items)
            {
                if (!DataMgr.ItemCfgs.TryGetValue(itemNum.CfgId, out var itemCfg))
                {
                    continue;
                }
                itemStrings.Add($"{itemCfg.Name}x{itemNum.Num}");
            }
            return string.Join(separator, itemStrings);
        }
        
    }
}