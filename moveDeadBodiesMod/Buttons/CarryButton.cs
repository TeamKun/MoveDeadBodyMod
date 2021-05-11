using System;
using System.Collections.Generic;
using HarmonyLib;
using Hazel;
using Reactor.Extensions;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.Windows.WebCam;
using Object = UnityEngine.Object;

namespace moveDeadBodiesMod
{
    public class CarryButton
    {
        public static List<CarryButton> AllButtons = new List<CarryButton>();
        private readonly HudManager _hudManager;
        public KillButtonManager ButtonManager;

        public float MaxTimer = 3;
        private float timer;

        public CarryButton(HudManager hudManager)
        {
            _hudManager = hudManager;
            AllButtons.Add(this);
            onStart();
            ButtonManager.gameObject.SetActive(false);
        }
        
        void onStart()
        {
            ButtonManager = GameObject.Instantiate(_hudManager.KillButton, _hudManager.transform);
            ButtonManager.renderer.sprite = MoveDeadBodyUtils.carryButton;
            ButtonManager.SetCoolDown(timer , MaxTimer);
            timer = MaxTimer;
            ButtonManager.renderer.SetCooldownNormalizedUvs();
            Extensions.SetTarget(ButtonManager , null);
            ButtonManager.GetComponent<PassiveButton>().OnClick.AddListener((UnityAction) OnClick);
            void OnClick()
            {
                if (CanUse())
                {
                    Use(Extensions.buttonTargets[ButtonManager]);
                    timer = MaxTimer;
                }
            }
        }
        void OnUpdate()
        {
            ButtonManager.renderer.sprite = MoveDeadBodyUtils.carryButton;

            ButtonManager.transform.localPosition = new Vector3(
                    (_hudManager.UseButton.transform.localPosition.x) * -1,
                    _hudManager.UseButton.transform.localPosition.y, _hudManager.KillButton.transform.localPosition.z) +
                new Vector3(0.2f, 0.2f);

            if (!PlayerControl.LocalPlayer.Data.IsDead && PlayerControl.LocalPlayer.Data.IsImpostor)
            {
                ButtonManager.gameObject.SetActive(true);
            }

            if (timer > 0)
            {
                timer -= Time.deltaTime;
                ButtonManager.SetCoolDown(timer , MaxTimer);
            }
            
            if (!Extensions.buttonTargets.ContainsKey(ButtonManager))
            {
                Extensions.buttonTargets.Add(ButtonManager , null);
            }

            Extensions.SetTarget(ButtonManager , MoveDeadBodyUtils.GetClosestDeadBody());

            if (CanUse())
            {
                ButtonManager.renderer.color = Palette.EnabledColor;
                ButtonManager.renderer.material.SetFloat("_Desat", 0f);
            }
            else
            {
                ButtonManager.renderer.color = new Color(1f, 1f, 1f, 0.3f);
                ButtonManager.renderer.material.SetFloat("_Desat", 1f);
            }
            
            foreach (byte playerId in Extensions.impoCarries.Keys)
            {
                PlayerControl player = Extensions.getPlayerById(playerId);
                List<DeadBody> deadBodies = Extensions.impoCarries[playerId];

                for (int i = 0; i < deadBodies.Count; i++)
                {
                    //deadBodies[i].transform.position = new Vector2(player.GetTruePosition().x , player.GetTruePosition().y + 1 + i*(0.3F));

                    if (i == 0)
                    {
                        Vector2 force = new Vector2(player.GetTruePosition().x - deadBodies[i].transform.position.x , 
                            player.GetTruePosition().y - deadBodies[i].transform.position.y) * 8;

                        deadBodies[i].gameObject.AddComponent<Rigidbody2D>();
                        Rigidbody2D rb = deadBodies[i].gameObject.GetComponent<Rigidbody2D>();
                        rb.velocity = force;
                        var w = AmongUsClient.Instance.StartRpc(player.NetId, (byte) 45, SendOption.Reliable); 
                        w.Write(deadBodies[i].ParentId);
                        w.Write(i);
                        w.EndMessage();
                        continue;
                    }

                    Vector2 force1 = new Vector2(
                        deadBodies[i - 1].transform.position.x - deadBodies[i].transform.position.x,
                        deadBodies[i - 1].transform.position.y - deadBodies[i].transform.position.y) * 8;

                    deadBodies[i].gameObject.AddComponent<Rigidbody2D>();
                    Rigidbody2D rb1 = deadBodies[i].gameObject.GetComponent<Rigidbody2D>();
                    rb1.velocity = force1;
                    var w1 = AmongUsClient.Instance.StartRpc(player.NetId, (byte) 45, SendOption.Reliable); 
                    w1.Write(deadBodies[i].ParentId);
                    w1.Write(i);
                    w1.EndMessage();
                    return;
                    
                }
            }
        }

        bool CanUse()
        {
            PlayerControl localPlayer = PlayerControl.LocalPlayer;

            bool canUse = localPlayer.Data.IsImpostor && !localPlayer.Data.IsDead;
            canUse &= Extensions.getCarringCount(localPlayer) < 2;

            DeadBody target = Extensions.buttonTargets[ButtonManager];

            if (target != null)
            {
                canUse &= !Extensions.IsDeadBodyCarried(target);
            }
            else
            {
                return false;
            }

            float num = Vector2.Distance(localPlayer.GetTruePosition() , target.transform.position);

            canUse &= num <= 1;
            
            return canUse && (timer < 0);

        }

        void Use(DeadBody deadBody)
        {
            PlayerControl localPlayer = PlayerControl.LocalPlayer;
            Extensions.CarryBody(localPlayer , deadBody);

            var w = AmongUsClient.Instance.StartRpc(localPlayer.NetId, (byte) 45, SendOption.Reliable); 
            w.Write(deadBody.ParentId);
            w.Write(3);
            w.EndMessage();
            
        }

        //HudManager -> server用
        [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
        static class HudManagerPatch
        {
            static void Postfix()
            {
                try
                {
                    AllButtons.RemoveAll(item => item.ButtonManager == null);

                    foreach (var button in AllButtons)
                    {
                        button.OnUpdate();
                    }

                }
                catch
                {
                    //System.Console.WriteLine("CarryButton processing was failed.");
                }

            }
        }
    }
}