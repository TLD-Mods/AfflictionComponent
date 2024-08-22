using AfflictionComponent.Components;

namespace AfflictionComponent.Patches.SaveGameSystemPatches;

internal static class SaveGlobalData
{
    [HarmonyPatch(typeof(SaveGameSystem), nameof(SaveGameSystem.SaveGlobalData))]
    private static class SaveAfflictionData
    {
        private static void Postfix()
        {
            if (Mod.sdm != null && Mod.afflictionManager != null) Mod.sdm.Save(new AfflictionManagerSaveDataProxy(Mod.afflictionManager.m_Afflictions));
        }
    }
}