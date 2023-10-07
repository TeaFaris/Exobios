using UnityEngine;

public sealed class Prop : Entity
{
    [field: Header("Interact Settings")]
    [field: SerializeField]
    [field: ReadOnlyProperty]
    public bool IsGrabbed { get; private set; }

    EntityRigidbody _rigidbody;

    protected override void SetProperties()
    {
        _rigidbody = GetComponent<EntityRigidbody>();

        OnInteract += PropOnInteract;
    }

    private void PropOnInteract(object sender, InteractArgs e)
    {
        if (!_rigidbody.Static)
        {
            var player = e.User as PlayerEntity;

            IsGrabbed = player.HoldingProp == this;
        }
    }
}