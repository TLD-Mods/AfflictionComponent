using AfflictionComponent.Components;

namespace AfflictionComponent.Patches;

// This hasn't been tested, I have no idea if it works or not.
internal static class PanelAfflictionPatches
{
    [HarmonyPatch(typeof(Panel_Affliction), nameof(Panel_Affliction.SetupScrollList))]
    private static class SetupCustomAfflictionOnScrollList
    {
        private static bool Prefix() { return false; }
        
        private static void Postfix(ref Il2CppSystem.Collections.Generic.List<Affliction> afflictionList, Panel_Affliction __instance)
        {

            if (afflictionList == null && AfflictionManager.GetAfflictionManagerInstance().GetCustomAfflictionCount() == 0) return;

            Mod.Logger.Log("Calling SetupScrollList", ComplexLogger.FlaggedLoggingLevel.Debug);
          
            int vanillaAfflictionCount = afflictionList != null ? afflictionList.Count : 0;
            int moddedAfflictionCount = Mod.afflictionManager.GetCustomAfflictionCount();

            Mod.Logger.Log($"Vanilla afflictions count: {vanillaAfflictionCount}", ComplexLogger.FlaggedLoggingLevel.Debug);
            Mod.Logger.Log($"Modded afflictions count: {moddedAfflictionCount}", ComplexLogger.FlaggedLoggingLevel.Debug);

            int combinedCount = vanillaAfflictionCount + moddedAfflictionCount;
            Mod.Logger.Log($"Combined afflictions count: {combinedCount}", ComplexLogger.FlaggedLoggingLevel.Debug);

            Mod.Logger.Log("Starting...", ComplexLogger.FlaggedLoggingLevel.Debug);

            __instance.m_CoverflowAfflictions.Clear();
            __instance.m_ScrollList.CleanUp();
            __instance.m_ScrollList.CreateList(combinedCount);

            if (afflictionList != null) {

                __instance.m_Afflictions = afflictionList;

                for (int i = 0; i < __instance.m_Afflictions.Count; i++)
                {
                    Mod.Logger.Log($"Vanilla affliction index: {i}", ComplexLogger.FlaggedLoggingLevel.Debug);
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

            if(AfflictionManager.GetAfflictionManagerInstance().GetCustomAfflictionCount() > 0)
            {

                int finalCounter = vanillaAfflictionCount <= moddedAfflictionCount ? combinedCount - 1 : combinedCount;
  
                for (int j = vanillaAfflictionCount; j < finalCounter; j++)
                {
                    Mod.Logger.Log($"Custom affliction index: {j}", ComplexLogger.FlaggedLoggingLevel.Debug);
                    Mod.Logger.Log($"Final counter: {finalCounter}", ComplexLogger.FlaggedLoggingLevel.Debug);

                    AfflictionCoverflow componentInChildren = Utils.GetComponentInChildren<AfflictionCoverflow>(__instance.m_ScrollList.m_ScrollObjects[j]);
                    if (componentInChildren != null)
                    {
                        __instance.m_CoverflowAfflictions.Add(componentInChildren);

                        componentInChildren.m_SpriteEffect.spriteName = Mod.afflictionManager.m_Afflictions[j - vanillaAfflictionCount].GetSpriteName();
                    }
                }
            }
        }
    }

    [HarmonyPatch(typeof(Panel_Affliction), nameof(Panel_Affliction.RefreshVisuals))]
    public static class RefreshVisualOverride
    {
        public static bool Prefix() { return false; }

        public static void Postfix(Panel_Affliction __instance)
        {

            int tweenTargetIndex = __instance.m_ScrollList.GetTweenTargetIndex();
            for (int i = 0; i < __instance.m_CoverflowAfflictions.Count; i++)
            {
                bool isSelected = tweenTargetIndex == i;
                __instance.UpdateCoverFlowColor(i, isSelected);
            }
            __instance.UpdateSelectedAffliction(tweenTargetIndex);
            bool flag = __instance.m_Afflictions.Count + AfflictionManager.GetAfflictionManagerInstance().GetCustomAfflictionCount() > 1;
            Utils.SetActive(__instance.m_ButtonLeft, flag && __instance.m_ScrollList.GetTweenTargetIndex() > 0);
            Utils.SetActive(__instance.m_ButtonLeftGamepad, flag && __instance.m_ScrollList.GetTweenTargetIndex() > 0);
            Utils.SetActive(__instance.m_ButtonRight, flag && __instance.m_ScrollList.GetTweenTargetIndex() < __instance.m_ScrollList.m_ScrollObjects.Count - 1);
            Utils.SetActive(__instance.m_ButtonRightGamepad, flag && __instance.m_ScrollList.GetTweenTargetIndex() < __instance.m_ScrollList.m_ScrollObjects.Count - 1);
            __instance.m_MouseButtonTreatWounds.SetLocID(__instance.m_TreatWoundsLocalizationId);
            __instance.UpdateButtonLegend();
        }
    }

    [HarmonyPatch(typeof(Panel_Affliction), nameof(Panel_Affliction.UpdateCoverFlowColor))]

    public static class UpdateCoverFlowCoverOverride
    {
        public static bool Prefix() { return false; }

        public static void Postfix(Panel_Affliction __instance, ref int index, ref bool isSelected)
        {

            Mod.Logger.Log("Coverflow colour shit", ComplexLogger.FlaggedLoggingLevel.Debug);

            if (__instance.m_AfflictionButtonColorReferences)
            {

                AfflictionManager am = AfflictionManager.GetAfflictionManagerInstance();
                Color colorBasedOnAffliction = new();

                if (index > __instance.m_Afflictions.Count)
                {
                    colorBasedOnAffliction = AfflictionManager.GetAfflictionColour(am.m_Afflictions[index - __instance.m_Afflictions.Count].GetAfflictionType());
                }
                else
                {
                    colorBasedOnAffliction = __instance.m_AfflictionButtonColorReferences.GetColorBasedOnAffliction(__instance.m_Afflictions[index].m_AfflictionType, isSelected);
                }

                __instance.m_CoverflowAfflictions[index].m_SpriteEffect.color = colorBasedOnAffliction;
            }
        }
    }
}