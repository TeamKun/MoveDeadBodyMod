
using System;
using System.Collections.Generic;
using HarmonyLib;
using Il2CppSystem.Net.Http.Headers;
using UnityEngine;
using Object = UnityEngine.Object;

namespace moveDeadBodiesMod
{
    public static class Extensions
    {
        public static bool IsPlayerCarry(byte playerId)
        {
            PlayerControl plr = getPlayerById(playerId);

            if (impoCarries[plr.PlayerId] != null)
            {
                return impoCarries[plr.PlayerId].Count > 0;
            }

            return false;
        }
        
        public static PlayerControl getPlayerById(byte id)
        {
            foreach (PlayerControl player in PlayerControl.AllPlayerControls)
            {
                if (player.PlayerId == id)
                {
                    return player;
                }
            }

            return null;
        }

        public static int getCarringCount(this PlayerControl plr)
        {
            return impoCarries[plr.PlayerId].Count;
        }
        
        public static int GetId(this DeadBody deadBody)
        {
            if (!deadBodyIds.ContainsKey(deadBody))
            {
                return Int32.MaxValue;
            }
            else
            {
                return deadBodyIds[deadBody];
            }
                
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

        public static bool IsDeadBodyCarried(DeadBody deadBody)
        {

            foreach (List<DeadBody> deadBodies in impoCarries.Values)
            {
                foreach (DeadBody dB in deadBodies)
                {
                    if (deadBody == dB)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public static void CarryBody(PlayerControl impostor, DeadBody deadBody)
        {
            if (!impoCarries.ContainsKey(impostor.PlayerId))
            {
                impoCarries.Add(impostor.PlayerId , new List<DeadBody>());
            }

            if (IsDeadBodyCarried(deadBody))
            {
                return;
            }

            impoCarries[impostor.PlayerId].Add(deadBody);
            RegisterDeadBodyId(deadBody);
            
        }
        
        
         private static void RegisterDeadBodyId(DeadBody deadBody)
        {
            if (!deadBodyIds.ContainsKey(deadBody))
            {
                deadBodyIds.Add(deadBody , GetAvailableId());
            }
        }
    

        public static void SetTarget(KillButtonManager killButtonManager, DeadBody target)
        {
            
            /*
            if (buttonTargets.ContainsKey(killButtonManager) && buttonTargets[killButtonManager] != target)
            {
                buttonTargets[killButtonManager].GetComponent<SpriteRenderer>().material.SetFloat("_Outline" , 0f);
            }
            */
            if (target == null) return;

            if (Vector2.Distance(PlayerControl.LocalPlayer.GetTruePosition(), target.transform.position) <= 3)
            {
                buttonTargets[killButtonManager] = target;
            }
            else
            {
                SpriteRenderer component = target.GetComponent<SpriteRenderer>();
                component.material.SetFloat("_Outline" , 1f);
                component.material.SetColor("_OutlineColor" , Color.clear);
                buttonTargets[killButtonManager] = null;
            }
            
            if (buttonTargets[killButtonManager] != null)
            {
                SpriteRenderer component = target.GetComponent<SpriteRenderer>();
                component.material.SetFloat("_Outline" , 1f);
                component.material.SetColor("_OutlineColor" , Color.green);
            }

        }

        public static IDictionary<byte, List<DeadBody>> impoCarries = new Dictionary<byte, List<DeadBody>>();

        private static IDictionary<DeadBody, int> deadBodyIds = new Dictionary<DeadBody, int>();

        public static IDictionary<KillButtonManager, DeadBody> buttonTargets = new Dictionary<KillButtonManager, DeadBody>();

    }
}