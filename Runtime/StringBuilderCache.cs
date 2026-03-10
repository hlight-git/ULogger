using System;
using System.Text;

namespace Hlight.Debug.ULogger
{
    internal static class StringBuilderCache
    {
        private const int DefaultCapacity = 256;
        private const int MaxCachedCapacity = 1024;

        [ThreadStatic] private static StringBuilder s_cached;

        internal static StringBuilder Acquire(int capacity = DefaultCapacity)
        {
            var sb = s_cached;
            if (sb != null && sb.Capacity >= capacity)
            {
                s_cached = null;
                sb.Clear();
                return sb;
            }
            return new StringBuilder(Math.Max(capacity, DefaultCapacity));
        }

        internal static string Release(StringBuilder sb)
        {
            var result = sb.ToString();
            if (sb.Capacity <= MaxCachedCapacity)
            {
                sb.Clear();
                s_cached ??= sb;
            }
            return result;
        }
    }
}
