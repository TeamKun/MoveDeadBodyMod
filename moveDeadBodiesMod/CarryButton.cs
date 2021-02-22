using System.Collections.Generic;
using HarmonyLib;
using Reactor.Extensions;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.Windows.WebCam;

namespace moveDeadBodiesMod
{
    public class CarryButton
    {
        public static List<CarryButton> AllButtons = new List<CarryButton>();
        private readonly HudManager _hudManager;
        public KillButtonManager buttonManager;

        public float MaxTimer = 15;
        private float timer;

        public CarryButton(HudManager hudManager)
        {
            _hudManager = hudManager;
            AllButtons.Add(this);
            onStart();
            buttonManager.gameObject.SetActive(true);
            System.Console.WriteLine("Generated.");
        }
        
        void onStart()
        {
            buttonManager = GameObject.Instantiate(_hudManager.KillButton, _hudManager.transform);
            buttonManager.renderer.sprite = MoveDeadBodyUtils.carryButton;
            buttonManager.SetCoolDown(timer , MaxTimer);
            timer = MaxTimer;
            buttonManager.renderer.SetCooldownNormalizedUvs();
            Extensions.SetTarget(buttonManager , null);
            
            buttonManager.GetComponent<PassiveButton>().OnClick.AddListener((UnityAction) OnClick);

            void OnClick()
            {
                PlayerControl.LocalPlayer.SetColor(3);
                if (CanUse())
                {
                    Use(Extensions.buttonTargets[buttonManager]);
                    timer = MaxTimer;
                }
            }
        }

        void OnUpdate()
        {
            buttonManager.transform.localPosition = buttonManager.transform.localPosition = new Vector3(
                    (_hudManager.UseButton.transform.localPosition.x) * -1,
                    _hudManager.UseButton.transform.localPosition.y, _hudManager.KillButton.transform.localPosition.z) +
                new Vector3(0.2f, 0.2f);

            if (timer > 0)
            {
                timer -= Time.deltaTime;
                buttonManager.SetCoolDown(timer , MaxTimer);
            }
            
            if (!Extensions.buttonTargets.ContainsKey(buttonManager))
            {
                Extensions.buttonTargets.Add(buttonManager , null);
            }

            Extensions.SetTarget(buttonManager , MoveDeadBodyUtils.GetClosestDeadBody());

            if (CanUse())
            {
                buttonManager.renderer.color = Palette.EnabledColor;
                buttonManager.renderer.material.SetFloat("_Desat", 0f);
            }
            else
            {
                buttonManager.renderer.color = new Color(1f, 1f, 1f, 0.3f);
                buttonManager.renderer.material.SetFloat("_Desat", 1f);
            }

            foreach (byte playerId in Extensions.impoCarries.Keys)
            {
                PlayerControl player = Extensions.getPlayerById(playerId);
                List<DeadBody> deadBodies = Extensions.impoCarries[playerId];

                for (int i = 0; i < deadBodies.Count; i++)
                {
                    deadBodies[i].transform.position = new Vector2(player.GetTruePosition().x , player.GetTruePosition().y + 1 + i*(0.3F));
                }
            }
        }

        bool CanUse()
        {
            PlayerControl localPlayer = PlayerControl.LocalPlayer;

            bool canUse = localPlayer.Data.IsImpostor && !localPlayer.Data.IsDead;
            canUse &= Extensions.getCarringCount(localPlayer) < MoveDeadBodyUtils.carryCount;

            DeadBody target = Extensions.buttonTargets[buttonManager];

            if (target != null)
            {
                canUse &= !Extensions.IsDeadBodyCarried(target);
            }
            else
            {
                return false;
            }

            float num = Vector2.Distance(localPlayer.GetTruePosition() , target.transform.position);

            canUse &= num <= 3;
            
            return canUse && (timer < 0);

        }

        void Use(DeadBody deadBody)
        {
            PlayerControl localPlayer = PlayerControl.LocalPlayer;
            Extensions.CarryBody(localPlayer , deadBody);
        }

        //HudManager -> server用
        [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
        static class HudManagerPatch
        {
            static void Prefix()
            {
                try
                {
                    AllButtons.RemoveAll(item => item.buttonManager == null);

                    foreach (var button in AllButtons)
                    {
                        button.OnUpdate();
                    }
                }
                catch
                {
                    
                }

            }
        }
    }
}