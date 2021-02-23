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
                    Extensions.impoCarries.Add(__instance.PlayerId , new List<DeadBody>());
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
        public class VentPatch
        {
            [HarmonyPatch(typeof(PlayerPhysics), nameof(PlayerPhysics.RpcEnterVent))]
            private static void Postfix(PlayerPhysics __instance)
            {
                PlayerControl player = __instance.myPlayer;

                if (!Extensions.IsPlayerCarry(player.PlayerId)) return;

                List<DeadBody> deadBodies = Extensions.impoCarries[player.PlayerId];
            
                for (int i = 0; i < deadBodies.Count; i++)
                {
                    deadBodies[i].transform.position = new Vector2(player.GetTruePosition().x , player.GetTruePosition().y + i*0.3F);
                    var w = AmongUsClient.Instance.StartRpc(player.NetId, (byte) 44, SendOption.Reliable); 
                    w.Write(deadBodies[i].ParentId);
                    w.Write(i);
                    w.EndMessage();
                }

                Extensions.impoCarries[player.PlayerId].Clear();
            }
        }
    }
}