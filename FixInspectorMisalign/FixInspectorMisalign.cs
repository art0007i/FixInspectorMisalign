using HarmonyLib;
using NeosModLoader;
using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Collections.Generic;
using FrooxEngine;
using FrooxEngine.LogiX;

namespace FixInspectorMisalign
{
    public class FixInspectorMisalign : NeosMod
    {
        public override string Name => "FixInspectorMisalign";
        public override string Author => "art0007i";
        public override string Version => "1.0.0";
        public override string Link => "https://github.com/art0007i/FixInspectorMisalign/";
        public override void OnEngineInit()
        {
            Harmony harmony = new Harmony("me.art0007i.FixInspectorMisalign");
            harmony.PatchAll();

        }
        [HarmonyPatch(typeof(LogixHelper), "GetImpulseTargets")]
        class TemplatePatch
        {
            public static bool Prefix()
            {
                return false;
            }
        }
    }
}