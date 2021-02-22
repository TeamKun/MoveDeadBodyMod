using System.Collections.Generic;
using System.IO;
using System.Linq;
using HarmonyLib;
using Reactor.Extensions;
using Reactor.Unstrip;
using UnityEngine;

namespace moveDeadBodiesMod
{

    [HarmonyPatch]
    public static class MoveDeadBodyUtils
    {
        public static AssetBundle bundle = AssetBundle.LoadFromFile(Directory.GetCurrentDirectory() + "\\Assets\\moveBundle");

        public static Sprite carryButton = bundle.LoadAsset<Sprite>("CB").DontUnload();

        public static int carryCount = 3;

        public static DeadBody GetClosestDeadBody()
        {
            double mindist = double.MaxValue;
            DeadBody closestDeadBody = null;
            DeadBody[] array = Object.FindObjectsOfType<DeadBody>();

            if (array.Length == 0)
            {
                return closestDeadBody;
            }

            foreach (DeadBody deadBody in array)
            {
                double dist = GetDistBetweenPlayerAndDeadBody(PlayerControl.LocalPlayer, deadBody);

                if (dist < mindist)
                {
                    mindist = dist;
                    closestDeadBody = deadBody;
                }
            }

            return closestDeadBody;
        }

        private static double GetDistBetweenPlayerAndDeadBody(PlayerControl player, DeadBody deadBody)
        {
            return Vector2.Distance(player.GetTruePosition(), deadBody.TruePosition);
        }
    }
}