using AfflictionComponent.Components;
using AfflictionComponent.Interfaces;

namespace AfflictionComponent.Patches;

internal static class AfflictionButtonPatches
{
    [HarmonyPatch(nameof(AfflictionButton), nameof(AfflictionButton.GetColorBasedOnAffliction))]
    private static class GetColorBasedOnCustomAffliction
    {
        private static void Postfix(AfflictionButton __instance, AfflictionType m_AfflictionType, bool isHovering, ref Color __result)
        {
            if (m_AfflictionType != AfflictionType.Generic) return;

            var customAffliction = AfflictionManager.GetAfflictionManagerInstance().GetAfflictionByIndex(__instance.m_Index);
            var color = Color.white;
            
            if (isHovering)
                if (customAffliction.HasRisk())
                    color = __instance.m_RiskColorHover;
                else
                    color = customAffliction.HasBuff() ? __instance.m_BeneficialColorHover : __instance.m_NegativeColorHover;
            else if (customAffliction.HasRisk())
                color = __instance.m_RiskColor;
            else
                color = customAffliction.HasBuff() ? __instance.m_BeneficialColor : __instance.m_NegativeColor;
            
            __result = color;
        }
    }
    
    [HarmonyPatch(nameof(AfflictionButton), nameof(AfflictionButton.SetColor))]
    private static class HasCustomRiskAffliction
    {
        private static void Postfix(AfflictionButton __instance, bool isSelected)
        {
            if (__instance.m_AfflictionType != AfflictionType.Generic) return;
            
            var colorBasedOnCustomAffliction = AfflictionManager.GetAfflictionColour(AfflictionManager.GetAfflictionManagerInstance().GetAfflictionByIndex(__instance.GetAfflictionIndex()).GetAfflictionType());
            __instance.m_SpriteEffect.color = colorBasedOnCustomAffliction;
            __instance.m_LabelEffect.color = colorBasedOnCustomAffliction;
            __instance.m_LabelCause.color = isSelected ? __instance.m_CauseColorHover : __instance.m_CauseColor;
        }
    }
    
    // Index was out of range error occurs in this patch when the percentage reaches 100% and is removed from the m_Afflictions list in AfflictionManager.
    // The cause of the problem with the directional indicator seems to be the animator, I don't know why it's treating our custom affliction differently though.
    [HarmonyPatch(nameof(AfflictionButton), nameof(AfflictionButton.UpdateFillBar))]
    private static class UpdateFillBarCustomRiskAffliction
    {
        private static void Postfix(AfflictionButton __instance)
        {
            if (__instance.m_AfflictionType != AfflictionType.Generic) return;

            var customAffliction = AfflictionManager.GetAfflictionManagerInstance().GetAfflictionByIndex(__instance.GetAfflictionIndex());
            
            var riskPercentage = AfflictionManager.TryGetInterface<IRiskPercentage>(customAffliction);
            if (riskPercentage != null && riskPercentage.Risk)
            {
                var num = riskPercentage.GetRiskValue() / 100f; // The 100f slows it down, otherwise right now it's way too quick.
                Utils.SetActive(__instance.m_AnimatorAfflictionBar.gameObject, num > 0f);
                __instance.m_FillSpriteAfflictionBar.fillAmount = Mathf.Lerp(__instance.m_FillSpriteOffset, 1f - __instance.m_FillSpriteOffset, num);
                __instance.m_SizeModifierAfflictionBar.localScale = new Vector3(num, 1f, 1f);
            }

            
            if (customAffliction.HasBuff())
            {
                var iDuration = AfflictionManager.TryGetInterface<IDuration>(customAffliction);
                if (iDuration != null)
                {
                    var num2 = customAffliction.InterfaceDuration.GetTimeRemaining() / 30f; // 30 seems to be the magic number! But it's still slightly off a tad.
                    Utils.SetActive(__instance.m_AnimatorBuffBar.gameObject, num2 > 0f);
                    __instance.m_FillSpriteBuffBar.fillAmount = Mathf.Lerp(__instance.m_FillSpriteOffset, 1f - __instance.m_FillSpriteOffset, num2);
                    __instance.m_SizeModifierBuffBar.localScale = new Vector3(num2, 1f, 1f);
                }
            }
        }
    }
    
    // Testing for displaying custom icons
    [HarmonyPatch(nameof(AfflictionButton), nameof(AfflictionButton.SetCauseAndEffect))]
    private static class Test1
    {
        private static void Postfix(AfflictionButton __instance, string causeStr, AfflictionType affType, AfflictionBodyArea location, int index, string effectName, string spriteName)
        {
            if (__instance.m_AfflictionType != AfflictionType.Generic) return;
            
            var customAffliction = AfflictionManager.GetAfflictionManagerInstance().GetAfflictionByIndex(__instance.GetAfflictionIndex());
            if (!customAffliction.m_CustomSprite) return;
            
            __instance.m_SpriteEffect.atlas = Mod.customAtlas;
        }
    }
}