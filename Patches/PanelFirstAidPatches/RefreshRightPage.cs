using AfflictionComponent.Components;
using AfflictionComponent.Interfaces;
using AfflictionComponent.Utilities;
using Il2CppTLD.IntBackedUnit;

namespace AfflictionComponent.Patches.PanelFirstAidPatches;

internal static class RefreshRightPage
{
    [HarmonyPatch(typeof(Panel_FirstAid), nameof(Panel_FirstAid.RefreshRightPage))]
    private static class RefreshRightPagePatch
    {
        private static bool Prefix(Panel_FirstAid __instance)
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

            if (__instance.m_SelectedAffButton.m_AfflictionType != AfflictionType.Generic) __instance.m_LabelAfflictionName.text = Affliction.LocalizedNameFromAfflictionType(__instance.m_SelectedAffButton.m_AfflictionType, __instance.m_SelectedAffButton.GetAfflictionIndex());

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

                    var InterfaceRemedies = AfflictionManager.TryGetInterface<IRemedies>(affliction);

                    if (affliction.HasRemedies())
                    {
                    // I don't know if the UI will support more than 2 items.
                    remedySprites = new string[InterfaceRemedies.RemedyItems.Length];
                    remedyNumRequired = new int[InterfaceRemedies.RemedyItems.Length];
                    remedyComplete = new bool[InterfaceRemedies.RemedyItems.Length];

                    for (int i = 0; i < InterfaceRemedies.RemedyItems.Length; i++)
                    {
                        Tuple<string, int, int> e = InterfaceRemedies.RemedyItems[i];
                        remedySprites[i] = e.Item1;
                        remedyNumRequired[i] = e.Item2;
                        remedyComplete[i] = e.Item3 < 1;
                    }

                    if (affliction.HasRemedies(true))
                    {
                        // I don't know if the UI will support more than 2 items.
                        altRemedySprites = new string[InterfaceRemedies.AltRemedyItems.Length];
                        altRemedyNumRequired = new int[InterfaceRemedies.AltRemedyItems.Length];
                        altRemedyComplete = new bool[InterfaceRemedies.AltRemedyItems.Length];

                        for (int i = 0; i < InterfaceRemedies.AltRemedyItems.Length; i++)
                        {
                            Tuple<string, int, int> e = InterfaceRemedies.AltRemedyItems[i];
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
                if (affliction.HasRisk()) // Need to add another check in here to actually determine if the risk affliction has a timer or not.
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

                var interfaceDuration = AfflictionManager.TryGetInterface<IDuration>(affliction);

                num = (int)affliction.m_Location;
                num4 = affliction.HasDuration() ? Mathf.CeilToInt(interfaceDuration.GetTimeRemaining()) : 0;
                
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
}