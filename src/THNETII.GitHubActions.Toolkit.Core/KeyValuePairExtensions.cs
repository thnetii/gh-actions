using System;
using System.Collections.Generic;
using System.Text;

namespace THNETII.GitHubActions.Toolkit.Core
{
    internal static class KeyValuePairExtensions
    {
        internal static KeyValuePair<TKey, TValue> AsKeyValuePair<TKey, TValue>(
            this ValueTuple<TKey, TValue> tuple) =>
            new KeyValuePair<TKey, TValue>(tuple.Item1, tuple.Item2);

        internal static KeyValuePair<TKey, TValue> AsKeyValuePair<TKey, TValue>(
            this Tuple<TKey, TValue> tuple)
        {
            _ = tuple ?? throw new ArgumentNullException(nameof(tuple));
            return new KeyValuePair<TKey, TValue>(tuple.Item1, tuple.Item2);
        }

        internal static void Deconstruct<TKey, TValue>(
            this KeyValuePair<TKey, TValue> kvp, out TKey key, out TValue value) =>
            (key, value) = (kvp.Key, kvp.Value);
    }
}
