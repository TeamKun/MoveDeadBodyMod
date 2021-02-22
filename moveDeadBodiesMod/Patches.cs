using HarmonyLib;
using Il2CppSystem;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace moveDeadBodiesMod
{
    public class Patches
    {
        [HarmonyPatch(typeof(HudManager), nameof(HudManager.Start))]
        static class ButtonPatch
        {
            static void Postfix(HudManager __instance)
            {
                new CarryButton(__instance);
            }
        }
    }
}