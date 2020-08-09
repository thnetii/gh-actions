using System;

namespace THNETII.GitHubActions.Toolkit.Core
{
    internal static class StringReplaceExtensions
    {
        internal static string ReplaceOrdinal(this string str, string oldString, string newString)
        {
            return str.Replace(oldString, newString
#if NETSTANDARD2_1
                , StringComparison.Ordinal
#endif
                );
        }
    }
}
