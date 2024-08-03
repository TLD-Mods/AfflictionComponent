using AfflictionComponent.Components;
using AsmResolver.DotNet;
using HarmonyLib;
using Il2CppTLD.IntBackedUnit;

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
        public static bool Prefix(Panel_FirstAid __instance)
        {
            if (!__instance.m_SelectedAffButton)
            {
                return true;
            }
            if (__instance.m_SelectedAffButton.m_AfflictionType != AfflictionType.FoodPoisioning && __instance.m_SelectedAffButton.m_AfflictionType != AfflictionType.Dysentery && __instance.m_SelectedAffButton.m_AfflictionType != AfflictionType.Generic)
            {
                return true;
            }

            return false;
        }
        
        private static void Postfix(Panel_FirstAid __instance)
        {

            if (!__instance.m_SelectedAffButton)
            {
                return;
            }
            if (__instance.m_SelectedAffButton.m_AfflictionType != AfflictionType.FoodPoisioning && __instance.m_SelectedAffButton.m_AfflictionType != AfflictionType.Dysentery && __instance.m_SelectedAffButton.m_AfflictionType != AfflictionType.Generic)
            {
                return;
            }

            //generic UI crap, some of it we probably don't even need for this override
            for (int i = 0; i < __instance.m_FakButtons.Length; i++)
            {
                __instance.m_FakButtons[i].SetNeeded(needed: false);
            }
            __instance.m_SpecialTreatmentWindow.SetActive(false);
            __instance.m_BuffWindow.SetActive(false);
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
            
            /* vanilla crap i'm not sure we even need here
            if (Affliction.AfflictionTypeIsBuff(__instance.m_SelectedAffButton.m_AfflictionType))
            {
                if (!Panel_Affliction.HasAffliction(__instance.m_SelectedAffButton.m_AfflictionType))
                {
                    __instance.HideRightPage();
                    return;
                }
            }
            else if (!GameManager.GetConditionComponent().HasSpecificAffliction(__instance.m_SelectedAffButton.m_AfflictionType))
                {
                    __instance.HideRightPage();
                    return;
                }
            */
            
            NGUITools.SetActive(__instance.m_RightPageObject, state: true);
            for (int j = 0; j < __instance.m_BodyIconList.Length; j++)
            {
                __instance.m_BodyIconList[j].width = __instance.m_BodyIconWidthOriginal;
                __instance.m_BodyIconList[j].height = __instance.m_BodyIconHeightOriginal;
            }
            int num = -1;

            if(__instance.m_SelectedAffButton.m_AfflictionType != AfflictionType.Generic) __instance.m_LabelAfflictionName.text = Affliction.LocalizedNameFromAfflictionType(__instance.m_SelectedAffButton.m_AfflictionType, __instance.m_SelectedAffButton.GetAfflictionIndex());

            //colour stuff
            if (Affliction.IsBeneficial(__instance.m_SelectedAffButton.m_AfflictionType))
            {
                __instance.m_LabelAfflictionName.color = InterfaceManager.m_FirstAidBuffColor;
            }
            else
            {
                __instance.m_LabelAfflictionName.color = InterfaceManager.m_FirstAidRedColor;
            }

            int num4 = 0; //THIS IS THE DURATION NUMBER
            int selectedAfflictionIndex = __instance.GetSelectedAfflictionIndex(); //this will most likely need to be modified

            switch (__instance.m_SelectedAffButton.m_AfflictionType)
            {
                case AfflictionType.Dysentery:
                    {
                        DysenteryMethod(__instance, selectedAfflictionIndex, out num, out num4); //this runs the vanilla code, IA will patch this method and override it to call custom code                   
                        break;
                    }
                case AfflictionType.FoodPoisioning:
                    {
                        FoodPoisoningMethod(__instance, selectedAfflictionIndex, out num, out num4); //this runs the vanilla code, IA will patch this method and override it to call custom code                   
                        break;
                    }
                case AfflictionType.Generic: //custom affliction

                    // This handles the out of index range error I was getting in the console.
                    CustomAffliction affliction = null;
                    try { affliction = AfflictionManager.GetAfflictionManagerInstance().GetAfflictionByIndex(selectedAfflictionIndex); }
                    catch (ArgumentOutOfRangeException) { return; }
                    
                    //do stuff here
                    //CustomAffliction affliction = AfflictionManager.GetAfflictionManagerInstance().GetAfflictionByIndex(selectedAfflictionIndex);
                    __instance.m_LabelAfflictionName.text = affliction.m_AfflictionKey;
                    __instance.m_LabelAfflictionDescriptionNoRest.text = "";
                    __instance.m_LabelAfflictionDescription.text = affliction.m_Desc;

                    float hoursPlayedNotPaused = GameManager.GetTimeOfDayComponent().GetHoursPlayedNotPaused();

                    string[] remedySprites;
                    int[] remedyNumRequired;
                    bool[] remedyComplete;

                    if(affliction.m_RemedyItems.Count() != 0)
                    {
                        //I don't know if the UI will support more than 2 items
                        remedySprites = new string[affliction.m_RemedyItems.Count()];
                        remedyNumRequired = new int[affliction.m_RemedyItems.Count()];
                        remedyComplete = new bool[affliction.m_RemedyItems.Count()];

                        for (int i = 0; i < affliction.m_RemedyItems.Length; i++)
                        {
                            Tuple<string, int, int> e = affliction.m_RemedyItems[i];
                            remedySprites[i] = e.Item1;
                            remedyNumRequired[i] = e.Item2;
                            remedyComplete[i] = e.Item3 < 1;
                        }
                    }
                    else
                    {
                        remedySprites = null;
                        remedyNumRequired = null;
                        remedyComplete = null;
                    }

                    string[] altRemedySprites;
                    int[] altRemedyNumRequired;
                    bool[] altRemedyComplete;

                    if (affliction.m_AltRemedyItems.Count() != 0)
                    {
                        //I don't know if the UI will support more than 2 items
                        altRemedySprites = new string[affliction.m_AltRemedyItems.Count()];
                        altRemedyNumRequired = new int[affliction.m_AltRemedyItems.Count()];
                        altRemedyComplete = new bool[affliction.m_AltRemedyItems.Count()];

                        for (int i = 0; i < affliction.m_AltRemedyItems.Length; i++)
                        {
                            Tuple<string, int, int> e = affliction.m_AltRemedyItems[i];
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



                    __instance.SetItemsNeeded(remedySprites, remedyComplete, remedyNumRequired, altRemedySprites, altRemedyComplete, altRemedyNumRequired, ItemLiquidVolume.Zero, 0f, 0f);

                    num = (int)affliction.m_Location;

                    //duration calculation, requires some conditionals

                    if (affliction.m_Permanent)
                    {
                        num4 = 0;
                    }
                    else num4 = Mathf.CeilToInt((affliction.m_EndTime - hoursPlayedNotPaused) * 60f);

                    break;
            }
            __instance.m_LabelCause.text = string.Format("{0} {1}", Localization.Get("GAMEPLAY_AfflictionCausedBy"), __instance.m_SelectedAffButton.m_LabelCause.text);
            if (num >= 0)
            {
                __instance.m_BodyIconList[num].width = (int)((float)__instance.m_BodyIconWidthOriginal * 1.5f);
                __instance.m_BodyIconList[num].height = (int)((float)__instance.m_BodyIconHeightOriginal * 1.5f);
                __instance.UpdateBodyIconActiveAnimation(num, __instance.m_SelectedAffButton.m_AfflictionType);
                __instance.UpdateBodyIconColors(__instance.m_SelectedAffButton, isButtonSelected: true, num);
                __instance.UpdateAllButSelectedBodyIconColors();
            }
            else
            {
                __instance.m_BodyIconActiveAnimationObj.SetActive(false);
            }
            if (num4 > 0) //duration
            {
                Il2Cpp.Utils.SetActive(__instance.m_DurationWidgetParentObj, active: true);
                int num5 = num4 / 60;
                int num6 = num4 % 60;
                __instance.m_DurationWidgetHoursLabel.text = num5.ToString();
                __instance.m_DurationWidgetMinutesLabel.text = num6.ToString();
            }
            else
            {
                Il2Cpp.Utils.SetActive(__instance.m_DurationWidgetParentObj, active: false);
            }
        }

        private static void FoodPoisoningMethod(Panel_FirstAid __instance, int selectedAfflictionIndex, out int num, out int num4)
        {

            FoodPoisoning foodPoisoningComponent = GameManager.GetFoodPoisoningComponent();
            __instance.m_LabelAfflictionDescriptionNoRest.text = "";
            __instance.m_LabelAfflictionDescription.text = foodPoisoningComponent.m_Description;
            string[] remedySprites = new string[]
            {
                "GEAR_BottleAntibiotics"
            };
            bool[] remedyComplete = new bool[]
            {
                foodPoisoningComponent.HasTakenAntibiotics()
            };
            int[] remedyNumRequired = new int[]
            {
                2
            };
            string[] altRemedySprites = new string[]
            {
                "GEAR_ReishiTea"
            };
            bool[] altRemedyComplete = new bool[]
            {
                foodPoisoningComponent.HasTakenAntibiotics()
            };
            int[] altRemedyNumRequired = new int[]
            {
                1
            };
            __instance.SetItemsNeeded(remedySprites, remedyComplete, remedyNumRequired, altRemedySprites, altRemedyComplete, altRemedyNumRequired, ItemLiquidVolume.Zero, foodPoisoningComponent.GetRestAmountRemaining(), foodPoisoningComponent.m_NumHoursRestForCure);
            num = (int)Panel_Affliction.GetAfflictionLocation(AfflictionType.FoodPoisioning, selectedAfflictionIndex);

            num4 = 0; //for compliance
        }

        private static void DysenteryMethod(Panel_FirstAid __instance, int selectedAfflictionIndex, out int num, out int num4)
        {
            Dysentery dysenteryComponent = GameManager.GetDysenteryComponent();
            __instance.m_LabelAfflictionDescriptionNoRest.text = "";
            __instance.m_LabelAfflictionDescription.text = dysenteryComponent.m_Description;
            string[] remedySprites = new string[]
            {
                "GEAR_WaterSupplyPotable",
                "GEAR_BottleAntibiotics"
            };
            bool[] remedyComplete = new bool[]
            {
                dysenteryComponent.GetWaterAmountRemaining().m_Units < 10000000,
                dysenteryComponent.HasTakenAntibiotics()
            };
            int[] remedyNumRequired = new int[]
            {
                1,
                2
            };
            string[] altRemedySprites = new string[]
            {
                "GEAR_WaterSupplyPotable",
                "GEAR_ReishiTea"
            };
            bool[] altRemedyComplete = new bool[]
            {
                dysenteryComponent.GetWaterAmountRemaining().m_Units < 10000000,
                dysenteryComponent.HasTakenAntibiotics()
            };
            int[] altRemedyNumRequired = new int[]
            {
                1,
                1
            };
            __instance.SetItemsNeeded(remedySprites, remedyComplete, remedyNumRequired, altRemedySprites, altRemedyComplete, altRemedyNumRequired, dysenteryComponent.GetWaterAmountRemaining(), dysenteryComponent.GetRestAmountRemaining(), dysenteryComponent.m_NumHoursRestForCure);
            num = (int)Panel_Affliction.GetAfflictionLocation(AfflictionType.Dysentery, selectedAfflictionIndex);
            
            //for compliance
            num4 = 0;
        }
    }
}