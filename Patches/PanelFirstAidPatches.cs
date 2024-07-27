using AfflictionComponent.Components;

namespace AfflictionComponent.Patches;

internal static class PanelFirstAidPatches
{
    [HarmonyPatch(nameof(Panel_FirstAid), nameof(Panel_FirstAid.HasRiskAffliction))]
    private static class HasCustomRiskAffliction
    {
        private static void Postfix(Panel_FirstAid __instance, ref bool __result)
        {
            var customAfflictions = AfflictionManager.GetAfflictionManagerInstance().m_Afflictions;
            foreach (var customAffliction in customAfflictions)
            {
                __result = customAffliction.HasAfflictionRisk();
            }
        }
    }
    
    [HarmonyPatch(nameof(Panel_FirstAid), nameof(Panel_FirstAid.HasBadAffliction))]
    private static class HasCustomBadAffliction
    {
        private static void Postfix(Panel_FirstAid __instance, ref bool __result)
        {
            var customAfflictions = AfflictionManager.GetAfflictionManagerInstance().m_Afflictions;
            foreach (var customAffliction in customAfflictions)
            {
                __result = !customAffliction.m_Buff;
            }
        }
    }
    
    // This hasn't been tested with buffs, risks, multiple afflictions at once or with vanilla afflictions, so it'll probably hide the paperdoll if there is a buff lol
    [HarmonyPatch(typeof(Panel_FirstAid), nameof(Panel_FirstAid.RefreshPaperDoll))]
    private static class RefreshPaperDollCustomAffliction
    {
        private static void Postfix(Panel_FirstAid __instance)
        {
            var afflictionManager = AfflictionManager.GetAfflictionManagerInstance();
            if (afflictionManager == null || afflictionManager.m_Afflictions.Count == 0) return;

            Utils.SetActive(__instance.m_PaperDollMale, PlayerManager.m_VoicePersona == VoicePersona.Male);
            Utils.SetActive(__instance.m_PaperDollFemale, PlayerManager.m_VoicePersona == VoicePersona.Female);

            if (InterfaceManager.GetPanel<Panel_Clothing>().IsEnabled())
            {
                InterfaceManager.GetPanel<Panel_Clothing>().Enable(false);
            }

            foreach (var bodyArea in Enum.GetValues(typeof(AfflictionBodyArea)).Cast<AfflictionBodyArea>())
            {
                List<CustomAffliction> afflictions = afflictionManager.GetAfflictionsByBodyArea(bodyArea);
                if (afflictions.Count == 0) return;
            
                string bodyPartName = bodyArea.ToString().ToLower();
                int iconIndex = Array.FindIndex<UISprite>(__instance.m_BodyIconList, icon => icon.name.ToLower().Contains(bodyPartName));
                if (iconIndex == -1) return;
            
                UISprite bodyIcon = __instance.m_BodyIconList[iconIndex];
                bodyIcon.gameObject.SetActive(true);
            
                var affliction = afflictions[0];
                bodyIcon.spriteName = __instance.m_BodyIconSpriteNameAffliction;
                bodyIcon.color = AfflictionManager.GetAfflictionColour(affliction.GetAfflictionType());
            
                foreach (var childSprite in bodyIcon.gameObject.GetComponentsInChildren<UISprite>())
                {
                    var colorWithAlpha = __instance.m_ColorAffliction;
                    colorWithAlpha.a = childSprite.color.a;
                    childSprite.color = colorWithAlpha;
                }
            }
        }
    }
    
    [HarmonyPatch(typeof(Panel_FirstAid), nameof(Panel_FirstAid.RefreshScrollList))]
    private static class RefreshScrollListPatch
    {
        private static void Postfix(Panel_FirstAid __instance) { }
    }
    
    [HarmonyPatch(typeof(Panel_FirstAid), nameof(Panel_FirstAid.RefreshRightPage))]
    private static class RefreshRightPagePatch
    {
        private static void Postfix(Panel_FirstAid __instance) { }
    }
}