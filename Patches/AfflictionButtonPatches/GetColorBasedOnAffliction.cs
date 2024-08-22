using AfflictionComponent.Components;

namespace AfflictionComponent.Patches.AfflictionButtonPatches;

internal static class GetColorBasedOnAffliction
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
}