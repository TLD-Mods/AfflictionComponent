namespace AfflictionComponent.Interfaces;

public interface IRisk
{
    internal bool Risk { get; set; }

    internal bool HasRisk() => Risk;
}