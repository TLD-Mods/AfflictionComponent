using AfflictionComponent.Components;
using AfflictionComponent.Resources;

namespace AfflictionComponent.Interfaces
{
    public interface IInstance
    {
        public InstanceType type { get; set; }
        public void OnFoundExistingInstance(CustomAffliction aff);
    }
}
