using AfflictionComponent.Components;
using AfflictionComponent.Interfaces;

namespace AfflictionComponent.Patches.AfflictionButtonPatches;

internal static class UpdateFillBar
{
    // Index was out of range error occurs in this patch when the percentage reaches 100% and is removed from the m_Afflictions list in AfflictionManager.
    // The cause of the problem with the directional indicator seems to be the animator, I don't know why it's treating our custom affliction differently though.
    [HarmonyPatch(nameof(AfflictionButton), nameof(AfflictionButton.UpdateFillBar))]
    private static class UpdateFillBarCustomRiskAffliction
    {
        private static void Postfix(AfflictionButton __instance)
        {
            if (__instance.m_AfflictionType != AfflictionType.Generic) return;

            var customAffliction = AfflictionManager.GetAfflictionManagerInstance().GetAfflictionByIndex(__instance.GetAfflictionIndex());
            
            var riskPercentage = AfflictionManager.TryGetInterface<IRiskPercentage>(customAffliction);
            if (riskPercentage != null && riskPercentage.Risk)
            {
                var num = riskPercentage.GetRiskValue() / 100f; // The 100f slows it down, otherwise right now it's way too quick.
                Utils.SetActive(__instance.m_AnimatorAfflictionBar.gameObject, num > 0f);
                __instance.m_FillSpriteAfflictionBar.fillAmount = Mathf.Lerp(__instance.m_FillSpriteOffset, 1f - __instance.m_FillSpriteOffset, num);
                __instance.m_SizeModifierAfflictionBar.localScale = new Vector3(num, 1f, 1f);
            }

            // TODO: The buff fill bar is no longer updating
            if (customAffliction.HasBuff())
            {
                var interfaceDuration = AfflictionManager.TryGetInterface<IDuration>(customAffliction);
                if (interfaceDuration != null)
                {
                    Mod.Logger.Log($"Time remaining: {interfaceDuration.GetTimeRemaining()}", ComplexLogger.FlaggedLoggingLevel.Debug);
                    Mod.Logger.Log($"Duration in minutes: {interfaceDuration.Duration * 60f}", ComplexLogger.FlaggedLoggingLevel.Debug);
                    var num2 = interfaceDuration.GetTimeRemaining() / (interfaceDuration.Duration * 60f);
                    Mod.Logger.Log($"num2: {num2}", ComplexLogger.FlaggedLoggingLevel.Debug);
                    Utils.SetActive(__instance.m_AnimatorBuffBar.gameObject, num2 > 0f);
                    __instance.m_FillSpriteBuffBar.fillAmount = Mathf.Lerp(__instance.m_FillSpriteOffset, 1f - __instance.m_FillSpriteOffset, num2);
                    __instance.m_SizeModifierBuffBar.localScale = new Vector3(num2, 1f, 1f);
                }
            }
        }
    }
}