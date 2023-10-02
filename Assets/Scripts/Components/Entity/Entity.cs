using System;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(EntityRigidbody))]
[RequireComponent(typeof(Collider))]
public abstract class Entity : MonoBehaviour
{
    public event EventHandler<SpawnArgs> OnSpawn = delegate { };
    public event EventHandler<DamageArgs> OnTakeDamage = delegate { };
    public event EventHandler<DamageArgs> OnHeal = delegate { };
    public event EventHandler<InteractArgs> OnInteract = delegate { };
    public event EventHandler<CollisionArgs> OnEnterCollision = delegate { };
    public event EventHandler<CollisionArgs> OnExitCollision = delegate { };
    public event EventHandler<CollisionArgs> OnStayCollision = delegate { };
    public event EventHandler<DamageArgs> OnDeath = delegate { };

    [Header("Health settings")]
    public bool Invulnerable = false;

    /// <summary>
    /// Max allowed health for entity.
    /// </summary>
    [Range(0, float.MaxValue)]
    public float FullHealth = 1;

    [InspectorLabel("Health")]
    [SerializeField]
    [ReadOnlyProperty]
    private float _health;

    // TODO: public float Health

    /// <summary>
    /// Use <see cref="OnSpawn"/> event.
    /// </summary>
    protected void Start()
    {
        _health = FullHealth;

        OnSpawn.Invoke(this, new());
    }
}