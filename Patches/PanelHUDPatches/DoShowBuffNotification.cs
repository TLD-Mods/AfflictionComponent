using AfflictionComponent.Components;

namespace AfflictionComponent.Patches.PanelHUDPatches;

internal static class DoShowBuffNotification
{
    [HarmonyPatch(nameof(Panel_HUD), nameof(Panel_HUD.DoShowBuffNotification))]
    private static class SwapBuffSpriteAtlas
    {
        private static void Postfix(Panel_HUD __instance, Panel_HUD.BuffNotification buffNotification)
        {
            if (AfflictionManager.GetAfflictionManagerInstance().m_Afflictions.Any(customAffliction => buffNotification.m_BuffNameLocID == customAffliction.m_Name && customAffliction.m_CustomSprite))
            {
                __instance.m_BuffSprite.atlas = Mod.customAtlas;
                return;
            }

            __instance.m_BuffSprite.atlas = __instance.m_StruggleBar.atlas;
        }
    }
}