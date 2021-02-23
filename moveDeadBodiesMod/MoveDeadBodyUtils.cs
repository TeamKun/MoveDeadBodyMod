using System.Collections.Generic;
using System.IO;
using System.Linq;
using HarmonyLib;
using Reactor.Extensions;
using Reactor.Unstrip;
using UnityEngine;

namespace moveDeadBodiesMod
{
    public static class MoveDeadBodyUtils
    {
        public static AssetBundle bundle = AssetBundle.LoadFromFile(Directory.GetCurrentDirectory() + "\\Assets\\movebundle");

        public static Sprite carryButton = bundle.LoadAsset<Sprite>("CB").DontUnload();
        public static Sprite dropButton = bundle.LoadAsset<Sprite>("DB").DontUnload();

        public static int carryCount = 3;
        public static float speed = float.MaxValue;
        public static DeadBody GetClosestDeadBody()
        {
            double mindist = double.MaxValue;
            DeadBody closestDeadBody = null;
            var array = Object.FindObjectsOfType<DeadBody>();

            foreach (DeadBody deadBody in array)
            {
                if (Extensions.IsDeadBodyCarried(deadBody))
                {
                    continue;
                }
                
                double dist = Vector2.Distance(PlayerControl.LocalPlayer.GetTruePosition(), deadBody.transform.position);

                if (dist < mindist)
                {
                    mindist = dist;
                    closestDeadBody = deadBody;
                }
            }
            return closestDeadBody;
        }
    }
}