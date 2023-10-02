using System;

public sealed class AxisArgs : EventArgs
{
    public readonly float X;
    public readonly float Y;

    public AxisArgs(float X, float Y)
    {
        this.Y = Y;
        this.X = X;
    }
}