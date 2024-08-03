using AfflictionComponent.Components;
using ComplexLogger;

namespace AfflictionComponent.Patches;

// This hasn't been tested, I have no idea if it works or not.
internal static class PanelAfflictionPatches
{
    public static CustomAffliction selectedCustomAffliction;
    public static List<CustomAffliction> panelAfflictionList;

    [HarmonyPatch(typeof(Panel_Affliction), nameof(Panel_Affliction.SetupScrollList))]
    private static class SetupCustomAfflictionOnScrollList
    {
        private static bool Prefix() => false;
        
        private static void Postfix(ref Il2CppSystem.Collections.Generic.List<Affliction> afflictionList, Panel_Affliction __instance)
        {

            panelAfflictionList = AfflictionManager.GetAfflictionManagerInstance().GetCustomAfflictionListCurable();

            if (afflictionList == null && panelAfflictionList.Count == 0) return;

            int vanillaAfflictionCount = afflictionList != null ? afflictionList.Count : 0;
            int moddedAfflictionCount = panelAfflictionList.Count();

            int combinedCount = vanillaAfflictionCount + moddedAfflictionCount;
            __instance.m_CoverflowAfflictions.Clear();
            __instance.m_ScrollList.CleanUp();
            __instance.m_ScrollList.CreateList(combinedCount);

            if (afflictionList != null) {

                __instance.m_Afflictions = afflictionList;

                for (int i = 0; i < __instance.m_Afflictions.Count; i++)
                {
                    //Mod.Logger.Log($"Vanilla affliction index: {i}", ComplexLogger.FlaggedLoggingLevel.Debug);
                    AfflictionCoverflow componentInChildren = Utils.GetComponentInChildren<AfflictionCoverflow>(__instance.m_ScrollList.m_ScrollObjects[i]);
                    if (!(componentInChildren == null))
                    {
                        __instance.m_CoverflowAfflictions.Add(componentInChildren);
                        if (__instance.m_Afflictions[i].IsValid())
                        {
                            componentInChildren.SetAffliction(__instance.m_Afflictions[i]);
                        }
                        else
                        {
                            componentInChildren.SetEmptySlot();
                        }
                    }
                }
            }   

            if (AfflictionManager.GetAfflictionManagerInstance().GetCustomAfflictionCount() > 0)
            {
                int finalCounter = vanillaAfflictionCount <= moddedAfflictionCount ? combinedCount - 1 : combinedCount;
  
                for (int j = vanillaAfflictionCount; j < finalCounter; j++)
                {
                    //Mod.Logger.Log($"Custom affliction index: {j}", ComplexLogger.FlaggedLoggingLevel.Debug);
                    //Mod.Logger.Log($"Final counter: {finalCounter}", ComplexLogger.FlaggedLoggingLevel.Debug);

                    AfflictionCoverflow componentInChildren = Utils.GetComponentInChildren<AfflictionCoverflow>(__instance.m_ScrollList.m_ScrollObjects[j]);
                    if (componentInChildren != null)
                    {
                        __instance.m_CoverflowAfflictions.Add(componentInChildren);

                        componentInChildren.m_SpriteEffect.spriteName = panelAfflictionList[j - vanillaAfflictionCount].GetSpriteName();
                    }
                }
            }
        }
    }

    // This part: ---__instance.m_MouseButtonTreatWounds.SetLocID(__instance.m_TreatWoundsLocalizationId);---
    // Probably don't account for custom afflictions - we will need to investigate that more.
    [HarmonyPatch(typeof(Panel_Affliction), nameof(Panel_Affliction.RefreshVisuals))]
    private static class RefreshVisualOverride
    {
        private static bool Prefix() => false;

        private static void Postfix(Panel_Affliction __instance)
        {

            int tweenTargetIndex = __instance.m_ScrollList.GetTweenTargetIndex();
            for (int i = 0; i < __instance.m_CoverflowAfflictions.Count; i++)
            {
                bool isSelected = tweenTargetIndex == i;
                __instance.UpdateCoverFlowColor(i, isSelected);
            }
            __instance.UpdateSelectedAffliction(tweenTargetIndex);
            Utils.SetActive(__instance.m_ButtonLeft, __instance.m_ScrollList.GetTweenTargetIndex() > 0);
            Utils.SetActive(__instance.m_ButtonLeftGamepad, __instance.m_ScrollList.GetTweenTargetIndex() > 0);
            Utils.SetActive(__instance.m_ButtonRight, __instance.m_ScrollList.GetTweenTargetIndex() < __instance.m_ScrollList.m_ScrollObjects.Count - 1);
            Utils.SetActive(__instance.m_ButtonRightGamepad, __instance.m_ScrollList.GetTweenTargetIndex() < __instance.m_ScrollList.m_ScrollObjects.Count - 1);
            __instance.m_MouseButtonTreatWounds.SetLocID(__instance.m_TreatWoundsLocalizationId);
            __instance.UpdateButtonLegend();
        }
    }

    [HarmonyPatch(typeof(Panel_Affliction), nameof(Panel_Affliction.UpdateCoverFlowColor))]
    private static class UpdateCoverFlowCoverOverride
    {
        private static bool Prefix() => false;

        private static void Postfix(Panel_Affliction __instance, ref int index, ref bool isSelected)
        {
            AfflictionManager am = AfflictionManager.GetAfflictionManagerInstance();
            Color colorBasedOnAffliction;

            if (index >= __instance.m_Afflictions.Count)
            {
                int customAfflictionIndex = index - __instance.m_Afflictions.Count;
                if (customAfflictionIndex < 0 || customAfflictionIndex >= am.m_Afflictions.Count) // This handles the index out of range error.
                {
                    Mod.Logger.Log($"Invalid custom affliction index: {customAfflictionIndex}. Custom afflictions count: {panelAfflictionList.Count}", ComplexLogger.FlaggedLoggingLevel.Warning);
                    return;
                }
                colorBasedOnAffliction = AfflictionManager.GetAfflictionColour(panelAfflictionList[customAfflictionIndex].GetAfflictionType());
            }
            else
            {
                colorBasedOnAffliction = __instance.m_AfflictionButtonColorReferences.GetColorBasedOnAffliction(__instance.m_Afflictions[index].m_AfflictionType, isSelected);
            }

            __instance.m_CoverflowAfflictions[index].m_SpriteEffect.color = colorBasedOnAffliction;
        }
    }
    
    [HarmonyPatch(typeof(Panel_Affliction), nameof(Panel_Affliction.UpdateSelectedAffliction), [typeof(Affliction)])]
    private static class UpdateSelectedAfflictionOverride
    {
        private static bool Prefix(Panel_Affliction __instance, Affliction affliction)
        {
            __instance.m_Label.text = Affliction.LocalizedNameFromAfflictionType(affliction.m_AfflictionType, affliction.m_Id);
            __instance.m_LabelCause.text = affliction.m_Cause;
            __instance.m_LabelLocation.text = Panel_Affliction.LocalizedNameFromAfflictionLocation(affliction.m_Location);
            if (__instance.m_AfflictionButtonColorReferences)
            {
                Color colorBasedOnAffliction = __instance.m_AfflictionButtonColorReferences.GetColorBasedOnAffliction(affliction.m_AfflictionType, true);
                __instance.m_LabelCause.color = colorBasedOnAffliction;
                __instance.m_LabelLocation.color = colorBasedOnAffliction;
            }
            
            return false;
        }
    }
    
    [HarmonyPatch(typeof(Panel_Affliction), nameof(Panel_Affliction.UpdateSelectedAffliction), [typeof(int)])]
    private static class UpdateSelectedAfflictionOverrideInt
    {
        private static bool Prefix(Panel_Affliction __instance, int selectedAfflictionIndex)
        {
            int vanillaAfflictionCount = __instance.m_Afflictions?.Count ?? 0;
            int totalAfflictions = vanillaAfflictionCount + panelAfflictionList.Count;

            if (selectedAfflictionIndex >= 0 && selectedAfflictionIndex < totalAfflictions)
            {
                if (selectedAfflictionIndex < vanillaAfflictionCount)
                {
                    __instance.UpdateSelectedAffliction(__instance.m_Afflictions?[selectedAfflictionIndex]);
                }
                else
                {
                    UpdateSelectedCustomAffliction(panelAfflictionList[selectedAfflictionIndex - vanillaAfflictionCount], __instance);
                }
            }
            
            return false;
        }

        private static void UpdateSelectedCustomAffliction(CustomAffliction customAffliction, Panel_Affliction panelAffliction)
        {
            panelAffliction.m_Label.text = customAffliction.m_AfflictionKey;
            panelAffliction.m_LabelCause.text = customAffliction.m_Cause;
            panelAffliction.m_LabelLocation.text = Panel_Affliction.LocalizedNameFromAfflictionLocation(customAffliction.m_Location);

            if (!panelAffliction.m_AfflictionButtonColorReferences) return;
            Color colorBasedOnAffliction = AfflictionManager.GetAfflictionColour(customAffliction.GetAfflictionType());
            panelAffliction.m_LabelCause.color = colorBasedOnAffliction;
            panelAffliction.m_LabelLocation.color = colorBasedOnAffliction;
        }
    }

    [HarmonyPatch(typeof(Panel_Affliction), nameof(Panel_Affliction.TreatWound))]

    private static class TreatWoundOverride
    {
        public static bool Prefix() => false;

        public static void Postfix(Panel_Affliction __instance)
        {

            selectedCustomAffliction = null;

            Affliction afflictionSelected;
            if (!TryGetVanillaAffliction(out afflictionSelected, __instance))
            {
                int tweenTargetIndex = __instance.m_ScrollList.GetTweenTargetIndex();
                int totalCount = panelAfflictionList.Count + __instance.m_Afflictions.Count;

                if (tweenTargetIndex >= 0 && tweenTargetIndex < totalCount)
                {
                    selectedCustomAffliction = panelAfflictionList[tweenTargetIndex - __instance.m_Afflictions.Count];
                    if (selectedCustomAffliction == null) return;
                    Mod.Logger.Log("Found custom affliction to heal", FlaggedLoggingLevel.Debug);
                }
                else return;
            }

            GameManager.GetPlayerManagerComponent().TreatAfflictionWithFirstAid(__instance.m_FirstAidItem, afflictionSelected);
            __instance.Enable(false, null, null);

        }

        private static bool TryGetVanillaAffliction(out Affliction affliction, Panel_Affliction __instance)
        {

            int tweenTargetIndex = __instance.m_ScrollList.GetTweenTargetIndex();
            if (tweenTargetIndex >= 0 && tweenTargetIndex < __instance.m_Afflictions.Count)
            {
                affliction = __instance.m_Afflictions[tweenTargetIndex];
                return true;
            }
            affliction = Affliction.InvalidAffliction;
            return false;

        }

    }
}