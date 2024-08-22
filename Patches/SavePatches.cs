using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AfflictionComponent.Components;
using HarmonyLib;
using Il2Cpp;  

namespace AfflictionComponent.Patches
{
    internal class SavePatches
    {
        [HarmonyPatch(typeof(SaveGameSystem), nameof(SaveGameSystem.SaveGlobalData))]
        public class SaveAfflictionData
        {
            public static void Postfix()
            {
                if (Mod.sdm != null && Mod.afflictionManager != null)
                {
                    Mod.sdm.Save(new AfflictionManagerSaveDataProxy(Mod.afflictionManager.m_Afflictions));
                }
            }
        }

    }
}
