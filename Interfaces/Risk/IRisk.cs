namespace AfflictionComponent.Interfaces.Risk;

public interface IRisk
{
    protected bool Risk { get; set; }

    protected sealed bool HasRisk() => Risk;
}