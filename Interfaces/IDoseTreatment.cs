namespace AfflictionComponent.Interfaces;

public interface IDoseTreatment
{
    public int DosesRemaining { get; set; }

    public int DosesRequired { get; set; }

    public bool HasTakenDoseToday { get; set; }
}