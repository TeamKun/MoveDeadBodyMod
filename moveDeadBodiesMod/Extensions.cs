
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using HarmonyLib;
using UnityEngine;
using Object = UnityEngine.Object;

namespace moveDeadBodiesMod
{
    public static class Extensions
    {
        public static bool IsPlayerCarry(this PlayerControl plr)
        {
            if (!plr.Data.IsImpostor && plr.Data.IsDead) return false;

            if (impoCarries[plr] != null)
            {
                return impoCarries[plr].Count > 0;
            }

            return false;
        }

        public static int getCarringCount(this PlayerControl plr)
        {
            if (!IsPlayerCarry(plr)) return 0;

            return impoCarries[plr].Count;
        }

        public static int GetId(this DeadBody deadBody)
        {
            return deadBodyIds[deadBody];
        }

        public static void SetId(this DeadBody deadBody)
        {
            if (deadBodyIds.ContainsKey(deadBody))
            {
                deadBodyIds.Remove(deadBody);
            }
            
            deadBodyIds.Add(deadBody , GetAvailableId());
        }

        public static int GetAvailableId()
        {
            DeadBody[] array = Object.FindObjectsOfType<DeadBody>();
            int id = 0;
            for (int i = 0; i < array.Length; i++)
            {
                id++;
            }

            return id;
        }

        public static bool IsDeadBodyCarried(this DeadBody deadBody)
        {
            foreach (List<DeadBody> deadBodies in impoCarries.Values)
            {
                if (deadBodies.Contains(deadBody))
                {
                    return true;
                }
            }

            return false;
        }

        public static void CarryBody(PlayerControl impostor, DeadBody deadBody)
        {
            if (!impoCarries.ContainsKey(impostor))
            {
                impoCarries.Add(impostor , new List<DeadBody>());
            }
            
            impoCarries[impostor].Add(deadBody);
            RegisterDeadBodyId(deadBody);
            
        }

        private static void RegisterDeadBodyId(DeadBody deadBody)
        {
            if (!deadBodyIds.ContainsKey(deadBody))
            {
                deadBodyIds.Add(deadBody , GetAvailableId());
            }
        }

        public static void SetTarget(this KillButtonManager killButtonManager, DeadBody target)
        {
            if (killButtonManager.CurrentTarget)
            {
                killButtonManager.CurrentTarget.GetComponent<SpriteRenderer>().material.SetFloat("_Outline" , 0f);
            }
            else if (buttonTargets.ContainsKey(killButtonManager) && buttonTargets[killButtonManager] != target)
            {
                buttonTargets[killButtonManager].GetComponent<SpriteRenderer>().material.SetFloat("_Outline" , 0f);
            }

            buttonTargets[killButtonManager] = target;

            if (buttonTargets[killButtonManager])
            {
                SpriteRenderer component = target.GetComponent<SpriteRenderer>();
                component.material.SetFloat("_Outline" , 1f);
                component.material.SetColor("_OutlineColor" , Color.red);
                return;
            }

        }
        
        public static class SetTargetPatch
        {
            [HarmonyPatch(typeof(KillButtonManager), nameof(KillButtonManager.SetTarget))]
            private static bool Prefix(KillButtonManager __instance , [HarmonyArgument(0)] PlayerControl target)
            {
                if (buttonTargets.ContainsKey(__instance) && buttonTargets[__instance] != null)
                {
                    buttonTargets[__instance].GetComponent<SpriteRenderer>().material.SetFloat("_Outline" , 0f);
                }

                return true;
            }
        }
        
        public static IDictionary<PlayerControl, List<DeadBody>> impoCarries = new Dictionary<PlayerControl, List<DeadBody>>();

        private static IDictionary<DeadBody, int> deadBodyIds = new Dictionary<DeadBody, int>();

        public static IDictionary<KillButtonManager, DeadBody> buttonTargets = new Dictionary<KillButtonManager, DeadBody>();

    }
}