using System.Text;
using UnityEditor;
using UnityEditor.ShortcutManagement;
using UnityEditor.Build;
using UnityEditor.Compilation;
using UnityEngine;
using ULog = ULogger;

namespace Hlight.Debug.ULogger.Editor
{
    internal static class LogScriptingDefineSymbolManager
    {
        private const KeyCode ADD_SYMBOL_SHORTCUT_KEY_CODE = KeyCode.D;

        private const ShortcutModifiers ADD_SYMBOL_SHORTCUT_MODIFIERS =
            ShortcutModifiers.Shift | ShortcutModifiers.Action;

        private static string GetAddSymbolShortcutKeyCombinationString()
        {
            var stringBuilder = new StringBuilder();

            stringBuilder.Append("<b>");

            foreach (ShortcutModifiers modifier in System.Enum.GetValues(typeof(ShortcutModifiers)))
            {
                if (modifier == ShortcutModifiers.None) continue;
                if (!ADD_SYMBOL_SHORTCUT_MODIFIERS.HasFlag(modifier)) continue;
                stringBuilder.Append(modifier);
                stringBuilder.Append(" + ");
            }

            stringBuilder.Append(ADD_SYMBOL_SHORTCUT_KEY_CODE);
            stringBuilder.Append("</b>");

            return stringBuilder.ToString();
        }

        private static bool IsMissingScriptingSymbol()
        {
            var nameBuildTarget =
                NamedBuildTarget.FromBuildTargetGroup(EditorUserBuildSettings.selectedBuildTargetGroup);
            var rawDefines = PlayerSettings.GetScriptingDefineSymbols(nameBuildTarget);
            var symbols = rawDefines.Split(';');
            return System.Array.IndexOf(symbols, ULog.SYMBOL) == -1;
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void TryToWarningMissingScriptingSymbolWhenPlay()
        {
            if (IsMissingScriptingSymbol())
                UnityEngine.Debug.LogError(
                    $"<color=white><b>[{nameof(ULogger)}]</b></color> All loggers are disabled because the scripting symbol '{ULog.SYMBOL}' is missing. Press {GetAddSymbolShortcutKeyCombinationString()} to add to Scripting Define Symbols.");
        }

        [Shortcut(nameof(TryAddLOGToScriptingDefineSymbol), ADD_SYMBOL_SHORTCUT_KEY_CODE,
            ADD_SYMBOL_SHORTCUT_MODIFIERS)]
        private static void TryAddLOGToScriptingDefineSymbol()
        {
            if (IsMissingScriptingSymbol())
                AddLOGToScriptingDefineSymbol();
        }

        private static void AddLOGToScriptingDefineSymbol()
        {
            if (EditorApplication.isPlayingOrWillChangePlaymode)
            {
                EditorApplication.ExitPlaymode();
                EditorApplication.delayCall += AddLOGToScriptingDefineSymbol;
                return;
            }

            var nameBuildTarget =
                NamedBuildTarget.FromBuildTargetGroup(EditorUserBuildSettings.selectedBuildTargetGroup);
            var rawDefines = PlayerSettings.GetScriptingDefineSymbols(nameBuildTarget);
            var newValue = string.IsNullOrEmpty(rawDefines)
                ? ULog.SYMBOL
                : $"{rawDefines};{ULog.SYMBOL}";
            PlayerSettings.SetScriptingDefineSymbols(nameBuildTarget, newValue);
            CompilationPipeline.RequestScriptCompilation();
        }
    }
}