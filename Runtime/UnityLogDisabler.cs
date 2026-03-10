#if PRODUCTION
using UnityEngine;
#endif

namespace Hlight.Debug.ULogger
{
    internal static class UnityLogDisabler
    {
#if PRODUCTION
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void TryToDisableUnityDebugLog()
        {
#if UNITY_EDITOR
            UnityEngine.Debug.LogError("<color=white><b>Unity Debug Log is disabled by the `PRODUCTION` compilation symbol.</b></color>");
#else
            UnityEngine.Debug.LogWarning("Unity Debug Log is disabled by the `PRODUCTION` compilation symbol.");
#endif
            UnityEngine.Debug.unityLogger.logEnabled = false;
        }
#endif
    }
}