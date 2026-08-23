using AfflictionComponent.Components;

namespace AfflictionComponent.Patches.PanelHUDPatches;

internal static class DoShowBuffNotification
{
    [HarmonyPatch(typeof(Panel_HUD), nameof(Panel_HUD.DoShowBuffNotification))]
    private static class SwapBuffSpriteAtlas
    {
        private static void Postfix(Panel_HUD __instance, Panel_HUD.BuffNotification buffNotification)
        {
            for (var i = 0; i < Mod.allCustomAtlas.transform.childCount; i++)
            {
                if ($"CustomAtlas{buffNotification.m_BuffSpriteName}(Clone)" != Mod.allCustomAtlas.transform.GetChild(i).name) continue;

                __instance.m_BuffSprite.atlas = Mod.allCustomAtlas.transform.GetChild(i).GetComponent<UIAtlas>();
                return;
            }

            __instance.m_BuffSprite.atlas = __instance.m_StruggleBar.atlas;
        }
    }
}