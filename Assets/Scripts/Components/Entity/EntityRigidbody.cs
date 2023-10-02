using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// Custom rigidbody for entities. Changes how entities gravity works.
/// </summary>
[RequireComponent(typeof(Rigidbody))]
public class EntityRigidbody : MonoBehaviour
{
    [HideInInspector]
    public Rigidbody Native;

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
                if (Native.useGravity)
                {
                    Native.useGravity = false;
                }
            }
            else
            {
                if (!Native.useGravity)
                {
                    Native.useGravity = true;
                }
            }
        }
    }

    [InspectorLabel("Static (Works only before execution)")]
    [SerializeField]
    private bool _static;

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
        Native.useGravity = true;

        OnStart();
    }

    protected virtual void OnStart() { }
}
