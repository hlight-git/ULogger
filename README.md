# ULogger

Zero-overhead logging for Unity. All calls compile away when the `LOG` scripting symbol is removed.

## Quick Start

```csharp
// Info / Warning / Error
ULogger.Log("player spawned");
ULogger.LogW("health low:", hp);
ULogger.LogE("asset missing:", path);

// Multiple args (space-separated, no array allocation for 1-4 args)
ULogger.Log("pos:", pos, "rot:", rot);
```

## Tagged Logging

```csharp
// One-off
ULogger.Tag("Network").Log("connected to", serverName);
ULogger.Tag("Network", "#0F0").LogW("latency:", ms, "ms");

// Generic tag (uses type name)
ULogger.Tag<PlayerController>().LogE("null reference");

// With context object (clickable in Console to ping the object)
ULogger.Ctx(gameObject).Log("destroyed");
```

## Reusable TagLogger

Store a `TagLogger` as a field to avoid re-creating it every call. It is a `readonly struct` — no heap allocation.

```csharp
public class EnemyAI : MonoBehaviour
{
    static readonly TagLogger log = new("EnemyAI", "#F00");

    // Or with per-instance context:
    TagLogger ilog;

    void Awake()
    {
        ilog = log.Ctx(this);
    }

    void Update()
    {
        ilog.Log("target:", currentTarget);
        ilog.LogW("health low:", hp);
    }
}
```

### TagLogger API

| Method | Description |
|---|---|
| `new TagLogger(name, color?, context?)` | Constructor |
| `.Ctx(Object)` | Returns copy with Unity Object context |
| `.WithColor(string)` | Returns copy with different hex color |
| `.Log(...)` | Info |
| `.LogW(...)` | Warning |
| `.LogE(...)` | Error |

## Scripting Symbols

| Symbol | Effect |
|---|---|
| `LOG` | **Required.** All `ULogger` / `TagLogger` calls compile away without it. Editor shortcut: **Shift+Cmd+D** (Mac) / **Shift+Ctrl+D** (Win) to add. |
| `PRODUCTION` | **Optional.** Disables Unity's built-in `Debug.unityLogger` entirely at startup. |

## Console Double-Click Navigation

All classes and methods follow Unity's stack-trace skipping convention:

- Class names end with **`Logger`** (`ULogger`, `TagLogger`)
- Method names start with **`Log`** (`Log`, `LogW`, `LogE`, `LogInternal`, `LogManyInternal`)

Unity's Console skips these frames automatically, so **double-clicking a log entry navigates directly to your call-site**.

## Performance

| Feature | Detail |
|---|---|
| Conditional compilation | `[Conditional("LOG")]` — zero cost when symbol absent |
| Overloaded args | 1–4 arg overloads avoid `params object[]` array allocation |
| StringBuilder pooling | Thread-static `StringBuilderCache` reuses builders (max 1024 capacity) |
| HashSet pooling | Thread-static pooled `HashSet<int>` for circular-reference detection |
| Readonly struct | `TagLogger` is a value type — no GC pressure when stored as a field |
| Collection rendering | `IDictionary` → `{k: v}`, `IEnumerable` → `[a, b, c]`, max depth 4 |
| Rich text tags | `<color><b>[Tag]</b></color>` prefix only in Editor (auto-detected) |

## Project Structure

```
Runtime/
  Log.cs               ULogger   — static entry point + internal emit logic
  LogTag.cs            TagLogger — reusable tagged logger (readonly struct)
  LogFormatter.cs      Tag prefix formatting + collection rendering
  StringBuilderCache.cs Thread-static StringBuilder pool
  UnityLogDisabler.cs  Disables Debug.unityLogger when PRODUCTION defined

Editor/
  LogScriptingDefineSymbolManager.cs  Warns on missing LOG symbol + shortcut to add it
```

**Assembly:** `Hlight.Debug.ULogger` (Runtime), `Hlight.Debug.ULogger.Editor` (Editor-only)
