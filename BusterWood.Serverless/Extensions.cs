using System.Collections.Generic;

namespace BusterWood.Serverless
{
    static class Extensions
    {
        public static void AddRange<TKey, TValue>(this IDictionary<TKey, TValue> dict, IEnumerable<KeyValuePair<TKey, TValue>> items)
        {
            foreach (var item in items)
            {
                dict.Add(item);
            }
        }
    }
}
