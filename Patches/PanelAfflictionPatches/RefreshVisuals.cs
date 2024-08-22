namespace AfflictionComponent.Patches.PanelAfflictionPatches;

internal static class RefreshVisuals
{
    // This part: ---__instance.m_MouseButtonTreatWounds.SetLocID(__instance.m_TreatWoundsLocalizationId);---
    // Probably don't account for custom afflictions - we will need to investigate that more.
    [HarmonyPatch(typeof(Panel_Affliction), nameof(Panel_Affliction.RefreshVisuals))]
    private static class RefreshVisualOverride
    {
        private static bool Prefix() => false;

        private static void Postfix(Panel_Affliction __instance)
        {

            int tweenTargetIndex = __instance.m_ScrollList.GetTweenTargetIndex();
            for (int i = 0; i < __instance.m_CoverflowAfflictions.Count; i++)
            {
                bool isSelected = tweenTargetIndex == i;
                __instance.UpdateCoverFlowColor(i, isSelected);
            }
            __instance.UpdateSelectedAffliction(tweenTargetIndex);
            Utils.SetActive(__instance.m_ButtonLeft, __instance.m_ScrollList.GetTweenTargetIndex() > 0);
            Utils.SetActive(__instance.m_ButtonLeftGamepad, __instance.m_ScrollList.GetTweenTargetIndex() > 0);
            Utils.SetActive(__instance.m_ButtonRight, __instance.m_ScrollList.GetTweenTargetIndex() < __instance.m_ScrollList.m_ScrollObjects.Count - 1);
            Utils.SetActive(__instance.m_ButtonRightGamepad, __instance.m_ScrollList.GetTweenTargetIndex() < __instance.m_ScrollList.m_ScrollObjects.Count - 1);
            __instance.m_MouseButtonTreatWounds.SetLocID(__instance.m_TreatWoundsLocalizationId);
            __instance.UpdateButtonLegend();
        }
    }
}