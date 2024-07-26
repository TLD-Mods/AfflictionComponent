using AfflictionComponent.Components;

namespace AfflictionComponent.Patches;

internal static class GameManagerPatches
{
    /**
    [HarmonyPatch(nameof(GameManager), nameof(GameManager.InstantiateSystems))]
    private static class AddAfflictionManager
    {
        private static void Postfix(GameManager __instance)
        {
            _ = __instance.m_FirstAidSystems.GetComponent<AfflictionManager>() ?? __instance.m_FirstAidSystems.AddComponent<AfflictionManager>();
        }
    }**/
}