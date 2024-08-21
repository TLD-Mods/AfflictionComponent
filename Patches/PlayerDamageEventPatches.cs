namespace AfflictionComponent.Patches;

internal static class PlayerDamageEventPatches
{
    [HarmonyPatch(typeof(PlayerDamageEvent), nameof(PlayerDamageEvent.SpawnDamageEvent))]
    private static class Test1
    {
        private static void Postfix(PlayerDamageEvent __instance, string damageEventName, string damageEventType,
            string iconName, Color tint, bool fadeout, float displayTime, float fadeoutTime)
        {
            
        }
    }
}