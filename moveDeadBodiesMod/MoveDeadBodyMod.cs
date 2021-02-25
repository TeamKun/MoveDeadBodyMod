using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using BepInEx;
using BepInEx.Configuration;
using BepInEx.IL2CPP;
using HarmonyLib;
using Reactor;

namespace moveDeadBodiesMod
{
    [BepInPlugin(Id , ModName , ModVersion)]
    [BepInProcess("Among Us.exe")]
    [BepInDependency(ReactorPlugin.Id)]
    public class MoveDeadBodyMod : BasePlugin
    {
        public static BepInEx.Logging.ManualLogSource log;
        
        public const string Id = "com.gmail.tomatan515";
        public const string ModName = "MoveDeadBodyMod";
        public const string ModVersion = "1.2";

        public Harmony Harmony { get; } = new Harmony(Id);

        public override void Load()
        {
            Harmony.PatchAll();
        }
    }
}
