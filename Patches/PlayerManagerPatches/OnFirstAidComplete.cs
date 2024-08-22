using AfflictionComponent.Components;

namespace AfflictionComponent.Patches.PlayerManagerPatches;

internal static class OnFirstAidComplete
{
    [HarmonyPatch(typeof(PlayerManager), nameof(PlayerManager.OnFirstAidComplete))]
    private static class OnFirstAidCompleteOverride
    {
        private static bool Prefix(PlayerManager __instance, ref bool success, ref bool playerCancel, ref float progress)
        {
            if (PanelAfflictionPatches.GlobalFields.selectedCustomAffliction != null)
            {
                CustomAffliction selected = PanelAfflictionPatches.GlobalFields.selectedCustomAffliction;

                if (!success)
                {
                    if (__instance.m_UsedItemFromFirstAidPanel)
                    {
                        __instance.m_UsedItemFromFirstAidPanel = false;
                        InterfaceManager.GetPanel<Panel_FirstAid>().ProgressBarCancel();
                    }
                    return false;
                }
                
                GameManager.GetConditionComponent().AddHealth(__instance.m_HealingOnFirstAidComplete, DamageSource.FirstAid);
                
                if (!__instance.m_FirstAidItemUsed)
                {
                    return false;
                }
                
                if (!selected.RequiresRemedyItem(__instance.m_FirstAidItemUsed))
                {
                    Mod.Logger.Log("Not the right healing item!", ComplexLogger.FlaggedLoggingLevel.Debug);

                    GearItem component = __instance.m_FirstAidItemUsed.GetComponent<GearItem>();
                    __instance.FirstAidConsumed(component);
                    __instance.m_FirstAidItemUsed = null;
                    HUDMessage.AddMessage(Localization.Get("GAMEPLAY_TreatmentDidNotDoAnything"));
                    
                    if (!string.IsNullOrEmpty(__instance.m_TreatmentFailedAudio))
                    {
                        GameManager.GetPlayerVoiceComponent().Play(__instance.m_TreatmentFailedAudio, Il2CppVoice.Priority.Normal);
                    }
                    
                    return false;
                }

                selected.ApplyRemedy(__instance.m_FirstAidItemUsed);

                GearItem component2 = __instance.m_FirstAidItemUsed.GetComponent<GearItem>();
                __instance.FirstAidConsumed(component2);
                __instance.m_FirstAidItemUsed = null;
                
                if (__instance.m_UsedItemFromFirstAidPanel)
                {
                    __instance.m_UsedItemFromFirstAidPanel = false;
                    InterfaceManager.GetPanel<Panel_FirstAid>().FirstAidItemCallback();
                }
                
                return false;
            }
            else
                return true;
        }
    }
}