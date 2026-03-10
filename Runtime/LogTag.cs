using System.Diagnostics;
using UnityEngine;

/// <summary>
/// Lightweight tagged logger. Store as a <c>static readonly</c> field for zero-cost reuse.
/// <code>
/// static readonly TagLogger log = new("EnemyAI", "#F00");
/// log.Log("spawned at", transform.position);
/// </code>
/// <para>Class/method naming follows Unity's stack-trace convention so double-clicking
/// a Console entry navigates to the actual call-site.</para>
/// </summary>
public readonly struct TagLogger
{
    internal readonly string Name;
    internal readonly string Color;
    internal readonly Object Context;

    public TagLogger(string name, string color = null, Object context = null)
    {
        Name = name;
        Color = color;
        Context = context;
    }

    /// <summary>Returns a copy with the given Unity Object as log context (clickable in Console).</summary>
    public TagLogger Ctx(Object context) => new(Name, Color, context);

    /// <summary>Returns a copy with a different tag color (hex string, e.g. "#0F0").</summary>
    public TagLogger WithColor(string color) => new(Name, color, Context);

    // ── Info ─────────────────────────────────────────────────────

    [Conditional(ULogger.SYMBOL)]
    public void Log(object a) => ULogger.LogInternal(LogType.Log, Name, Color, Context, a);

    [Conditional(ULogger.SYMBOL)]
    public void Log(object a, object b) => ULogger.LogInternal(LogType.Log, Name, Color, Context, a, b);

    [Conditional(ULogger.SYMBOL)]
    public void Log(object a, object b, object c) => ULogger.LogInternal(LogType.Log, Name, Color, Context, a, b, c);

    [Conditional(ULogger.SYMBOL)]
    public void Log(object a, object b, object c, object d)
        => ULogger.LogInternal(LogType.Log, Name, Color, Context, a, b, c, d);

    [Conditional(ULogger.SYMBOL)]
    public void Log(object a, object b, object c, object d, params object[] rest)
        => ULogger.LogManyInternal(LogType.Log, Name, Color, Context, a, b, c, d, rest);

    // ── Warning ──────────────────────────────────────────────────

    [Conditional(ULogger.SYMBOL)]
    public void LogW(object a) => ULogger.LogInternal(LogType.Warning, Name, Color, Context, a);

    [Conditional(ULogger.SYMBOL)]
    public void LogW(object a, object b) => ULogger.LogInternal(LogType.Warning, Name, Color, Context, a, b);

    [Conditional(ULogger.SYMBOL)]
    public void LogW(object a, object b, object c)
        => ULogger.LogInternal(LogType.Warning, Name, Color, Context, a, b, c);

    [Conditional(ULogger.SYMBOL)]
    public void LogW(object a, object b, object c, object d)
        => ULogger.LogInternal(LogType.Warning, Name, Color, Context, a, b, c, d);

    [Conditional(ULogger.SYMBOL)]
    public void LogW(object a, object b, object c, object d, params object[] rest)
        => ULogger.LogManyInternal(LogType.Warning, Name, Color, Context, a, b, c, d, rest);

    // ── Error ────────────────────────────────────────────────────

    [Conditional(ULogger.SYMBOL)]
    public void LogE(object a) => ULogger.LogInternal(LogType.Error, Name, Color, Context, a);

    [Conditional(ULogger.SYMBOL)]
    public void LogE(object a, object b) => ULogger.LogInternal(LogType.Error, Name, Color, Context, a, b);

    [Conditional(ULogger.SYMBOL)]
    public void LogE(object a, object b, object c)
        => ULogger.LogInternal(LogType.Error, Name, Color, Context, a, b, c);

    [Conditional(ULogger.SYMBOL)]
    public void LogE(object a, object b, object c, object d)
        => ULogger.LogInternal(LogType.Error, Name, Color, Context, a, b, c, d);

    [Conditional(ULogger.SYMBOL)]
    public void LogE(object a, object b, object c, object d, params object[] rest)
        => ULogger.LogManyInternal(LogType.Error, Name, Color, Context, a, b, c, d, rest);
}
