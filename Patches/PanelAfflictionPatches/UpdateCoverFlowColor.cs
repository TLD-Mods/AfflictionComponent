using AfflictionComponent.Components;

namespace AfflictionComponent.Patches.PanelAfflictionPatches;

internal static class UpdateCoverFlowColor
{
    [HarmonyPatch(typeof(Panel_Affliction), nameof(Panel_Affliction.UpdateCoverFlowColor))]
    private static class UpdateCoverFlowCoverOverride
    {
        private static bool Prefix() => false;

        private static void Postfix(Panel_Affliction __instance, ref int index, ref bool isSelected)
        {
            AfflictionManager am = AfflictionManager.GetAfflictionManagerInstance();
            Color colorBasedOnAffliction;

            if (index >= __instance.m_Afflictions.Count)
            {
                int customAfflictionIndex = index - __instance.m_Afflictions.Count;
                if (customAfflictionIndex < 0 || customAfflictionIndex >= am.m_Afflictions.Count) return;
                colorBasedOnAffliction = AfflictionManager.GetAfflictionColour(GlobalFields.panelAfflictionList[customAfflictionIndex].GetAfflictionType());
            }
            else
            {
                colorBasedOnAffliction = __instance.m_AfflictionButtonColorReferences.GetColorBasedOnAffliction(__instance.m_Afflictions[index].m_AfflictionType, isSelected);
            }

            __instance.m_CoverflowAfflictions[index].m_SpriteEffect.color = colorBasedOnAffliction;
        }
    }
}