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
    private static class RefreshScrollListOverride
    {
        private static bool Prefix(Panel_FirstAid __instance)
        {
            bool hasSelectedButton = __instance.m_SelectedAffButton != null;
            AfflictionType selectedType = hasSelectedButton ? __instance.m_SelectedAffButton.m_AfflictionType : AfflictionType.BloodLoss;
            AfflictionBodyArea selectedArea = hasSelectedButton ? __instance.m_SelectedAffButton.m_AfflictionLocation : AfflictionBodyArea.ArmLeft;

            __instance.m_ScrollListEffects.CleanUp();
            __instance.ClearAfflictionsAtLocationArray();
            Panel_Affliction.GetAllAfflictions(__instance.m_ScrollListAfflictions);
            
            foreach (var uiSprite in __instance.m_BodyIconList)
            {
                uiSprite.gameObject.SetActive(false);
            }
            
            List<CustomAffliction> customAfflictions = AfflictionManager.GetAfflictionManagerInstance().m_Afflictions;
            int totalAfflictions = __instance.m_ScrollListAfflictions.Count + customAfflictions.Count;

            __instance.m_ScrollListEffects.CreateList(totalAfflictions);
            AfflictionType lastAfflictionType = AfflictionType.BloodLoss;
            int count = 0;
            
            for (int i = 0; i < totalAfflictions; i++)
            {
                AfflictionButton component = __instance.m_ScrollListEffects.m_ScrollObjects[i].transform.GetChild(0).GetComponent<AfflictionButton>();
                if (component == null) continue;

                if (i < __instance.m_ScrollListAfflictions.Count)
                {
                    Affliction affliction = __instance.m_ScrollListAfflictions[i];
                    ProcessAffliction(__instance, affliction, component, ref lastAfflictionType, ref count, hasSelectedButton, selectedType, selectedArea);
                }
                else
                {
                    CustomAffliction customAffliction = customAfflictions[i - __instance.m_ScrollListAfflictions.Count];
                    ProcessCustomAffliction(__instance, customAffliction, component, ref lastAfflictionType, ref count, hasSelectedButton, selectedType, selectedArea);
                }
            }

            return false;
        }

        private static void ProcessAffliction(Panel_FirstAid instance, Affliction affliction, AfflictionButton component, ref AfflictionType lastAfflictionType, ref int count, bool hasSelectedButton, AfflictionType selectedType, AfflictionBodyArea selectedArea)
        {
            int location = (int)affliction.m_Location;
            instance.AddAfflictionAtLocation(location, affliction);
            instance.m_BodyIconList[location].gameObject.SetActive(true);

            if (affliction.m_AfflictionType != lastAfflictionType)
            {
                count = 0;
            }

            string text = Affliction.LocalizedNameFromAfflictionType(affliction.m_AfflictionType, count);
            string text2 = Affliction.SpriteNameFromAfflictionType(affliction.m_AfflictionType);
            component.SetCauseAndEffect(affliction.m_Cause, affliction.m_AfflictionType, affliction.m_Location, count, text, text2);
            count++;
            lastAfflictionType = affliction.m_AfflictionType;

            bool isSelected = hasSelectedButton && affliction.m_AfflictionType == selectedType && affliction.m_Location == selectedArea;
            component.SetSelected(isSelected);
            instance.UpdateBodyIconColors(component, isSelected, location);
        }
        
        private static void ProcessCustomAffliction(Panel_FirstAid instance, CustomAffliction customAffliction, AfflictionButton component, ref AfflictionType lastAfflictionType, ref int count, bool hasSelectedButton, AfflictionType selectedType, AfflictionBodyArea selectedArea)
        {
            int location = (int)customAffliction.m_Location;
            // Rather than patching 'RefreshPaperDoll' in the above, we should create a custom method to add it .
            // AddCustomAfflictionAtLocation(location, customAffliction);
            instance.m_BodyIconList[location].gameObject.SetActive(true);

            AfflictionType afflictionType = AfflictionType.Generic;
            if (afflictionType != lastAfflictionType)
            {
                count = 0;
            }

            string text = customAffliction.m_AfflictionKey;
            string text2 = customAffliction.GetSpriteName();
            component.SetCauseAndEffect(customAffliction.m_Cause, afflictionType, customAffliction.m_Location, count, text, text2);
            count++;
            lastAfflictionType = afflictionType;

            bool isSelected = hasSelectedButton && afflictionType == selectedType && customAffliction.m_Location == selectedArea;
            component.SetSelected(isSelected);
            instance.UpdateBodyIconColors(component, isSelected, location);
        }
    }
    
    [HarmonyPatch(typeof(Panel_FirstAid), nameof(Panel_FirstAid.RefreshRightPage))]
    private static class RefreshRightPagePatch
    {
        private static void Postfix(Panel_FirstAid __instance) { }
    }
}