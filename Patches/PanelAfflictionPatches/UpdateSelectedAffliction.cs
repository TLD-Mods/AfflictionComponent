using AfflictionComponent.Components;

namespace AfflictionComponent.Patches.PanelAfflictionPatches;

internal static class UpdateSelectedAffliction
{
    [HarmonyPatch(typeof(Panel_Affliction), nameof(Panel_Affliction.UpdateSelectedAffliction), typeof(Affliction))]
    private static class UpdateSelectedAfflictionOverride
    {
        private static bool Prefix(Panel_Affliction __instance, Affliction affliction)
        {
            __instance.m_Label.text = Affliction.LocalizedNameFromAfflictionType(affliction.m_AfflictionType, affliction.m_Id);
            __instance.m_LabelCause.text = affliction.m_Cause;
            __instance.m_LabelLocation.text = Panel_Affliction.LocalizedNameFromAfflictionLocation(affliction.m_Location);
            if (__instance.m_AfflictionButtonColorReferences)
            {
                Color colorBasedOnAffliction = __instance.m_AfflictionButtonColorReferences.GetColorBasedOnAffliction(affliction.m_AfflictionType, true);
                __instance.m_LabelCause.color = colorBasedOnAffliction;
                __instance.m_LabelLocation.color = colorBasedOnAffliction;
            }
            
            return false;
        }
    }
    
    [HarmonyPatch(typeof(Panel_Affliction), nameof(Panel_Affliction.UpdateSelectedAffliction), typeof(int))]
    private static class UpdateSelectedAfflictionOverrideInt
    {
        private static bool Prefix(Panel_Affliction __instance, int selectedAfflictionIndex)
        {
            int vanillaAfflictionCount = __instance.m_Afflictions?.Count ?? 0;
            int totalAfflictions = vanillaAfflictionCount + GlobalFields.panelAfflictionList.Count;

            if (selectedAfflictionIndex >= 0 && selectedAfflictionIndex < totalAfflictions)
            {
                if (selectedAfflictionIndex < vanillaAfflictionCount)
                    __instance.UpdateSelectedAffliction(__instance.m_Afflictions?[selectedAfflictionIndex]);
                else
                    UpdateSelectedCustomAffliction(GlobalFields.panelAfflictionList[selectedAfflictionIndex - vanillaAfflictionCount], __instance);
            }
            
            return false;
        }

        private static void UpdateSelectedCustomAffliction(CustomAffliction customAffliction, Panel_Affliction panelAffliction)
        {
            panelAffliction.m_Label.text = customAffliction.m_Name;
            panelAffliction.m_LabelCause.text = customAffliction.m_CauseText;
            panelAffliction.m_LabelLocation.text = Panel_Affliction.LocalizedNameFromAfflictionLocation(customAffliction.m_Location);

            if (!panelAffliction.m_AfflictionButtonColorReferences) return;
            Color colorBasedOnAffliction = AfflictionManager.GetAfflictionColour(customAffliction.GetAfflictionType());
            panelAffliction.m_LabelCause.color = colorBasedOnAffliction;
            panelAffliction.m_LabelLocation.color = colorBasedOnAffliction;
        }
    }
}