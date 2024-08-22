using AfflictionComponent.Components;

namespace AfflictionComponent.Patches.PanelAfflictionPatches;

internal static class SetupScrollList
{
    [HarmonyPatch(typeof(Panel_Affliction), nameof(Panel_Affliction.SetupScrollList))]
    private static class SetupCustomAfflictionOnScrollList
    {
        private static bool Prefix() => false;
        
        private static void Postfix(ref Il2CppSystem.Collections.Generic.List<Affliction> afflictionList, Panel_Affliction __instance)
        {
            GlobalFields.panelAfflictionList = AfflictionManager.GetAfflictionManagerInstance().GetCustomAfflictionListCurable();

            if (afflictionList == null && GlobalFields.panelAfflictionList.Count == 0) return;

            int vanillaAfflictionCount = afflictionList != null ? afflictionList.Count : 0;
            int moddedAfflictionCount = GlobalFields.panelAfflictionList.Count;

            int combinedCount = vanillaAfflictionCount + moddedAfflictionCount;
            __instance.m_CoverflowAfflictions.Clear();
            __instance.m_ScrollList.CleanUp();
            __instance.m_ScrollList.CreateList(combinedCount);

            if (afflictionList != null) {

                __instance.m_Afflictions = afflictionList;

                for (int i = 0; i < __instance.m_Afflictions.Count; i++)
                {
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

            if (AfflictionManager.GetAfflictionManagerInstance().m_Afflictions.Count > 0)
            {
                int finalCounter = vanillaAfflictionCount <= moddedAfflictionCount ? combinedCount - 1 : combinedCount;
              
                for (int j = vanillaAfflictionCount; j <= finalCounter; j++)
                {
                    AfflictionCoverflow componentInChildren = Utils.GetComponentInChildren<AfflictionCoverflow>(__instance.m_ScrollList.m_ScrollObjects[j]);
                    if (componentInChildren != null)
                    {
                        __instance.m_CoverflowAfflictions.Add(componentInChildren);

                        for (int i = 0; i < GlobalFields.panelAfflictionList.Count; i++)
                        {
                            if (GlobalFields.panelAfflictionList[i].m_CustomSprite)
                            {
                                componentInChildren.m_SpriteEffect.atlas = Mod.customAtlas;
                            }

                            componentInChildren.m_SpriteEffect.spriteName = GlobalFields.panelAfflictionList[j - vanillaAfflictionCount].m_SpriteName;
                        }
                    }
                }
            }
        }
    }
}