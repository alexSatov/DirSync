using System.Collections.Generic;

namespace DirSync.Extensions
{
    internal static class EnumerableExtensions
    {
        internal static string ToLinesText<T>(this IEnumerable<T> lines)
        {
            return string.Join("\r\n", lines);
        }
    }
}
