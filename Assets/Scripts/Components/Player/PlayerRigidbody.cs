using UnityEngine;

public sealed class PlayerRigidbody : EntityRigidbody
{
    public void SetGround(Transform ground) => Ground = ground;

    private new void OnCollisionEnter(Collision collision) { }
    private new void OnCollisionExit(Collision collision) { }
}