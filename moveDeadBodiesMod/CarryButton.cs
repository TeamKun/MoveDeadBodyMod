using System.Collections.Generic;
using HarmonyLib;
using Il2CppSystem.Xml.Schema;
using Reactor.Extensions;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.Windows.WebCam;

namespace moveDeadBodiesMod
{
    public class CarryButton
    {
        private static List<CarryButton> AllButtons = new List<CarryButton>();
        private readonly HudManager _hudManager;
        public KillButtonManager buttonManager;
        
        public float MaxTimer;
        private float timer;

        public CarryButton(HudManager hudManager)
        {
            _hudManager = hudManager;
            AllButtons.Add(this);
            onStart();
        }

        void OnUpdate()
        {
            Extensions.SetTarget(buttonManager , MoveDeadBodyUtils.GetClosestDeadBody());
            
            if (CanUse())
            {
                buttonManager.renderer.color = Palette.EnabledColor;
                buttonManager.renderer.material.SetFloat("_Desat", 0f);
                buttonManager.enabled = true;
            }
            else
            {
                buttonManager.renderer.color = new Color(1f, 1f, 1f, 0.3f);
                buttonManager.renderer.material.SetFloat("_Desat", 1f);
                buttonManager.enabled = false;
            }

            foreach (PlayerControl player in Extensions.impoCarries.Keys)
            {
                List<DeadBody> deadBodies = Extensions.impoCarries[player];

                for (int i = 0; i < deadBodies.Count; i++)
                {
                    deadBodies[i].transform.position = new Vector2(player.GetTruePosition().x , player.GetTruePosition().y + (i+1)*3);
                }
                
            }
            
        }

        void onStart()
        {
            buttonManager = GameObject.Instantiate(_hudManager.KillButton, _hudManager.transform);
            buttonManager.renderer.sprite = MoveDeadBodyUtils.carryButton;
            buttonManager.SetCoolDown(timer , MaxTimer);
            timer = MaxTimer;
            buttonManager.renderer.SetCooldownNormalizedUvs();
            buttonManager.transform.localPosition = new Vector3((buttonManager.transform.localPosition.x + 1.3f) * -1, buttonManager.transform.localPosition.y, buttonManager.transform.localPosition.z);

            buttonManager.GetComponent<PassiveButton>().OnClick.AddListener((UnityAction) OnClick);

            void OnClick()
            {
                if (CanUse())
                {
                    Use();
                    timer = MaxTimer;
                }
            }
        }

        bool CanUse()
        {
            PlayerControl localPlayer = PlayerControl.LocalPlayer;

            bool canUse = localPlayer.Data.IsImpostor && !localPlayer.Data.IsDead;
            canUse &= Extensions.getCarringCount(localPlayer) < MoveDeadBodyUtils.carryCount;

            DeadBody target = Extensions.buttonTargets[buttonManager];

            canUse &= target != null;

            float num = Vector2.Distance(localPlayer.GetTruePosition() , target.TruePosition);

            canUse &= num <= 3;

            return canUse && timer < 0;

        }

        void Use()
        {
            PlayerControl localPlayer = PlayerControl.LocalPlayer;
            
        }

        [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
        class HudManagerPatch
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