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
            int combinedCount = __instance.m_Afflictions.Count + Mod.afflictionManager.GetCustomAfflictionCount();

            if (afflictionList == null) return;
            
            __instance.m_Afflictions = afflictionList;
            __instance.m_CoverflowAfflictions.Clear();
            __instance.m_ScrollList.CleanUp();
            __instance.m_ScrollList.CreateList(combinedCount);
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

            for (int j = __instance.m_Afflictions.Count; j < combinedCount; j++)
            {
                AfflictionCoverflow componentInChildren = Utils.GetComponentInChildren<AfflictionCoverflow>(__instance.m_ScrollList.m_ScrollObjects[j]);
                if (componentInChildren != null)
                {
                    __instance.m_CoverflowAfflictions.Add(componentInChildren);

                    componentInChildren.m_SpriteEffect.spriteName = Mod.afflictionManager.m_Afflictions[j].GetSpriteName();
                }
            }
        }
    }
}