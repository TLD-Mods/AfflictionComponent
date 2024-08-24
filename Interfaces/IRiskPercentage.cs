namespace AfflictionComponent.Interfaces;

public interface IRiskPercentage : IRisk
{
    public sealed int GetRiskPercentage() => Mathf.RoundToInt(GetRiskValue());

    public float GetRiskValue();

    protected void UpdateRiskValue();
}