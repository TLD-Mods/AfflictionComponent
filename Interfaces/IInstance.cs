using AfflictionComponent.Components;
using AfflictionComponent.Enums;

namespace AfflictionComponent.Interfaces;

public interface IInstance
{
    public InstanceType Type { get; set; }
    
    public void OnFoundExistingInstance(CustomAffliction customAffliction);
}