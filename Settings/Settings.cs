namespace AfflictionComponent
{
    public class Settings : JsonModSettings
    {
        internal static Settings Instance { get; } = new();

       

        // this is used to set things when user clicks confirm. If you dont need this ability, dont include this method
        protected override void OnConfirm()
        {
            base.OnConfirm();

           
        }

        // this is called whenever there is a change. Ensure it contains the null bits that the base method has
        protected override void OnChange(FieldInfo field, object? oldValue, object? newValue)
        {
            base.OnChange(field, oldValue, newValue);
        }

        // This is used to load the settings
        internal static void OnLoad()
        {
            Instance.AddToModSettings(BuildInfo.GUIName);
            Instance.RefreshGUI();
        }
    }
}