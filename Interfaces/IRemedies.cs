namespace AfflictionComponent.Interfaces;

public interface IRemedies
{
    protected bool InstantHeal { get; set; }
    protected Tuple<string, int, int> RemedyItems { get; set; }
    protected Tuple<string, int, int> AltRemedyItems { get; set; }
    
    /// <summary>
    /// Called when InstantHeal is false and all remedy items have been taken. Can be used to run any custom code for that use case.
    /// </summary>
    protected void CureSymptoms();
    
    /// <summary>
    /// Called when the affliction is cured. Can be used to run custom code for this use case.
    /// </summary>
    protected void OnCure();
}