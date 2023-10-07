using UnityEngine;

[RequireComponent(typeof(PlayerRigidbody))]
public sealed class PlayerEntity : Entity
{
    [Header("Movement settings")]
    public float MaxSpeed = 6f;

    public float WalkSpeed = 1f;

    public float Acceleration = 7.62f;

    public float JumpPower = 5.112f;

    public float MaxGrabDistance = 3f;

    public float MaxHeldDistance = 6f;

    public float PushForce = 20f;

    protected override void SetProperties()
    {
        Invulnerable = false;
        FullHealth = 100;
    }
}
