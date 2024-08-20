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
        [HarmonyPatch(typeof(GameManager), nameof(GameManager.SaveGameAndDisplayHUDMessage))]

        public class SaveAfflictionData
        {
            public static void Postfix()
            {
                if (Mod.sdm != null && Mod.afflictionManager != null)
                {
                    Mod.Logger.Log($"Saving affliction list of {Mod.afflictionManager.GetCustomAfflictionCount()}", ComplexLogger.FlaggedLoggingLevel.Debug);
                    Mod.sdm.Save(new AfflictionManagerSaveDataProxy(Mod.afflictionManager.m_Afflictions));
                }
            }
        }

    }
}
