namespace AfflictionComponent.Patches;

internal static class PanelHUDPatches
{
    [HarmonyPatch(nameof(Panel_HUD), nameof(Panel_HUD.DoShowBuffNotification))]
    private static class Test
    {
        private static void Postfix(Panel_HUD __instance, Panel_HUD.BuffNotification buffNotification)
        {
            // Need to add conditions to see if it is a custom sprite, or a custom affliction. If neither, then don't change the atlas.
            __instance.m_BuffSprite.atlas = Mod.customAtlas;
        }
    }
}