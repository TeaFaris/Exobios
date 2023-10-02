using UnityEngine;

/// <summary>
/// Custom rigidbody for entities. Changes how entities gravity works.
/// </summary>
[RequireComponent(typeof(Rigidbody))]
public class EntityRigidbody : MonoBehaviour
{
    public Rigidbody Native;

    [field: SerializeField]
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
