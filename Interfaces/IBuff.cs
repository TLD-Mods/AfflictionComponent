namespace AfflictionComponent.Interfaces;

public interface IBuff
{
    internal bool Buff { get; }
    
    internal bool HasBuff() => Buff;
}