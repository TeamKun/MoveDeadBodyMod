using System.Collections.Generic;
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
        public const string Id = "com.gmail.tomatan515";
        public const string ModName = "MoveDeadBodyMod";
        public const string ModVersion = "1.0";

        public Harmony Harmony { get; } = new Harmony(Id);

        public ConfigEntry<string> Name { get; private set; }

        public override void Load()
        {
            Harmony.PatchAll();
        }
    }
}
