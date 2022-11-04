using System.Reflection;
using BepInEx;
using HarmonyLib;

namespace ForgeFuelSaver
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
    [HarmonyPatch(typeof(TileEntityWorkstation))]
    static class TileEntityWorkstationPatch
    {
        private const int FUEL_MODULE = 3;
        private const int MATERIAL_INPUT_MODULE = 4;
        static bool HasRecipeInQueue(RecipeQueueItem[] queue)
        {
            for (int i = 0; i < queue.Length; i++)
            {
                if (queue[i].Multiplier > 0 && queue[i].Recipe != null) return true;
            }
            return false;
        }
        static bool HasItemsToMelt(float[] currentMeltTimesLeft)
        {
            for (int i = 0; i < currentMeltTimesLeft.Length; i++)
            {
                if (currentMeltTimesLeft[i] > 0) return true;
            }
            return false;
        }
        [HarmonyPatch(typeof(TileEntityWorkstation), "UpdateTick")]
        [HarmonyPrefix]
        static void UpdateTick(TileEntityWorkstation __instance, ref bool[] ___isModuleUsed, ref float[] ___currentMeltTimesLeft, ref RecipeQueueItem[] ___queue)
        {
            // Check basic entity state
            if (!___isModuleUsed[FUEL_MODULE]) return;
            if (!__instance.IsBurning) return;
            // Check content
            if (___isModuleUsed[MATERIAL_INPUT_MODULE] && HasItemsToMelt(___currentMeltTimesLeft)) return;
            if (HasRecipeInQueue(___queue)) return;
            // Don't turn off if busy
            if (__instance.IsUserAccessing()) return;
            // Save fuel
            __instance.IsBurning = false;
        }
    }
}
