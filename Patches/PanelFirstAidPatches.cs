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
                __result = customAffliction.HasAffliction() && !customAffliction.m_Buff;
            }
        }
    }
    
    // This is the code I was testing to override the paperdoll if a custom affliction is applied.
    // This will obviously need to be updated in order to work with what we currently have.
    [HarmonyPatch(typeof(Panel_FirstAid), nameof(Panel_FirstAid.RefreshPaperDoll))]
    private static class RefreshPaperDollCustomAffliction
    {
        private static void Postfix(Panel_FirstAid __instance)
        {
            /*Utils.SetActive(__instance.m_PaperDollMale, PlayerManager.m_VoicePersona == VoicePersona.Male);
            Utils.SetActive(__instance.m_PaperDollFemale, PlayerManager.m_VoicePersona == VoicePersona.Female);

            if (InterfaceManager.GetPanel<Panel_Clothing>().IsEnabled())
            {
                InterfaceManager.GetPanel<Panel_Clothing>().Enable(false);
            }
            
            var chestIconIndex = -1;
            for (var i = 0; i < __instance.m_BodyIconList.Length; i++)
            {
                if (!__instance.m_BodyIconList[i].name.ToLower().Contains("chest")) continue;
                chestIconIndex = i;
                break;
            }

            if (chestIconIndex == -1) return;
            
            __instance.m_BodyIconList[chestIconIndex].gameObject.SetActive(true);
            __instance.m_BodyIconList[chestIconIndex].color = new Color(0.7352941f, 0.5191095f, 0.2324827f, 1f);
            __instance.m_BodyIconList[chestIconIndex].spriteName = __instance.m_BodyIconSpriteNameAffliction;
            
            var chestIconObject = __instance.m_BodyIconList[chestIconIndex].gameObject;
            Component[] childComponents = chestIconObject.GetComponentsInChildren<Component>();
            foreach (var component in childComponents)
            {
                if (component is not UISprite childSprite) continue;
                var colorWithAlpha = __instance.m_ColorAffliction;
                colorWithAlpha.a = childSprite.color.a;
                childSprite.color = colorWithAlpha;
            }*/
        }
    }
    
    [HarmonyPatch(typeof(Panel_FirstAid), nameof(Panel_FirstAid.RefreshScrollList))]
    private static class AddCustomAfflictionsToScrollList
    {
        private static void Postfix() { }
    }
}