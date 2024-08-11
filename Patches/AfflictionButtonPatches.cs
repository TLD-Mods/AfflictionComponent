using AfflictionComponent.Components;
using AfflictionComponent.Interfaces.Risk;

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
                Color colorBasedOnCustomAffliction = AfflictionManager.GetAfflictionColour(AfflictionManager.GetAfflictionManagerInstance().GetAfflictionByIndex(__instance.GetAfflictionIndex()).GetAfflictionType());
                __instance.m_SpriteEffect.color = colorBasedOnCustomAffliction;
                __instance.m_LabelEffect.color = colorBasedOnCustomAffliction;
                __instance.m_LabelCause.color = isSelected ? __instance.m_CauseColorHover : __instance.m_CauseColor;
            }
        }
    }
    
    // Index was out of range error occurs in this patch when the percentage reaches 100% and is removed from the m_Afflictions list in AfflictionManager.
    // The cause of the problem with the directional indicator seems to be the animator, I don't know why it's treating our custom affliction differently though.
    [HarmonyPatch(nameof(AfflictionButton), nameof(AfflictionButton.UpdateFillBar))]
    private static class UpdateFillBarCustomRiskAffliction
    {
        private static void Postfix(AfflictionButton __instance)
        {
            if (__instance.m_AfflictionType != AfflictionType.Generic || AfflictionManager.GetAfflictionManagerInstance().GetAfflictionByIndex(__instance.GetAfflictionIndex()) is not IRiskPercentage riskPercentage) return;
            
            var num = riskPercentage.GetRiskValue() / 100f; // The 100f slows it down, otherwise right now it's way too quick.
            Utils.SetActive(__instance.m_AnimatorAfflictionBar.gameObject, num > 0f);
            __instance.m_FillSpriteAfflictionBar.fillAmount = Mathf.Lerp(__instance.m_FillSpriteOffset, 1f - __instance.m_FillSpriteOffset, num);
            __instance.m_SizeModifierAfflictionBar.localScale = new Vector3(num, 1f, 1f);
        }
    }
}