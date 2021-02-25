using System;
using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using Hazel;
using Il2CppSystem.Collections.Generic;
using Reactor.Extensions;
using UnhollowerBaseLib;
using UnityEngine;
using Object = UnityEngine.Object;

namespace moveDeadBodiesMod
{
    public class RPC
    {
        public enum CustomRPC
        {
            DropPlayer = 44 ,
            CarryPlayer = 45 ,
            SetID = 46
        }

        [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.HandleRpc))]
        class HandleRpcPatch
        {
            static void Postfix(PlayerControl __instance ,[HarmonyArgument(0)] byte HKHMBLJFLMC, [HarmonyArgument(1)] MessageReader ALMCIJKELCP)
            {
                byte packetId = HKHMBLJFLMC;
                MessageReader reader = ALMCIJKELCP;

                var deadBodies = Object.FindObjectsOfType<DeadBody>();

                switch (packetId)
                {
                    case (byte) CustomRPC.DropPlayer:
                        byte idd = reader.ReadByte();
                        DeadBody ddeadBody = deadBodies.First(x => x.ParentId == idd);
                        int i1 = reader.ReadInt32();
                        ddeadBody.transform.position = new Vector2(__instance.GetTruePosition().x , __instance.GetTruePosition().y + i1*(0.3F));
                        Rigidbody2D rb1 = ddeadBody.gameObject.GetComponent<Rigidbody2D>();
                        rb1.Destroy();
                        break;
                    case (byte) CustomRPC.CarryPlayer:
                        //Rpcに紐づけられたNetIdのプレイヤーが、__instance.
                        byte idc = reader.ReadByte();
                        DeadBody cdeadBody = deadBodies.First(x => x.ParentId == idc);

                        //int i2 = reader.ReadInt32();
                        //cdeadBody.transform.position = new Vector2(__instance.GetTruePosition().x , __instance.GetTruePosition().y + 1 + i2*(0.3F));

                        Vector2 force = new Vector2(__instance.GetTruePosition().x - cdeadBody.transform.position.x,
                            __instance.GetTruePosition().y - cdeadBody.transform.position.y) * 8;

                        cdeadBody.gameObject.AddComponent<Rigidbody2D>();
                        Rigidbody2D rb = cdeadBody.gameObject.GetComponent<Rigidbody2D>();
                        rb.velocity = force;

                        /*
                         *                         foreach (DeadBody deadBody in Bodies)
                        {
                            deadBody.transform.position = new Vector2(__instance.GetTruePosition().x , __instance.GetTruePosition().y + 1 + i*(0.3F));
                            __instance.transform.position = new Vector3(deadBody.ParentId , deadBody.ParentId);
                        }
                         */

                        break;
                }
            }
        }
    }
}