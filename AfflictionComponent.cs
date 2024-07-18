
using AfflictionComponent.Afflictions;
using ComplexLogger;

namespace AfflictionComponent
{
    public class Main : MelonMod
    {

        internal static AfflictionManager afflictionManager;
        internal static ComplexLogger<Main> Logger = new();
        public override void OnInitializeMelon()
        {
            Logger.Log("AfflictionComponent is online", FlaggedLoggingLevel.None);
            Settings.OnLoad();
        }
    }
}
