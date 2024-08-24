using AfflictionComponent.Components;

namespace AfflictionComponent.Patches.PanelHUDPatches;

internal static class DoShowBuffNotification
{
    [HarmonyPatch(nameof(Panel_HUD), nameof(Panel_HUD.DoShowBuffNotification))]
    private static class SwapBuffSpriteAtlas
    {
        private static void Postfix(Panel_HUD __instance, Panel_HUD.BuffNotification buffNotification)
        {
            var customAffliction = AfflictionManager.GetAfflictionManagerInstance().m_Afflictions.FirstOrDefault(a => buffNotification.m_BuffNameLocID == a.m_Name && a.m_CustomSprite);
            if (customAffliction != null)
            {
                for (var i = 0; i < Mod.allCustomAtlas.transform.childCount; i++)
                {
                    if ($"CustomAtlas{customAffliction.m_SpriteName}(Clone)" == Mod.allCustomAtlas.transform.GetChild(i).name)
                    {
                        __instance.m_BuffSprite.atlas = Mod.allCustomAtlas.transform.GetChild(i).GetComponent<UIAtlas>();
                        break;
                    }
                }

                return;
            }
            
            __instance.m_BuffSprite.atlas = __instance.m_StruggleBar.atlas;
        }
    }
}