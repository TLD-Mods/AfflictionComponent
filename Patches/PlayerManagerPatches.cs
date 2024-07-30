using AfflictionComponent.Components;
using Unity.VisualScripting;

namespace AfflictionComponent.Patches;

internal static class PlayerManagerPatches
{
    // Copy and pasted this method from 2.02 mono code, this allows the information in the afflictions panel to change for some reason?
    [HarmonyPatch(typeof(PlayerManager), nameof(PlayerManager.UseFirstAidItem))]
    internal static class UseFirstAidItemPatch
    {
        private static bool Prefix(PlayerManager __instance, GearItem gi, ref bool __result)
        {
            if (gi.m_StackableItem && GameManager.GetInventoryComponent().NumGearInInventory(gi.name) < gi.m_FirstAidItem.m_UnitsPerUse)
            {
                GameAudioManager.PlayGUIError();
                HUDMessage.AddMessage(Localization.Get("GAMEPLAY_PillsRequiredValue").Replace("{num-pills}", gi.m_FirstAidItem.m_UnitsPerUse.ToString()), false, false);
                return false;
            }
            Panel_Affliction.AfflictionExclusions afflictionExclusions = Panel_Affliction.AfflictionExclusions.MajorWristSprain | Panel_Affliction.AfflictionExclusions.MustBeTreatable;
            Il2CppSystem.Collections.Generic.List<Affliction> list = new();
            Panel_Affliction.GetAllBadAfflictions(afflictionExclusions, list);
            if (list.Count == 0 && AfflictionManager.GetAfflictionManagerInstance().GetCustomAfflictionCount() == 0)
            {
                __instance.TreatAfflictionWithFirstAid(gi.m_FirstAidItem, Affliction.InvalidAffliction);
                return false;
            }
            InterfaceManager.GetPanel<Panel_Affliction>().Enable(true, list, gi.m_FirstAidItem);
            return false;
        }
    }

    [HarmonyPatch(typeof(PlayerManager), nameof(PlayerManager.OnFirstAidComplete))]

    private static class OnFirstAidCompleteOverride
    {

        public static bool Prefix(PlayerManager __instance, ref bool success, ref bool playerCancel, ref float progress)
        {

            Mod.Logger.Log($"Affliction selected valid: {__instance.m_AfflictionSelected.m_IsValid}", ComplexLogger.FlaggedLoggingLevel.Debug);
            Mod.Logger.Log($"Affliction selected location: {__instance.m_AfflictionSelected.m_Location.ToString()}", ComplexLogger.FlaggedLoggingLevel.Debug);

            if (PanelAfflictionPatches.selectedCustomAffliction != null)
            {
                Mod.Logger.Log("CUSTOM affliction selected", ComplexLogger.FlaggedLoggingLevel.Debug);

                CustomAffliction selected = PanelAfflictionPatches.selectedCustomAffliction;

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
                if (!selected.RequiresRemedy(__instance.m_FirstAidItemUsed))
                {
                    Mod.Logger.Log("Not the right healing item!", ComplexLogger.FlaggedLoggingLevel.Debug);

                    GearItem component = __instance.m_FirstAidItemUsed.GetComponent<GearItem>();
                    __instance.FirstAidConsumed(component);
                    __instance.m_FirstAidItemUsed = null;
                    HUDMessage.AddMessage(Localization.Get("GAMEPLAY_TreatmentDidNotDoAnything"), false, false);
                    if (!string.IsNullOrEmpty(__instance.m_TreatmentFailedAudio))
                    {
                        GameManager.GetPlayerVoiceComponent().Play(__instance.m_TreatmentFailedAudio, Il2CppVoice.Priority.Normal, PlayerVoice.Options.None, null);
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
            else {
                Mod.Logger.Log("Vanilla method being called!", ComplexLogger.FlaggedLoggingLevel.Debug);
                return true;
            } 
        }
    }

}