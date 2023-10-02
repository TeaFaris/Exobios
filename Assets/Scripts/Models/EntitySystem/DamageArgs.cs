using System;

public sealed class DamageArgs : EventArgs
{
    public readonly float Value;

    public readonly DamageSource Source;

    public DamageArgs(float value, DamageSource source)
    {
        Value = value;
        Source = source;
    }
}