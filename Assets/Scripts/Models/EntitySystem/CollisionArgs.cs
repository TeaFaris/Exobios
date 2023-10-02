using System;
using UnityEngine;

public sealed class CollisionArgs : EventArgs
{
    public readonly Collision Collision;

    public CollisionArgs(Collision collision)
    {
        this.Collision = collision;
    }
}