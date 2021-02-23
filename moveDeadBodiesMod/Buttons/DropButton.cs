using System;
using System.Collections.Generic;
using HarmonyLib;
using Hazel;
using Il2CppSystem.Xml;
using Il2CppSystem.Xml.Schema;
using Reactor;
using Reactor.Extensions;
using UnityEngine;
using UnityEngine.Events;

namespace moveDeadBodiesMod
{
    public class DropButton
    {
        public static List<DropButton> AllButtons = new List<DropButton>();
        private readonly HudManager _hudManager;
        public KillButtonManager ButtonManager;

        public float MaxTimer = 3;
        private float timer;

        public DropButton(HudManager hudManager)
        {
            _hudManager = hudManager;
            AllButtons.Add(this);
            OnStart();
            ButtonManager.gameObject.SetActive(false);
        }

        void OnStart()
        {
            ButtonManager = GameObject.Instantiate(_hudManager.KillButton, _hudManager.transform);
            ButtonManager.renderer.sprite = MoveDeadBodyUtils.dropButton;
            ButtonManager.SetCoolDown(timer , MaxTimer);
            timer = MaxTimer;
            ButtonManager.renderer.SetCooldownNormalizedUvs();
            ButtonManager.GetComponent<PassiveButton>().OnClick.AddListener((UnityAction) OnClick);
            void OnClick()
            {
                if (CanUse())
                {
                    Use();
                    timer = MaxTimer;
                }
            }
        }
        
        void OnUpdate()
        {
            ButtonManager.transform.localPosition = new Vector3(_hudManager.ReportButton.transform.localPosition.x * -1,
                _hudManager.ReportButton.transform.localPosition.y, _hudManager.ReportButton.transform.localPosition.z) + new Vector3(0.2f, 0.2f);

            if (PlayerControl.LocalPlayer.Data.IsImpostor && !PlayerControl.LocalPlayer.Data.IsDead)
            {
                ButtonManager.gameObject.SetActive(true);
            }

            if (timer > 0)
            {
                timer -= Time.deltaTime;
                ButtonManager.SetCoolDown(timer , MaxTimer);
            }

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

        }
        bool CanUse()
        {
            return Extensions.IsPlayerCarry(PlayerControl.LocalPlayer.PlayerId);
        }

        void Use()
        {
            PlayerControl player = PlayerControl.LocalPlayer;
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
        
        [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
        static class HudManagerPatch
        {
            static void Prefix()
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
                    //System.Console.WriteLine("DropButton processing was failed.");
                }
            }
        }
    }
}