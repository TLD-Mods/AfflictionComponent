namespace AfflictionComponent.Interfaces;

public interface IRemedies
{
    public Tuple<string, int, int>[] AltRemedyItems { get; set; }
    
    public bool InstantHeal { get; set; }
    
    public Tuple<string, int, int>[] RemedyItems { get; set; } // GearItem, Required Amount, Current Amount.
    
    /// <summary>
    /// Called when InstantHeal is false and all remedy items have been taken. Can be used to run any custom code for that use case.
    /// </summary>
    public void CureSymptoms();
    
    /// <summary>
    /// Called when the affliction is cured. Can be used to run custom code for this use case.
    /// </summary>
    public void OnCure();
}