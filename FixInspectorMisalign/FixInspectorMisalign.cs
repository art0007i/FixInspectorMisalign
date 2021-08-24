using HarmonyLib;
using NeosModLoader;
using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Collections.Generic;
using FrooxEngine;
using FrooxEngine.LogiX;
using FrooxEngine.UIX;
using BaseX;
using System.Reflection.Emit;

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

        [HarmonyPatch(typeof(PrimitiveMemberEditor))]
        [HarmonyPatch("BuildUI")]
        class PrimitiveMemberEditor_BuildUI_Patch
        {
            public static void Postfix(UIBuilder ui)
            {
                ui.Style.MinWidth = -1f;
            }
        }
        [HarmonyPatch(typeof(QuaternionMemberEditor))]
        [HarmonyPatch("BuildUI")]
        class QuaternionMemberEditor_BuildUI_Patch
        {
            static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
            {
                var foundIndex = -1;

                var codes = new List<CodeInstruction>(instructions);

                var alignConstructor = typeof(Alignment?).GetConstructor(new Type[] { typeof(Alignment) });
                var textMethod = typeof(UIBuilder).GetMethod("Text", new Type[] { typeof(FrooxEngine.LocaleString), typeof(Boolean), typeof(Alignment?), typeof(Boolean), typeof(String) });

                for (var i = 0; i < codes.Count; i++)
                {
                    //this checks if its an null algnment field, and saves the index for later
                    if (codes[i].opcode == OpCodes.Initobj && codes[i].operand == typeof(Alignment?) && codes[i - 1].opcode == OpCodes.Ldloca_S)
                    {
                        foundIndex = i;
                    }
                    //this looks for the text creation method, we want them to align to leftmiddle (3 as int)
                    if (codes[i].opcode == OpCodes.Callvirt && codes[i].operand is MethodInfo && (codes[i].operand as MethodInfo).Name == "Text" && foundIndex != -1)
                    {
                        //this edits stuff around the saved index to change alignment from null to leftmiddle
                        codes[foundIndex].opcode = OpCodes.Newobj;
                        codes[foundIndex].operand = alignConstructor;
                        codes[foundIndex - 1].opcode = OpCodes.Ldc_I4_3;
                        codes[foundIndex - 1].operand = null;
                        //should i be removing from the instructions list?
                        //for now i just do nops cause then i dont have to account for index shifts
                        codes[foundIndex + 1].opcode = OpCodes.Nop;
                        codes[foundIndex + 1].operand = null;
                        foundIndex = -1;
                    }
                }

                //This prints the il code, useful for debugging
                /*
                for(var i = 0; i < codes.Count; i++)
                {
                    UniLog.Log("IL_"+i.ToString("0000") + ": " + codes[i].ToString());
                }
                */
                return codes.AsEnumerable();
            }
        }
    }
}