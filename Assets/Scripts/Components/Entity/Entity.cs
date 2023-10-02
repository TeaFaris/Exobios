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

    public float Health
    {
        get => _health;
        set
        {
            _health = value;

            if (Invulnerable)
            {
                return;
            }

            if (_health <= 0)
            {
                OnDeath.Invoke(this, new(0, new DamageSource(this, this, "{0} was killed.")));
            }
        }
    }

    /// <summary>
    /// Use <see cref="OnSpawn"/> event.
    /// </summary>
    protected void Start()
    {
        _health = FullHealth;

        OnSpawn.Invoke(this, new());
    }

    protected void OnCollisionEnter(Collision collision)
    {
        OnEnterCollision.Invoke(this, new CollisionArgs(collision));
    }
    protected void OnCollisionExit(Collision collision)
    {
        OnExitCollision.Invoke(this, new CollisionArgs(collision));
    }
    protected void OnCollisionStay(Collision collision)
    {
        OnStayCollision.Invoke(this, new CollisionArgs(collision));
    }

    public override string ToString() => name;
}