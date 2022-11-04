using System.Reflection;
using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;
using UnityEngine;
using System.Collections.Generic;

namespace FpsLimiter
{
    [BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
    public class Plugin : BaseUnityPlugin
    {
        // Config
        public static new readonly ConfigFile Config = new($"{Paths.ConfigPath}/{PluginInfo.PLUGIN_GUID}.cfg", true);
        public static ConfigEntry<int> TargetFps { get; private set; } = Config.Bind(
            new ConfigDefinition("FpsLimiter", "TargetFps"),
            0,
            new ConfigDescription("Run 'settargetfps <amount> command on startup'", new AcceptableValueRange<int>(0, int.MaxValue))
           );
        public static ConfigEntry<bool> SavePowerIfNotFocused { get; private set; } = Config.Bind(
            new ConfigDefinition("FpsLimiter", "SavePowerIfNotFocused"),
            true,
            new ConfigDescription("Run 'settargetfps <amount> on window focus change'")
           );
        public static ConfigEntry<bool> Debug { get; private set; } = Config.Bind(
            new ConfigDefinition("Dev", "Debug"),
            false,
            new ConfigDescription("Show when patches are executed")
           );
        // Patching
        private static readonly Harmony harmony = new(PluginInfo.PLUGIN_GUID);
        [System.Diagnostics.CodeAnalysis.SuppressMessage("CodeQuality", "IDE0051:Remove unused private members", Justification = "Called by Unity")]
        private void Awake()
        {
            Config.SaveOnConfigSet = true;
            harmony.PatchAll(Assembly.GetExecutingAssembly());
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("CodeQuality", "IDE0051:Remove unused private members", Justification = "Called by Unity")]
        private void OnDestroy() => harmony.UnpatchSelf();
    }
    [HarmonyPatch(typeof(GameManager))]
    static class TargetFpsPatch
    {
        [HarmonyPatch(typeof(GameManager), "Awake")]
        [HarmonyPostfix]
        static void Awake(GameManager __instance)
        {
            // Restored saved TargetFPS
            if (Plugin.Debug.Value) Debug.Log($"Restoring TargetFPS to {Plugin.TargetFps.Value}");
            __instance.waitForTargetFPS.TargetFPS = Plugin.TargetFps.Value;
        }
        [HarmonyPatch(typeof(ConsoleCmdSetTargetFps), "Execute")]
        [HarmonyPostfix]
        static void Execute(List<string> __0)
        {
            // Check and parse new fps target value
            if (__0.Count != 1) return;
            int.TryParse(__0[0], out int parsedFps);
            if (parsedFps < 0) return;
            // Save TargetFPS to config
            Plugin.TargetFps.Value = parsedFps;
        }
        [HarmonyPatch(typeof(GameManager), "GameIsFocused", MethodType.Setter)]
        [HarmonyPrefix]
        static void GameIsFocused(bool __0, GameManager __instance)
        {
            // Check if changing TargetFPS is possible
            if (!__instance.waitForTargetFPS) return;
            // Check if should save power
            if (!Plugin.SavePowerIfNotFocused.Value) return;
            // Save power if game is in background (!focused)
            if (!__0)
            {
                if (Plugin.Debug.Value) Debug.Log($"Game not focused - TargetFPS: 1");
                __instance.waitForTargetFPS.TargetFPS = 1;
                return;
            }
            if (Plugin.Debug.Value) Debug.Log($"Game focused - TargetFPS: {Plugin.TargetFps.Value}");
            // Restore saved TargetFPS
            __instance.waitForTargetFPS.TargetFPS = Plugin.TargetFps.Value;
        }
    }
}