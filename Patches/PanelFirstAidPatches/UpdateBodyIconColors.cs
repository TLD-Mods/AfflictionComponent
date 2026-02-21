using AfflictionComponent.Components;

namespace AfflictionComponent.Patches.PanelFirstAidPatches;

internal static class UpdateBodyIconColors
{
    [HarmonyPatch(typeof(Panel_FirstAid), nameof(Panel_FirstAid.UpdateBodyIconColors))]
    private static class OverrideUpdateBodyIconColors
    {
        private static void Postfix(Panel_FirstAid __instance, AfflictionButton afflictionButton, bool isButtonSelected, int bodyIconIndex)
        {
            if (afflictionButton.m_AfflictionType != AfflictionType.Generic) return;
            if (bodyIconIndex < 0 || bodyIconIndex >= __instance.m_BodyIconList.Length) return;
            if (!AfflictionManager.GetAfflictionManagerInstance().TryGetAfflictionByIndex(afflictionButton.m_Index, out var customAffliction) || customAffliction == null) return;

            __instance.m_BodyIconList[bodyIconIndex].spriteName = customAffliction.HasBuff() ? __instance.m_BodyIconSpriteNameBuff : __instance.m_BodyIconSpriteNameAffliction;
        }
    }
}
