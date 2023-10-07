using Unity.VisualScripting;
using UnityEngine;
using Zenject;

[RequireComponent(typeof(PlayerEntity))]
[RequireComponent(typeof(CapsuleCollider))]
public sealed class PlayerMovement : MonoBehaviour
{
    [SerializeField]
    private Transform groundCheckPosition;

    [Header("Movement Stats")]
    [SerializeField]
    [InspectorLabel("Velocity")]
    [ReadOnlyProperty]
    private Vector3 velocity = Vector3.zero;

    private Vector3 origin = Vector3.zero;
    private Vector2 move = Vector2.zero;

    KeybindSystem keybindSystem;

    PlayerEntity player;
    PlayerRigidbody _playerRigidbody;
    Camera _camera;
    CapsuleCollider _collider;

    [Inject]
    private void Construct(KeybindSystem keybindSystem)
    {
        this.keybindSystem = keybindSystem;
    }

    private void Awake()
    {
        player = GetComponent<PlayerEntity>();
        _playerRigidbody = GetComponent<PlayerRigidbody>();
        _camera = GetComponentInChildren<Camera>();
        _collider = GetComponent<CapsuleCollider>();

        origin = transform.position;

        keybindSystem[ActionCodes.Jump].KeyDown += Jump;

        keybindSystem.AxisChangedFixedUpdate += MovementInput;
    }

    private void FixedUpdate()
    {
        CheckGround();
        Move();
    }

    private void Jump(object sender, System.EventArgs e)
    {
        if (_playerRigidbody.Ground != null)
        {
            velocity.y = 0;
            velocity.y += player.JumpPower;
        }
    }

    private void MovementInput(object sender, AxisArgs e)
    {
        move = new Vector2(e.X * player.Acceleration, e.Y * player.Acceleration);
    }

    private void CheckGround()
    {
        Collider[] colliders = Physics.OverlapSphere(groundCheckPosition.position, _collider.radius);

        for (int i = colliders.Length - 1; i >= 0; i--)
        {
            Collider collider = colliders[i];

            if (collider != _collider)
            {
                _playerRigidbody.SetGround(collider.transform);
                return;
            }
        }

        _playerRigidbody.SetGround(null);
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;

        Gizmos.DrawSphere(groundCheckPosition.position, _collider.radius);
    }
#endif

    private void Move()
    {
        if (_playerRigidbody.Ground == null)
        {
            velocity.y -= -Physics.gravity.y * 1.55f * Time.fixedDeltaTime;

            velocity += AirMovement();

            PhysicsExtensions.Reflect(ref velocity, _collider, origin, Time.fixedDeltaTime);
        }
        else
        {
            velocity.y = velocity.y < 0 ? 0 : velocity.y;

            velocity += GroundInputMovement();

            ApplyFriction();
        }

        origin += velocity * Time.fixedDeltaTime;

        PhysicsExtensions.ResolveCollisions(_collider, ref origin, ref velocity);

        transform.position = origin;

        Vector3 AirMovement()
        {
            GetWishValues(out _, out var wishDirectory, out var wishSpeed);

            if (wishSpeed != 0f && (wishSpeed > player.MaxSpeed))
            {
                wishSpeed = player.MaxSpeed;
            }

            return PhysicsExtensions.AirAccelerate(velocity, wishDirectory, wishSpeed, MovementConstants.AirAcceleration, MovementConstants.AirCap, Time.fixedDeltaTime);
        }

        Vector3 GroundInputMovement()
        {
            GetWishValues(out _, out Vector3 wishDir, out float wishSpeed);

            if ((wishSpeed != 0f) && (wishSpeed > player.MaxSpeed))
            {
                wishSpeed = player.MaxSpeed;
            }

            wishSpeed *= player.WalkSpeed;

            return PhysicsExtensions.Accelerate(velocity, wishDir, wishSpeed, player.Acceleration, Time.fixedDeltaTime, 1f);
        }

        void GetWishValues(out Vector3 wishVelocity, out Vector3 wishDirectory, out float wishSpeed)
        {
            wishVelocity = Vector3.zero;

            Vector3 forward = _camera.transform.forward;
            Vector3 right = _camera.transform.right;

            forward.y = 0;
            right.y = 0;

            forward.Normalize();
            right.Normalize();

            for (int i = 0; i < 3; i++)
            {
                wishVelocity[i] = (forward[i] * move.y) + (right[i] * move.x);
            }

            wishVelocity.y = 0;

            wishSpeed = wishVelocity.magnitude;
            wishDirectory = wishVelocity.normalized;
        }

        void ApplyFriction()
        {
            if (_playerRigidbody.Ground.TryGetComponent(out Collider ground) && ground.material != null)
            {
                float friction = ground.material.dynamicFriction * MovementConstants.FrictionMultiplier;
                PhysicsExtensions.Friction(ref velocity, MovementConstants.StopSpeed, friction, Time.fixedDeltaTime);
            }
        }
    }
}