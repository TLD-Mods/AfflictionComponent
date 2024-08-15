using AfflictionComponent.Components;
using AfflictionComponent.TestAfflictions;
using ComplexLogger;

namespace AfflictionComponent;

internal sealed class Mod : MelonMod
{
    internal static AfflictionManager afflictionManager;
    internal static ComplexLogger<Mod> Logger = new();

    public override void OnInitializeMelon() => RegisterLocalizationKeys("AfflictionComponent.Resources.Localization.json");

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
            var testAffliction = new TestAffliction("GAMEPLAY_AfflictionTesting",
                "GAMEPLAY_AfflictionTestingCause",
                "GAMEPLAY_AfflictionTestingDescription",
                "GAMEPLAY_AfflictionTestingDescriptionNoHeal",
                AfflictionBodyArea.Chest, "ico_injury_majorBruising", 1, true, true,
                [Tuple.Create("GEAR_HeavyBandage", 1, 1)], []);

            testAffliction.Start();

            var testAfflictionRisk = new TestAffliction("GAMEPLAY_AfflictionTestingRisk",
                "GAMEPLAY_AfflictionTestingCause",
                "GAMEPLAY_AfflictionTestingRiskDescription",
                "GAMEPLAY_AfflictionTestingRiskDescriptionNoHeal",
                AfflictionBodyArea.Chest, "ico_injury_majorBruising", 2, false, true,
                [], [])
            {
                Risk = true
            };
            
            testAfflictionRisk.Start();
                
            var testAfflictionBuff = new TestAffliction("GAMEPLAY_AfflictionTestingBuff", 
                "GAMEPLAY_AfflictionTestingCause",
                "GAMEPLAY_AfflictionTestingBuffDescription",
                "GAMEPLAY_AfflictionTestingBuffDescriptionNoHeal",
                AfflictionBodyArea.Stomach, "ico_injury_majorBruising", 0.50f, false, false,
                [], [])
            {
                Buff = true
            };
            
            testAfflictionBuff.Start();
        }
    }
    
    /* --- Testing Localization --- */
    private static void RegisterLocalizationKeys(string jsonFilePath)
    {
        if (string.IsNullOrWhiteSpace(jsonFilePath)) throw new ArgumentNullException(nameof(jsonFilePath));

        using var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(jsonFilePath);
        if (stream == null) throw new FileNotFoundException($"Resource not found: {jsonFilePath}");

        using var reader = new StreamReader(stream);
        var jsonText = reader.ReadToEnd();

        if (string.IsNullOrWhiteSpace(jsonText)) throw new InvalidDataException("JSON content is empty or whitespace.");

        LocalizationManager.LoadJsonLocalization(jsonText);
    }
}