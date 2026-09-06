using AfflictionComponent.Components;
using AfflictionComponent.Utilities;

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

            // Scroll list objects are pooled, so a slot that once showed a custom sprite is still on that
            // single sprite atlas. Every slot goes back to the base atlas before it is filled, and only a
            // custom sprite entry is moved off it again.
            var baseAtlas = AtlasUtilities.GetBaseCoverflowAtlas(__instance);

            if (afflictionList != null) {

                __instance.m_Afflictions = afflictionList;

                for (int i = 0; i < __instance.m_Afflictions.Count; i++)
                {
                    AfflictionCoverflow componentInChildren = Utils.GetComponentInChildren<AfflictionCoverflow>(__instance.m_ScrollList.m_ScrollObjects[i]);
                    if (!(componentInChildren == null))
                    {
                        __instance.m_CoverflowAfflictions.Add(componentInChildren);
                        AtlasUtilities.ResetToBaseAtlas(componentInChildren.m_SpriteEffect, baseAtlas);
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
                int finalCounter = combinedCount;
    
                for (int j = vanillaAfflictionCount; j < finalCounter; j++)
                {
                    AfflictionCoverflow componentInChildren = Utils.GetComponentInChildren<AfflictionCoverflow>(__instance.m_ScrollList.m_ScrollObjects[j]);
                    if (componentInChildren != null)
                    {
                        __instance.m_CoverflowAfflictions.Add(componentInChildren);
                        AtlasUtilities.ResetToBaseAtlas(componentInChildren.m_SpriteEffect, baseAtlas);

                        int customIndex = j - vanillaAfflictionCount;
                        if (customIndex < GlobalFields.panelAfflictionList.Count)
                        {
                            var customAffliction = GlobalFields.panelAfflictionList[customIndex];
                            if (customAffliction.m_CustomSprite)
                            {
                                for (var l = 0; l < Mod.allCustomAtlas.transform.childCount; l++)
                                {
                                    if ($"CustomAtlas{customAffliction.m_SpriteName}(Clone)" == Mod.allCustomAtlas.transform.GetChild(l).name)
                                    {
                                        componentInChildren.m_SpriteEffect.atlas = Mod.allCustomAtlas.transform.GetChild(l).GetComponent<UIAtlas>();
                                        break;
                                    }
                                }
                            }
                            componentInChildren.m_SpriteEffect.spriteName = customAffliction.m_SpriteName;
                        }
                    }
                }
            }
        }
    }
}