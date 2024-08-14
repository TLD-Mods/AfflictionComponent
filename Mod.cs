using AfflictionComponent.Components;
using AfflictionComponent.TestAfflictions;
using ComplexLogger;

namespace AfflictionComponent;

internal sealed class Mod : MelonMod
{
    internal static AfflictionManager afflictionManager;
    internal static ComplexLogger<Mod> Logger = new();

    public override void OnInitializeMelon()
    {
        Logger.Log("AfflictionComponent is online", FlaggedLoggingLevel.None);
    }

    public override void OnSceneWasInitialized(int buildIndex, string sceneName)
    {
        if (sceneName.ToLowerInvariant().Contains("boot") || sceneName.ToLowerInvariant().Contains("empty")) return;
        if (sceneName.ToLowerInvariant().Contains("menu"))
        {
            UnityEngine.Object.Destroy(GameObject.Find("AfflictionManager"));
            afflictionManager = null;
            return;
        }

        if (!sceneName.Contains("_SANDBOX") && !sceneName.Contains("_DLC") && !sceneName.Contains("_WILDLIFE"))
        {
            if (afflictionManager == null)
            {
                GameObject AfflictionManager = new() { name = "AfflictionManager", layer = vp_Layer.Default };
                UnityEngine.Object.Instantiate(AfflictionManager, GameManager.GetVpFPSPlayer().transform);
                UnityEngine.Object.DontDestroyOnLoad(AfflictionManager);
                afflictionManager = AfflictionManager.AddComponent<AfflictionManager>();
            }
        }
    }

    public override void OnUpdate()
    {
        if (InputManager.GetKeyDown(InputManager.m_CurrentContext, KeyCode.Delete))
        {
            /*TestAffliction test = new TestAffliction("Test Affliction", "TEST", "Test Affliction Description",
                "You have a test affliction, wait until it heals", AfflictionBodyArea.Head, "ico_injury_majorBruising",
                false, false, 1, true, true, [Tuple.Create("GEAR_HeavyBandage", 1, 1)], []);
            
            
            TestAffliction risktest = new TestAffliction("Test Risk Affliction", "TEST",
                "Test Risk Affliction Description",
                "You are at risk of developing test affliction, buy Fuar a drink of suffer the consequences",
                AfflictionBodyArea.Neck, "ico_injury_majorBruising", true, false, 2, false, true,
                [], []); */
            
            TestAffliction buffTest = new TestAffliction("Test Buff Affliction", "TEST",
                "Test Risk Affliction Description",
                "You have a buff!",
                AfflictionBodyArea.Stomach, "ico_injury_majorBruising", false, true, 0.50f, false, false,
                [], []); 
        }
    }
}