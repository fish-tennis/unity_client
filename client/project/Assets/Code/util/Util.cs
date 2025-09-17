using System;
using System.Collections.Generic;
using System.Linq;

namespace Code.util
{
    public static class Util
    {
        // 从原始配置数据中根据字段值创建索引字典
        public static Dictionary<int, Dictionary<int, T>> CreateDictionaryIndex<T>(Dictionary<int, T> data, Func<T, int> indexFn)
        {
            var indexDict = new Dictionary<int, Dictionary<int, T>>();
            foreach (var kvp in data)
            {
                var index = indexFn(kvp.Value);
                indexDict.TryGetValue(index, out var dict);
                if (dict == null)
                {
                    dict = new Dictionary<int, T>();
                    indexDict.Add(index, dict);
                }
                dict.Add(kvp.Key, kvp.Value);
            }
            return indexDict;
        }
        
        // 从原始配置数据中筛选出一个子集(字典)
        public static Dictionary<int, T> CreateSubset<T>(Dictionary<int, T> data, Func<T, bool> filter)
        {
            return data.Where(kvp => filter.Invoke(kvp.Value)).ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
        }
        
        // 从原始配置数据中筛选出一个列表,可选排序
        public static List<T> CreateList<T>(Dictionary<int, T> data, Func<T, bool> filter, IComparer<T> comparer = null)
        {
            var list = (from kvp in data where filter.Invoke(kvp.Value) select kvp.Value).ToList();
            if (comparer != null)
            {
                list.Sort(comparer);
            }
            return list;
        }
    }
}