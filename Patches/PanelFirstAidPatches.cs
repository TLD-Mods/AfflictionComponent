using AfflictionComponent.Components;
using AfflictionComponent.Interfaces;
using Il2CppTLD.IntBackedUnit;
using AfflictionComponent.Utilities;

namespace AfflictionComponent.Patches;

internal static class PanelFirstAidPatches
{
    [HarmonyPatch(nameof(Panel_FirstAid), nameof(Panel_FirstAid.HasBadAffliction))]
    private static class HasCustomBadAffliction
    {
        private static void Postfix(Panel_FirstAid __instance, ref bool __result)
        {
            if (__result) return;
            
            var customAfflictions = AfflictionManager.GetAfflictionManagerInstance().m_Afflictions;
            __result = customAfflictions.Any(affliction => !affliction.HasBuff() && !affliction.HasRisk());
        }
    }
    
    [HarmonyPatch(nameof(Panel_FirstAid), nameof(Panel_FirstAid.HasRiskAffliction))]
    private static class HasCustomRiskAffliction
    {
        private static void Postfix(Panel_FirstAid __instance, ref bool __result)
        {
            if (__result) return;
            
            var customAfflictions = AfflictionManager.GetAfflictionManagerInstance().m_Afflictions;
            if (!customAfflictions.Any(affliction => !affliction.HasBuff() && !affliction.HasRisk()))
            {
                __result = customAfflictions.Any(affliction => affliction.HasRisk());
            }
        }
    }
    
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
                var afflictionButton = __instance.m_ScrollListEffects.m_ScrollObjects[i].transform.GetChild(0).GetComponent<AfflictionButton>();
                if (afflictionButton == null) continue;

                if (i < __instance.m_ScrollListAfflictions.Count)
                {
                    Affliction affliction = __instance.m_ScrollListAfflictions[i];
                    ProcessAffliction(__instance, affliction, afflictionButton, ref lastAfflictionType, ref count, hasSelectedButton, selectedType, selectedArea);
                }
                else
                {
                    CustomAffliction customAffliction = customAfflictions[i - __instance.m_ScrollListAfflictions.Count];
                    ProcessCustomAffliction(__instance, customAffliction, afflictionButton, ref lastAfflictionType, ref count, hasSelectedButton, selectedType, selectedArea);
                }
            }

            return false;
        }

        private static void ProcessAffliction(Panel_FirstAid instance, Affliction affliction, AfflictionButton component, ref AfflictionType lastAfflictionType, ref int count, bool hasSelectedButton, AfflictionType selectedType, AfflictionBodyArea selectedArea)
        {
            int location = (int)affliction.m_Location;
            instance.AddAfflictionAtLocation(location, affliction);
            instance.m_BodyIconList[location].gameObject.SetActive(true);

            if (affliction.m_AfflictionType != lastAfflictionType) count = 0;

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
            AddCustomAfflictionAtLocation(instance, location, customAffliction);
            instance.m_BodyIconList[location].gameObject.SetActive(true);

            AfflictionType afflictionType = AfflictionType.Generic;
            if (afflictionType != lastAfflictionType) count = 0;

            string text = customAffliction.m_Name;
            string text2 = customAffliction.GetSpriteName();
            component.SetCauseAndEffect(customAffliction.m_CauseText, afflictionType, customAffliction.m_Location, count, text, text2);
            count++;
            lastAfflictionType = afflictionType;

            bool isSelected = hasSelectedButton && afflictionType == selectedType && customAffliction.m_Location == selectedArea;
            component.SetSelected(isSelected);
            instance.UpdateBodyIconColors(component, isSelected, location);
        }
        
        private static void AddCustomAfflictionAtLocation(Panel_FirstAid instance, int bodyIconIndex, CustomAffliction customAffliction)
        {
            Panel_FirstAid.AfflictionsAtLocation afflictionsAtLocation = instance.m_AfflictionsAtLocationArray[bodyIconIndex];
            if (afflictionsAtLocation == null)
            {
                afflictionsAtLocation = new Panel_FirstAid.AfflictionsAtLocation(customAffliction.m_Location);
                instance.m_AfflictionsAtLocationArray[bodyIconIndex] = afflictionsAtLocation;
            }
        }
    }

    [HarmonyPatch(typeof(Panel_FirstAid), nameof(Panel_FirstAid.RefreshRightPage))]
    private static class RefreshRightPagePatch
    {
        public static bool Prefix(Panel_FirstAid __instance)
        {
            if (!__instance.m_SelectedAffButton) return true;
            if (__instance.m_SelectedAffButton.m_AfflictionType != AfflictionType.FoodPoisioning && __instance.m_SelectedAffButton.m_AfflictionType != AfflictionType.Dysentery && __instance.m_SelectedAffButton.m_AfflictionType != AfflictionType.Generic) return true;

            return false;
        }
        
        private static void Postfix(Panel_FirstAid __instance)
        {
            if (!__instance.m_SelectedAffButton) return;
            if (__instance.m_SelectedAffButton.m_AfflictionType != AfflictionType.FoodPoisioning && __instance.m_SelectedAffButton.m_AfflictionType != AfflictionType.Dysentery && __instance.m_SelectedAffButton.m_AfflictionType != AfflictionType.Generic) return;

            // Generic UI crap, some of it we probably don't even need for this override.
            foreach (var firstAidKitButton in __instance.m_FakButtons)
            {
                firstAidKitButton.SetNeeded(needed: false);
            }
            
            __instance.m_SpecialTreatmentWindow.SetActive(false);
            __instance.m_BuffWindow.SetActive(false);

            // Disable treatment window & standard description because for some reason it doesn't do it on its own.
            __instance.m_ItemsNeededOnlyOneObj.SetActive(false);
            __instance.m_ItemsNeededMultipleObj.SetActive(false);
            __instance.m_LabelAfflictionDescription.text = string.Empty;

            // Disable rest requirement since we're not using this yet.
            __instance.m_ObjectRestRemaining.SetActive(false);

            if (__instance.m_ScrollListEffects.m_ScrollObjects.Count == 0)
            {
                NGUITools.SetActive(__instance.m_RightPageHealthyObject, state: true);
                __instance.HideRightPage();
                return;
            }
            
            NGUITools.SetActive(__instance.m_RightPageHealthyObject, state: false);
            
            if (__instance.m_SelectedAffButton == null)
            {
                __instance.HideRightPage();
                return;
            }
            
            NGUITools.SetActive(__instance.m_RightPageObject, state: true);
            for (int j = 0; j < __instance.m_BodyIconList.Length; j++)
            {
                __instance.m_BodyIconList[j].width = __instance.m_BodyIconWidthOriginal;
                __instance.m_BodyIconList[j].height = __instance.m_BodyIconHeightOriginal;
            }
            int num = -1;

            if(__instance.m_SelectedAffButton.m_AfflictionType != AfflictionType.Generic) __instance.m_LabelAfflictionName.text = Affliction.LocalizedNameFromAfflictionType(__instance.m_SelectedAffButton.m_AfflictionType, __instance.m_SelectedAffButton.GetAfflictionIndex());

            // Colour stuff.
            __instance.m_LabelAfflictionName.color = Affliction.IsBeneficial(__instance.m_SelectedAffButton.m_AfflictionType) ? InterfaceManager.m_FirstAidBuffColor : InterfaceManager.m_FirstAidRedColor;

            int num4 = 0; // THIS IS THE DURATION NUMBER
            int selectedAfflictionIndex = __instance.GetSelectedAfflictionIndex(); // This will most likely need to be modified.

            switch (__instance.m_SelectedAffButton.m_AfflictionType)
            {
                case AfflictionType.Dysentery:
                {
                    VanillaOverrides.DysenteryMethod(__instance, selectedAfflictionIndex, out num, out num4); // This runs the vanilla code, IA will patch this method and override it to call custom code.
                    break;
                }
                case AfflictionType.FoodPoisioning:
                {
                    VanillaOverrides.FoodPoisoningMethod(__instance, selectedAfflictionIndex, out num, out num4); // This runs the vanilla code, IA will patch this method and override it to call custom code.
                    break;
                }
                case AfflictionType.Generic: // Custom affliction.

                // This handles the out of index range error I was getting in the console.
                CustomAffliction affliction;
                try
                {
                    affliction = AfflictionManager.GetAfflictionManagerInstance().GetAfflictionByIndex(selectedAfflictionIndex);
                }
                catch (ArgumentOutOfRangeException e) 
                {
                    Mod.Logger.Log(e.Message, ComplexLogger.FlaggedLoggingLevel.Error);
                    return; 
                }
                
                __instance.m_LabelAfflictionName.text = affliction.m_Name;

                string[] remedySprites;
                int[] remedyNumRequired;
                bool[] remedyComplete;

                string[]? altRemedySprites;
                int[]? altRemedyNumRequired;
                bool[]? altRemedyComplete;

                __instance.m_LabelAfflictionName.color = AfflictionManager.GetAfflictionColour(affliction.GetAfflictionType());

                if (affliction.InterfaceRemedies != null && affliction.InterfaceRemedies.RemedyItems != null && affliction.InterfaceRemedies.RemedyItems.Length != 0)
                {
                    // I don't know if the UI will support more than 2 items.
                    remedySprites = new string[affliction.InterfaceRemedies.RemedyItems.Length];
                    remedyNumRequired = new int[affliction.InterfaceRemedies.RemedyItems.Length];
                    remedyComplete = new bool[affliction.InterfaceRemedies.RemedyItems.Length];

                    for (int i = 0; i < affliction.InterfaceRemedies.RemedyItems.Length; i++)
                    {
                        Tuple<string, int, int> e = affliction.InterfaceRemedies.RemedyItems[i];
                        remedySprites[i] = e.Item1;
                        remedyNumRequired[i] = e.Item2;
                        remedyComplete[i] = e.Item3 < 1;
                    }

                    if (affliction.InterfaceRemedies.AltRemedyItems != null && affliction.InterfaceRemedies.AltRemedyItems.Length != 0)
                    {
                        // I don't know if the UI will support more than 2 items.
                        altRemedySprites = new string[affliction.InterfaceRemedies.AltRemedyItems.Length];
                        altRemedyNumRequired = new int[affliction.InterfaceRemedies.AltRemedyItems.Length];
                        altRemedyComplete = new bool[affliction.InterfaceRemedies.AltRemedyItems.Length];

                        for (int i = 0; i < affliction.InterfaceRemedies.AltRemedyItems.Length; i++)
                        {
                            Tuple<string, int, int> e = affliction.InterfaceRemedies.AltRemedyItems[i];
                            altRemedySprites[i] = e.Item1;
                            altRemedyNumRequired[i] = e.Item2;
                            altRemedyComplete[i] = e.Item3 < 1;
                        }
                    }
                    else
                    {
                        altRemedySprites = null;
                        altRemedyNumRequired = null;
                        altRemedyComplete = null;
                    }
                    
                    __instance.m_LabelRiskDescription.text = string.Empty;
                    __instance.m_LabelAfflictionDescriptionNoRest.text = string.Empty;
                    __instance.m_LabelAfflictionDescription.text = affliction.m_Description;
                    __instance.SetItemsNeeded(remedySprites, remedyComplete, remedyNumRequired, altRemedySprites, altRemedyComplete, altRemedyNumRequired, ItemLiquidVolume.Zero, 0f, 0f);
                }
                else
                {
                    __instance.m_MultipleDosesObject.SetActive(false);
                    __instance.m_RightPageObject.SetActive(false);
                    
                    if (!string.IsNullOrEmpty(affliction.m_DescriptionNoHeal) && !affliction.HasBuff())
                    {
                        __instance.m_LabelAfflictionDescriptionNoRest.text = string.Empty;
                        __instance.m_LabelAfflictionDescription.text = string.Empty;
                        __instance.m_BuffWindow.SetActive(false);
                        
                        __instance.m_LabelSpecialTreatment.text = affliction.m_DescriptionNoHeal; // Is this what we want here?
                        __instance.m_LabelSpecialTreatmentDescription.text = affliction.m_Description;
                        __instance.m_SpecialTreatmentWindow.SetActive(true);
                    }
                    else
                    {
                        __instance.m_LabelRiskDescription.text = affliction.m_Description;
                    }
                    
                    if (affliction.HasBuff())
                    {
                        __instance.m_LabelRiskDescription.text = string.Empty;
                        __instance.m_LabelBuffDescription.text = affliction.m_Description;
                        __instance.m_BuffWindow.SetActive(true);
                    }
                }
                
                var uiLabel = __instance.m_LabelAfflictionName;
                
                var riskPercentage = AfflictionManager.TryGetInterface<IRiskPercentage>(affliction);
                if (riskPercentage != null && affliction.InterfaceRisk.Risk) // Need to add another check in here to actually determine if the risk affliction has a timer or not.
                {
                    uiLabel.text = string.Concat(uiLabel.text, " (", riskPercentage.GetRiskPercentage(), "%)");
                }
                
                // Now supports displaying multiple instances of the same custom affliction if the user has one.
                // We should filter this out a bit more, maybe via a boolean to determine if the custom affliction will have multiple (instances).
                var (hasMultiple, count, index) = AfflictionManager.GetAfflictionManagerInstance().CheckMultipleAfflictionsByKey(affliction.m_Name, affliction);
                if (hasMultiple)
                {
                    uiLabel.text = string.Concat(uiLabel.text, " (", index, "/", count, ")");
                }
                
                num = (int)affliction.m_Location;
                num4 = affliction.HasDuration() ? Mathf.CeilToInt(affliction.InterfaceDuration.GetTimeRemaining()) : 0;
                
                break;
            }
            
            __instance.m_LabelCause.text = $"{Localization.Get("GAMEPLAY_AfflictionCausedBy")} {__instance.m_SelectedAffButton.m_LabelCause.text}";
            
            if (num >= 0)
            {
                __instance.m_BodyIconList[num].width = (int)(__instance.m_BodyIconWidthOriginal * 1.5f);
                __instance.m_BodyIconList[num].height = (int)(__instance.m_BodyIconHeightOriginal * 1.5f);
                __instance.UpdateBodyIconActiveAnimation(num, __instance.m_SelectedAffButton.m_AfflictionType);
                __instance.UpdateBodyIconColors(__instance.m_SelectedAffButton, isButtonSelected: true, num);
                __instance.UpdateAllButSelectedBodyIconColors();
            }
            else
                __instance.m_BodyIconActiveAnimationObj.SetActive(false);
            
            if (num4 > 0) // Duration.
            {
                Utils.SetActive(__instance.m_DurationWidgetParentObj, active: true);
                int num5 = num4 / 60;
                int num6 = num4 % 60;
                __instance.m_DurationWidgetHoursLabel.text = num5.ToString();
                __instance.m_DurationWidgetMinutesLabel.text = num6.ToString();
            }
            else
                Utils.SetActive(__instance.m_DurationWidgetParentObj, active: false);
        }
    }

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