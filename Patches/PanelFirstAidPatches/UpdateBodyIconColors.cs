using AfflictionComponent.Components;

namespace AfflictionComponent.Patches.PanelFirstAidPatches;

internal static class UpdateBodyIconColors
{
    [HarmonyPatch(nameof(Panel_FirstAid), nameof(Panel_FirstAid.UpdateBodyIconColors))]
    private static class OverrideUpdateBodyIconColors
    {
        private static void Postfix(Panel_FirstAid __instance, AfflictionButton afflictionButton, bool isButtonSelected, int bodyIconIndex)
        {
            if (afflictionButton.m_AfflictionType != AfflictionType.Generic) return;
            __instance.m_BodyIconList[bodyIconIndex].spriteName = AfflictionManager.GetAfflictionManagerInstance().GetAfflictionByIndex(afflictionButton.m_Index).HasBuff() ? __instance.m_BodyIconSpriteNameBuff : __instance.m_BodyIconSpriteNameAffliction;
        }
    }
}