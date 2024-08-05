﻿using AfflictionComponent.Components;

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
}