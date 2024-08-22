using AfflictionComponent.Components;
using AfflictionComponent.TestAfflictions;
using AfflictionComponent.Utilities;
using ComplexLogger;

namespace AfflictionComponent;

internal sealed class Mod : MelonMod
{
    internal static AfflictionManager afflictionManager;
    internal static ComplexLogger<Mod> Logger = new();
    internal static SaveDataManager sdm = new();

    internal static UIAtlas customAtlas;

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
                
                GameObject CustomAtlas = new() { name = "CustomAtlas", layer = vp_Layer.Default };
                UnityEngine.Object.Instantiate(CustomAtlas, GameManager.GetVpFPSPlayer().transform);
                UnityEngine.Object.DontDestroyOnLoad(CustomAtlas);
                customAtlas = CustomAtlas.AddComponent<UIAtlas>();
            }
        }
    }

    public override void OnUpdate()
    {
        if (InputManager.GetKeyDown(InputManager.m_CurrentContext, KeyCode.Delete))
        {
            var testAffliction = new TestAffliction("Testing Affliction",
                "Testing Cause",
                "Testing Description",
                null,
                "AfflictionComponent.Resources.Lambda.png",
                AfflictionBodyArea.Chest,
                true)
            {
                RemedyItems = [Tuple.Create("GEAR_HeavyBandage", 1, 1)],
                InstantHeal = true
            };

            testAffliction.Start();

            /*var testAfflictionRisk = new TestAffliction("GAMEPLAY_AfflictionTestingRisk",
                "Testing Cause",
                "Testing Risk Description",
                null,
                "ico_injury_majorBruising",
                AfflictionBodyArea.Chest)
            {
                Risk = true,
                InstantHeal = true
            };
            
            testAfflictionRisk.Start();*/

            /*var testAfflictionBuff = new TestAffliction("Testing Buff 1",
                "Testing Cause",
                "Testing Buff Description 1",
                null,
                "AfflictionComponent.Resources.Aperture.png",
                AfflictionBodyArea.Stomach,
                true)
            {
                Buff = true,
                Duration = 2f
            };
            
            testAfflictionBuff.Start();*/
        }
    }
}