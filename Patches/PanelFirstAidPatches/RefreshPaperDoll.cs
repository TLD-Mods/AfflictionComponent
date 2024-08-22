using AfflictionComponent.Components;

namespace AfflictionComponent.Patches.PanelFirstAidPatches;

internal static class RefreshPaperDoll
{
    [HarmonyPatch(typeof(Panel_FirstAid), nameof(Panel_FirstAid.RefreshPaperDoll))]
    private static class RefreshPaperDollCustomAffliction
    {
        private static void Postfix(Panel_FirstAid __instance)
        {
            var afflictionManager = AfflictionManager.GetAfflictionManagerInstance();
            if (afflictionManager == null || afflictionManager.m_Afflictions.Count == 0) return;

            var panelClothing = InterfaceManager.GetPanel<Panel_Clothing>();
            
            var flag = false;
            for (var i = 0; i < afflictionManager.m_Afflictions.Count; i++)
            {
                if (!afflictionManager.GetAfflictionByIndex(i).HasBuff())
                {
                    flag = true;
                }
            }
            
            switch (flag)
            {
                case true:
                {
                    Utils.SetActive(__instance.m_PaperDollMale, PlayerManager.m_VoicePersona == VoicePersona.Male);
                    Utils.SetActive(__instance.m_PaperDollFemale, PlayerManager.m_VoicePersona == VoicePersona.Female);
                
                    if (!panelClothing.IsEnabled()) return;
                    panelClothing.Enable(false);
                    break;
                }
                case false:
                {
                    Utils.SetActive(__instance.m_PaperDollMale, false);
                    Utils.SetActive(__instance.m_PaperDollFemale, false);
                
                    if (panelClothing.IsEnabled()) return;
                    panelClothing.Enable(true);
                    panelClothing.ShowPaperDollOnly();
                    break;
                }
            }
        }
    }
}