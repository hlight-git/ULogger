using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using UnityEngine;

namespace Hlight.Debug.ULogger
{
    internal static class LogFormatter
    {
        private const int MaxDepth = 4;

        private static readonly bool s_richText;

        static LogFormatter()
        {
            try { s_richText = Application.installMode == ApplicationInstallMode.Editor; }
            catch { s_richText = false; }
        }

        // Pooled per-thread to avoid allocation on every enumerable render.
        [System.ThreadStatic] private static HashSet<int> s_visited;

        // ── Public API ───────────────────────────────────────────

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static void WritePrefix(StringBuilder sb, string tag, string color)
        {
            if (tag == null) return;

            if (s_richText)
            {
                sb.Append("<color=").Append(color ?? "#FFF").Append("><b>[")
                    .Append(tag).Append("]</b></color> ");
            }
            else
            {
                sb.Append('[').Append(tag).Append("] ");
            }
        }

        internal static void WriteValue(StringBuilder sb, object value)
        {
            if (value == null) { sb.Append("null"); return; }
            if (value is string s) { sb.Append(s); return; }

            if (value is IDictionary dict)
            {
                var visited = AcquireVisited();
                try { WriteDict(sb, dict, MaxDepth, visited); }
                finally { ReleaseVisited(visited); }
                return;
            }

            if (value is IEnumerable enumerable)
            {
                var visited = AcquireVisited();
                try { WriteList(sb, enumerable, MaxDepth, visited); }
                finally { ReleaseVisited(visited); }
                return;
            }

            AppendSafe(sb, value);
        }

        // ── Recursive internals ──────────────────────────────────

        private static void WriteInner(StringBuilder sb, object value, int depth, HashSet<int> visited)
        {
            if (depth <= 0) { sb.Append("..."); return; }
            if (value == null) { sb.Append("null"); return; }
            if (value is string s) { sb.Append(s); return; }
            if (value is IDictionary dict) { WriteDict(sb, dict, depth, visited); return; }
            if (value is IEnumerable enumerable) { WriteList(sb, enumerable, depth, visited); return; }
            AppendSafe(sb, value);
        }

        private static void WriteList(StringBuilder sb, IEnumerable list, int depth, HashSet<int> visited)
        {
            var id = RuntimeHelpers.GetHashCode(list);
            if (!visited.Add(id)) { sb.Append("[...]"); return; }

            try
            {
                sb.Append('[');
                var first = true;
                foreach (var item in list)
                {
                    if (!first) sb.Append(", ");
                    WriteInner(sb, item, depth - 1, visited);
                    first = false;
                }
                sb.Append(']');
            }
            finally { visited.Remove(id); }
        }

        private static void WriteDict(StringBuilder sb, IDictionary dict, int depth, HashSet<int> visited)
        {
            var id = RuntimeHelpers.GetHashCode(dict);
            if (!visited.Add(id)) { sb.Append("{...}"); return; }

            try
            {
                sb.Append('{');
                var first = true;
                foreach (DictionaryEntry e in dict)
                {
                    if (!first) sb.Append(", ");
                    WriteInner(sb, e.Key, depth - 1, visited);
                    sb.Append(": ");
                    WriteInner(sb, e.Value, depth - 1, visited);
                    first = false;
                }
                sb.Append('}');
            }
            finally { visited.Remove(id); }
        }

        // ── Helpers ──────────────────────────────────────────────

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void AppendSafe(StringBuilder sb, object value)
        {
            try { sb.Append(value); }
            catch { sb.Append("<error>"); }
        }

        private static HashSet<int> AcquireVisited()
        {
            var v = s_visited;
            if (v != null) { s_visited = null; return v; }
            return new HashSet<int>();
        }

        private static void ReleaseVisited(HashSet<int> v)
        {
            v.Clear();
            s_visited = v;
        }
    }
}
