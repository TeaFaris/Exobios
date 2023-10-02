using System;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(EntityRigidbody))]
[RequireComponent(typeof(Collider))]
public abstract class Entity : MonoBehaviour
{
    /* TODO:
     * protected event EventHandler OnUpdate = delegate { };
     * protected event EventHandler OnFixedUpdate = delegate { };
     * protected event EventHandler<SpawnArgs> OnSpawn = delegate { };
     * protected event EventHandler<DamageArgs> OnTakeDamage = delegate { };
     * protected event EventHandler<DamageArgs> OnHeal = delegate { };
     * protected event EventHandler<InteractArgs> OnInteract = delegate { };
     * protected event EventHandler<CollisionArgs> OnEnterCollision = delegate { };
     * protected event EventHandler<CollisionArgs> OnExitCollision = delegate { };
     * protected event EventHandler<CollisionArgs> OnStayCollision = delegate { };
     * protected event EventHandler<DamageArgs> OnDeath = delegate { };
     */

    [Header("Health settings")]
    public bool Invulnerable = false;

    [Range(0, short.MaxValue)]
    public float FullHealth = 1;

    [InspectorLabel("Health")]
    [SerializeField]
    [ReadOnlyProperty]
    private float _health;

    // TODO: public float Health
}