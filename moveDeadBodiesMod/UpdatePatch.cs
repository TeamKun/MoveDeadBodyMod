using HarmonyLib;

namespace moveDeadBodiesMod
{
    public class UpdatePatch
    {
        [HarmonyPatch(typeof(GameOptionsData), nameof(GameOptionsData.Method_24))]
        class GameOptionsData_ToHudString
        {
            static void Postfix(ref string __result)
            {
                DestroyableSingleton<HudManager>.Instance.GameSettings.scale = 0.5f;
            }
        }
        
        private static float defaultBounds = 0f;

        [HarmonyPatch(typeof(GameOptionsMenu), nameof(GameOptionsMenu.Start))]
        class Start
        {
            static void Postfix(ref GameOptionsMenu __instance)
            {
                defaultBounds = __instance.GetComponentInParent<Scroller>().YBounds.max;
            }
        }

        [HarmonyPatch(typeof(GameOptionsMenu), nameof(GameOptionsMenu.Update))]
        class Update
        {
            static void Postfix(ref GameOptionsMenu __instance)
            {
                __instance.GetComponentInParent<Scroller>().YBounds.max = 13.5f;
            }
        }
        
        [HarmonyPatch(typeof(VersionShower), nameof(VersionShower.Start))]
        public static class VersionStartPatch
        {
            static void Postfix(VersionShower __instance)
            {
                __instance.text.Text = __instance.text.Text + MoveDeadBodyMod.ModName + " ver." + MoveDeadBodyMod.ModVersion + " Loaded. Author:(twitter.com/nier_Automatan)";
            }
        }
        
    }
}