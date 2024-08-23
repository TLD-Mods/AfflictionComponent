namespace AfflictionComponent.Interfaces;

public interface IBuff
{
    public bool Buff { get; set; }
    
    public bool BuffCold { get; set; }
    
    public bool BuffFatigue { get; set; }
    
    public bool BuffHunger { get; set; }
    
    public bool BuffThirst { get; set; }
}