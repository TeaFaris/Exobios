using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// Custom rigidbody for entities. Changes how entities gravity works.
/// </summary>
[RequireComponent(typeof(Rigidbody))]
public class EntityRigidbody : MonoBehaviour
{
    public Rigidbody Native { get; private set; }

    /// <summary>
    /// <see cref="Static"/> from inspector doesn't work, however in code it does, so use this property freely.
    /// </summary>
    public bool Static
    {
        get => _static;
        set
        {
            _static = value;

            if (value)
            {
                Native.useGravity = false;
                Native.isKinematic = true;
            }
            else
            {
                Native.useGravity = true;
                Native.isKinematic = false;
            }
        }
    }

    [InspectorLabel("Static (Works only before execution)")]
    [SerializeField]
    private bool _static;

    [field: Header("Collision settings")]
    [field: SerializeField]
    [field: ReadOnlyProperty]
    public Transform Ground { get; protected set; }

#if UNITY_EDITOR
    private InsertList<ContactPoint> contactPointsTrace = new InsertList<ContactPoint>(100);
#endif

    /// <summary>
    /// Override <see cref="OnCollisionEntityEnter"/> instead.
    /// </summary>
    protected void OnCollisionEnter(Collision collision)
    {
        if (Ground != null)
        {
            OnCollisionEntityEnter(collision);
            return;
        }

        for (int i = 0; i < collision.contactCount; i++)
        {
            var contact = collision.GetContact(i);

            if (contact.point.y >= collision.transform.position.y)
            {
                Ground = collision.transform;
            }
        }

        OnCollisionEntityEnter(collision);
    }

    protected virtual void OnCollisionEntityEnter(Collision collision) { }

    /// <summary>
    /// Override <see cref="OnCollisionEntityStay"/> instead.
    /// </summary>
    protected void OnCollisionStay(Collision collision)
    {
#if UNITY_EDITOR
        contactPointsTrace.Insert(collision.contacts);
#endif

        OnCollisionEntityStay(collision);
    }

    protected virtual void OnCollisionEntityStay(Collision collision) { }

    /// <summary>
    /// Override <see cref="OnCollisionEntityExit"/> instead.
    /// </summary>
    protected void OnCollisionExit(Collision collision)
    {
        if (Ground == collision.transform)
        {
            Ground = null;
        }

        OnCollisionEntityExit(collision);
    }

    protected virtual void OnCollisionEntityExit(Collision collision) { }

#if UNITY_EDITOR
    /// <summary>
    /// Override <see cref="OnDrawEntityGizmos"/> instead.
    /// </summary>
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;

        for (int i = 0; i < contactPointsTrace.Count; i++)
        {
            var contactPoint = contactPointsTrace[i];

            var magnitude = contactPoint.impulse.magnitude / 2f;

            Gizmos.DrawCube(contactPoint.point, new Vector3(magnitude, magnitude, magnitude));
        }

        OnDrawEntityGizmos();
    }

    protected virtual void OnDrawEntityGizmos() { }
#endif

    /// <summary>
    /// Override <see cref="OnAwake"/> instead.
    /// </summary>
    protected void Awake()
    {
        Native = GetComponent<Rigidbody>();

        OnAwake();
    }

    protected virtual void OnAwake() { }

    /// <summary>
    /// Override <see cref="OnStart"/> instead.
    /// </summary>
    protected void Start()
    {
        Static = _static;

        OnStart();
    }

    protected virtual void OnStart() { }
}
