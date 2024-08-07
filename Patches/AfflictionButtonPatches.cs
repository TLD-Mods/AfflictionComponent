using AfflictionComponent.Components;
using AfflictionComponent.Interfaces;

namespace AfflictionComponent.Patches;

internal static class AfflictionButtonPatches
{
    [HarmonyPatch(nameof(AfflictionButton), nameof(AfflictionButton.SetColor))]
    private static class HasCustomRiskAffliction
    {
        private static void Postfix(AfflictionButton __instance, bool isSelected)
        {
            if (__instance.m_AfflictionType == AfflictionType.Generic)
            {
                Color colorBasedOnCustomAffliction = AfflictionManager.GetAfflictionColour(AfflictionManager.GetAfflictionManagerInstance().GetAfflictionByIndex(__instance.m_Index).GetAfflictionType());
                __instance.m_SpriteEffect.color = colorBasedOnCustomAffliction;
                __instance.m_LabelEffect.color = colorBasedOnCustomAffliction;
                __instance.m_LabelCause.color = isSelected ? __instance.m_CauseColorHover : __instance.m_CauseColor;
            }
        }
    }
    
    [HarmonyPatch(nameof(AfflictionButton), nameof(AfflictionButton.UpdateFillBar))]
    private static class UpdateFillBarCustomRiskAffliction
    {
        private static void Postfix(AfflictionButton __instance)
        {
            if (__instance.m_AfflictionType == AfflictionType.Generic && AfflictionManager.GetAfflictionManagerInstance().GetAfflictionByIndex(__instance.m_Index) is IRiskPercentage riskPercentage)
            {
                var num = riskPercentage.GetRiskValue() / 100f; // The 100f slows it down, otherwise right now it's way to quick.
                Utils.SetActive(__instance.m_AnimatorAfflictionBar.gameObject, num > 0f);
                __instance.m_FillSpriteAfflictionBar.fillAmount = Mathf.Lerp(__instance.m_FillSpriteOffset, 1f - __instance.m_FillSpriteOffset, num);
                __instance.m_SizeModifierAfflictionBar.localScale = new Vector3(num, 1f, 1f);
            }
        }
    }
}