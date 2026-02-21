using AfflictionComponent.Components;

namespace AfflictionComponent.Patches.AfflictionButtonPatches;

internal static class SetColor
{
    [HarmonyPatch(typeof(AfflictionButton), nameof(AfflictionButton.SetColor))]
    private static class HasCustomRiskAffliction
    {
        private static void Postfix(AfflictionButton __instance, bool isSelected)
        {
            if (__instance.m_AfflictionType != AfflictionType.Generic) return;
            
            if (!AfflictionManager.GetAfflictionManagerInstance().TryGetAfflictionByIndex(__instance.GetAfflictionIndex(), out var customAffliction) || customAffliction == null) return;

            var colorBasedOnCustomAffliction = AfflictionManager.GetAfflictionColour(customAffliction.GetAfflictionType());
            __instance.m_SpriteEffect.color = colorBasedOnCustomAffliction;
            __instance.m_LabelEffect.color = colorBasedOnCustomAffliction;
            __instance.m_LabelCause.color = isSelected ? __instance.m_CauseColorHover : __instance.m_CauseColor;
        }
    }
}
