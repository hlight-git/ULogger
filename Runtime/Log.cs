using System.Diagnostics;
using System.Text;
using Hlight.Debug.ULogger;
using UnityEngine;

/// <summary>
/// Zero-overhead logging facade. All calls compile away when the <c>LOG</c> symbol is absent.
/// <para><c>ULogger.Log("msg")</c> · <c>ULogger.LogW("msg")</c> · <c>ULogger.LogE("msg")</c></para>
/// <para><c>ULogger.Tag("Net").Log("connected")</c> · <c>ULogger.Tag&lt;T&gt;().LogE("err")</c></para>
/// <para>Class/method naming follows Unity's stack-trace convention so double-clicking
/// a Console entry navigates to the actual call-site.</para>
/// </summary>
public static class ULogger
{
    public const string SYMBOL = "LOG";

    // ── Info ─────────────────────────────────────────────────────

    [Conditional(SYMBOL)]
    public static void Log(object a) => LogInternal(LogType.Log, null, null, null, a);

    [Conditional(SYMBOL)]
    public static void Log(object a, object b) => LogInternal(LogType.Log, null, null, null, a, b);

    [Conditional(SYMBOL)]
    public static void Log(object a, object b, object c) => LogInternal(LogType.Log, null, null, null, a, b, c);

    [Conditional(SYMBOL)]
    public static void Log(object a, object b, object c, object d)
        => LogInternal(LogType.Log, null, null, null, a, b, c, d);

    [Conditional(SYMBOL)]
    public static void Log(object a, object b, object c, object d, params object[] rest)
        => LogManyInternal(LogType.Log, null, null, null, a, b, c, d, rest);

    // ── Warning ──────────────────────────────────────────────────

    [Conditional(SYMBOL)]
    public static void LogW(object a) => LogInternal(LogType.Warning, null, null, null, a);

    [Conditional(SYMBOL)]
    public static void LogW(object a, object b) => LogInternal(LogType.Warning, null, null, null, a, b);

    [Conditional(SYMBOL)]
    public static void LogW(object a, object b, object c)
        => LogInternal(LogType.Warning, null, null, null, a, b, c);

    [Conditional(SYMBOL)]
    public static void LogW(object a, object b, object c, object d)
        => LogInternal(LogType.Warning, null, null, null, a, b, c, d);

    [Conditional(SYMBOL)]
    public static void LogW(object a, object b, object c, object d, params object[] rest)
        => LogManyInternal(LogType.Warning, null, null, null, a, b, c, d, rest);

    // ── Error ────────────────────────────────────────────────────

    [Conditional(SYMBOL)]
    public static void LogE(object a) => LogInternal(LogType.Error, null, null, null, a);

    [Conditional(SYMBOL)]
    public static void LogE(object a, object b) => LogInternal(LogType.Error, null, null, null, a, b);

    [Conditional(SYMBOL)]
    public static void LogE(object a, object b, object c)
        => LogInternal(LogType.Error, null, null, null, a, b, c);

    [Conditional(SYMBOL)]
    public static void LogE(object a, object b, object c, object d)
        => LogInternal(LogType.Error, null, null, null, a, b, c, d);

    [Conditional(SYMBOL)]
    public static void LogE(object a, object b, object c, object d, params object[] rest)
        => LogManyInternal(LogType.Error, null, null, null, a, b, c, d, rest);

    // ── Factory ──────────────────────────────────────────────────

    public static TagLogger Tag(string name) => new(name, null, null);
    public static TagLogger Tag(string name, string color) => new(name, color, null);
    public static TagLogger Tag<T>() => new(typeof(T).Name, null, null);
    public static TagLogger Tag<T>(string color) => new(typeof(T).Name, color, null);
    public static TagLogger Ctx(Object context) => new(null, null, context);

    // ── Internal (method prefix "Log" + class suffix "Logger" = skipped by Unity) ──

    internal static void LogInternal(LogType t, string tag, string color, Object ctx, object a)
    {
        var sb = StringBuilderCache.Acquire();
        LogFormatter.WritePrefix(sb, tag, color);
        LogFormatter.WriteValue(sb, a);
        UnityEngine.Debug.unityLogger.Log(t, (object)StringBuilderCache.Release(sb), ctx);
    }

    internal static void LogInternal(LogType t, string tag, string color, Object ctx, object a, object b)
    {
        var sb = StringBuilderCache.Acquire();
        LogFormatter.WritePrefix(sb, tag, color);
        LogFormatter.WriteValue(sb, a);
        sb.Append(' ');
        LogFormatter.WriteValue(sb, b);
        UnityEngine.Debug.unityLogger.Log(t, (object)StringBuilderCache.Release(sb), ctx);
    }

    internal static void LogInternal(LogType t, string tag, string color, Object ctx,
        object a, object b, object c)
    {
        var sb = StringBuilderCache.Acquire();
        LogFormatter.WritePrefix(sb, tag, color);
        LogFormatter.WriteValue(sb, a);
        sb.Append(' ');
        LogFormatter.WriteValue(sb, b);
        sb.Append(' ');
        LogFormatter.WriteValue(sb, c);
        UnityEngine.Debug.unityLogger.Log(t, (object)StringBuilderCache.Release(sb), ctx);
    }

    internal static void LogInternal(LogType t, string tag, string color, Object ctx,
        object a, object b, object c, object d)
    {
        var sb = StringBuilderCache.Acquire();
        LogFormatter.WritePrefix(sb, tag, color);
        LogFormatter.WriteValue(sb, a);
        sb.Append(' ');
        LogFormatter.WriteValue(sb, b);
        sb.Append(' ');
        LogFormatter.WriteValue(sb, c);
        sb.Append(' ');
        LogFormatter.WriteValue(sb, d);
        UnityEngine.Debug.unityLogger.Log(t, (object)StringBuilderCache.Release(sb), ctx);
    }

    internal static void LogManyInternal(LogType t, string tag, string color, Object ctx,
        object a, object b, object c, object d, object[] rest)
    {
        var sb = StringBuilderCache.Acquire();
        LogFormatter.WritePrefix(sb, tag, color);
        LogFormatter.WriteValue(sb, a);
        sb.Append(' ');
        LogFormatter.WriteValue(sb, b);
        sb.Append(' ');
        LogFormatter.WriteValue(sb, c);
        sb.Append(' ');
        LogFormatter.WriteValue(sb, d);
        for (var i = 0; i < rest.Length; i++)
        {
            sb.Append(' ');
            LogFormatter.WriteValue(sb, rest[i]);
        }
        UnityEngine.Debug.unityLogger.Log(t, (object)StringBuilderCache.Release(sb), ctx);
    }
}
