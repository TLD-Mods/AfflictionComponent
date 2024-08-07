namespace AfflictionComponent.Interfaces;

public interface IRiskPercentage
{
    bool RiskPercentage => true;

    // TODO: We need a 'standardised' system when it comes to the user calculating the risk value. It's a bit tricky using either floats or integers for the text label and fill bar.
    // Maybe two separate methods? One for just the risk value as a float and a risk percentage as a integer? We could make the risk value overrideable but calculate the percentage for everyone.
    // I guess that's how we can 'standardise' it a bit?
    public float GetRiskValue();
}