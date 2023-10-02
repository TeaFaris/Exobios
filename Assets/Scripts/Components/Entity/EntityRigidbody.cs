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

    private void Awake()
    {
        Native = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        Native.useGravity = true;
    }
}
