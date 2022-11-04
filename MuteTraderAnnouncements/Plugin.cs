using System.Reflection;
using BepInEx;
using HarmonyLib;
using UnityEngine;

namespace MuteTraderAnnouncements
{
    [BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
    public class Plugin : BaseUnityPlugin
    {
        private static readonly Harmony harmony = new(PluginInfo.PLUGIN_GUID);
        [System.Diagnostics.CodeAnalysis.SuppressMessage("CodeQuality", "IDE0051:Remove unused private members", Justification = "Called by Unity")]
        private void Awake() => harmony.PatchAll(Assembly.GetExecutingAssembly());
        [System.Diagnostics.CodeAnalysis.SuppressMessage("CodeQuality", "IDE0051:Remove unused private members", Justification = "Called by Unity")]
        private void OnDestroy() => harmony.UnpatchSelf();
    }
    [HarmonyPatch(typeof(Audio.Manager))]
    static class AudioManagerPatch
    {
        [HarmonyPatch(typeof(Audio.Manager), "Play", new[] { typeof(Vector3), typeof(string), typeof(int) })]
        [HarmonyPatch(typeof(Audio.Manager), "Play", new[] { typeof(Entity), typeof(string), typeof(int), typeof(bool) })]
        [HarmonyPrefix]
        static bool Play(ref string __1)
        {
            // Filter annoying announcements
            if (__1.Contains("trader_announce")) return false;
            return true;
        }
        // TODO: LoadAudio _clipName patch?
    }
    [HarmonyPatch(typeof(BlockSpeakerTrader))]
    static class BlockSpeakerTraderPatch
    {
        [HarmonyPatch(typeof(BlockSpeakerTrader), "PlayClose")]
        [HarmonyPatch(typeof(BlockSpeakerTrader), "PlayOpen")]
        [HarmonyPatch(typeof(BlockSpeakerTrader), "PlayWarning")]
        [HarmonyPrefix]
        static bool Play()
        {
            // Prevent broadcasting
            return false;
        }
    }
    [HarmonyPatch(typeof(TraderInfo))]
    static class TraderInfoPatch
    {
        [HarmonyPatch(typeof(TraderInfo), "ShouldPlayOpenSound", MethodType.Getter)]
        [HarmonyPatch(typeof(TraderInfo), "ShouldPlayCloseSound", MethodType.Getter)]
        [HarmonyPatch(typeof(TraderInfo), "IsWarningTime", MethodType.Getter)]
        [HarmonyPrefix]
        static bool Getter(ref bool __result)
        {
            // Prevent broadcasting
            __result = false;
            return false;
        }
    }
}
