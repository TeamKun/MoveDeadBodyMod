using System.Collections;
using HarmonyLib;
using System.Collections.Generic;
using Hazel;
using Il2CppSystem;
using PowerTools;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace moveDeadBodiesMod
{
    [HarmonyPatch]
    public class Patches
    {
        [HarmonyPatch(typeof(HudManager), nameof(HudManager.Start))]
        static class ButtonPatch
        {
            static void Postfix(HudManager __instance)
            {
                new CarryButton(__instance);
                new DropButton(__instance);
            }
        }

        [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.FixedUpdate))]
        public static class PlayerControlUpdatePatch
        {
            public static void Postfix(PlayerControl __instance)
            {
                /*
                 *                 if (MoveDeadBodyUtils.speed == float.MaxValue)
                {
                    MoveDeadBodyUtils.speed = __instance.MyPhysics.Speed;
                }
                 */

                if (!Extensions.impoCarries.ContainsKey(__instance.PlayerId) && __instance.Data.IsImpostor)
                {
                    Extensions.impoCarries.Add(__instance.PlayerId, new List<DeadBody>());
                }

                /*
                 *                 if (Extensions.IsPlayerCarry(__instance.PlayerId))
                {
                    __instance.MyPhysics.Speed = MoveDeadBodyUtils.speed * 0.75F;
                }
                else
                {
                    __instance.MyPhysics.Speed = MoveDeadBodyUtils.speed;
                }
                 */
            }
        }
    }
}