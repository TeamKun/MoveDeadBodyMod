
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
        public static int getId(this DeadBody body)
        {
            if (!deadBodyIds.ContainsKey(body))
            {
                return Int32.MaxValue;
            }
            
            return deadBodyIds[body];
        }
        public static void setId(this DeadBody body)
        {
            if (!deadBodyIds.Values.Contains(body.getId()))
            {
                deadBodyIds.Add(body, getAvailableId());
            }
        }

        public static int getAvailableId()
        {
            int id = 0;

            for (int i = 0; i < deadBodyIds.Count; i++)
            {
                id++;
            }

            return id;
        }
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

        public static DeadBody getDeadBodyById(byte id)
        {
            foreach (var deadBody in Object.FindObjectsOfType<DeadBody>())
            {
                if (deadBody.ParentId == id)
                {
                    return deadBody;
                }
            }
            return null;
        }
        public static int getCarringCount(this PlayerControl plr)
        {
            return impoCarries[plr.PlayerId].Count;
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

        }
        public static void SetTarget(KillButtonManager killButtonManager, DeadBody target)
        {
            if (target == null) return;

            if (Vector2.Distance(PlayerControl.LocalPlayer.GetTruePosition(), target.transform.position) <= 1)
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
            
            if(buttonTargets[killButtonManager] != null)
            {
                SpriteRenderer component = target.GetComponent<SpriteRenderer>();
                component.material.SetFloat("_Outline" , 1f);
                component.material.SetColor("_OutlineColor" , Color.green);
            }
        }

        public static IDictionary<byte, List<DeadBody>> impoCarries = new Dictionary<byte, List<DeadBody>>();
        public static IDictionary<DeadBody, int> deadBodyIds = new Dictionary<DeadBody, int>();
        public static IDictionary<KillButtonManager, DeadBody> buttonTargets = new Dictionary<KillButtonManager, DeadBody>();

    }
}