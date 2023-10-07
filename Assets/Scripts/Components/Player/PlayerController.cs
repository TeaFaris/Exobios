using UnityEngine;
using Zenject;

[RequireComponent(typeof(PlayerMovement))]
public sealed class PlayerController : MonoBehaviour
{
    private float previousDrag;
    private float previousAngularDrag;
    private bool rotateMode;

    private Transform objectHeldPoint;

    private Vector3 sensitivity = Vector3.zero;

    KeybindSystem keybindSystem;
    PlayerEntity playerEntity;
    PlayerRigidbody playerRigidbody;
    PlayerCamera playerCamera;
    Camera _camera;

    [Inject]
    private void Construct(KeybindSystem keybindSystem)
    {
        this.keybindSystem = keybindSystem;
    }

    private void Awake()
    {
        playerEntity = GetComponent<PlayerEntity>();
        playerRigidbody = GetComponent<PlayerRigidbody>();
        playerCamera = GetComponentInChildren<PlayerCamera>();
        _camera = GetComponentInChildren<Camera>();

        sensitivity = new Vector3(
            #if UNITY_EDITOR
                20,
                20
            #else
                PlayerPrefs.GetFloat(PlayerPrefsConstants.Sensitivity),
                PlayerPrefs.GetFloat(PlayerPrefsConstants.Sensitivity)
            #endif
            );

        objectHeldPoint = new GameObject(nameof(objectHeldPoint)).transform;
        objectHeldPoint.parent = _camera.transform;
        objectHeldPoint.localPosition = new Vector3(0, 0, playerEntity.MaxGrabDistance);

        keybindSystem[ActionCodes.Use].KeyDown += OnUseDown;
        keybindSystem[ActionCodes.Use].KeyUp += OnUseUp;
        keybindSystem[ActionCodes.Use].FixedKeyPressed += OnUsePressed;

        keybindSystem[ActionCodes.Rotate].KeyDown += RotateDown;
        keybindSystem.MouseAxisChangedFixedUpdate += RotatePressed;
        keybindSystem[ActionCodes.Rotate].KeyUp += RotateUp;
    }

    private void FixedUpdate()
    {
        UseUpdate();
    }

    private void RotateDown(object sender, System.EventArgs e)
    {
        if(playerEntity.HoldingProp == null)
        {
            return;
        }

        var propRigid = playerEntity.HoldingProp.GetComponent<Rigidbody>();

        rotateMode = true;
        playerCamera.LockCameraRotation = true;

        previousAngularDrag = propRigid.angularDrag;
        propRigid.angularDrag = MovementConstants.DragPower / propRigid.mass / 10f;
    }

    private void RotatePressed(object sender, AxisArgs e)
    {
        if (!rotateMode)
        {
            return;
        }

        var propRigid = playerEntity.HoldingProp.GetComponent<Rigidbody>();

        float x = Mathf.Clamp(-e.X * sensitivity.x, -20f, 20f);
        float y = Mathf.Clamp(-e.Y * sensitivity.y, -20f, 20f);

        var force = new Vector3(y, x, 0);

        propRigid.AddTorque(force, ForceMode.Force);
    }

    private void RotateUp(object sender, System.EventArgs e)
    {
        if (playerEntity.HoldingProp == null)
        {
            return;
        }
        
        var propRigid = playerEntity.HoldingProp.GetComponent<Rigidbody>();

        propRigid.angularDrag = previousAngularDrag;

        rotateMode = false;
        playerCamera.LockCameraRotation = false;
    }

    private void UseUpdate()
    {
        if(playerEntity.HoldingProp == null)
        {
            return;
        }

        var HoldingPropRigid = playerEntity.HoldingProp.GetComponent<EntityRigidbody>();
        
        var mass = HoldingPropRigid.Native.mass;

        if(mass < MovementConstants.HeavyProp)
        {
            var distance = Vector3.Distance(transform.position, playerEntity.HoldingProp.transform.position);

            var belowPlayer = playerRigidbody.Ground == playerEntity.HoldingProp.transform;

            if(distance > playerEntity.MaxHeldDistance || belowPlayer)
            {
                DropProp();
                return;
            }


            float massModifier = 1;

            if(mass < 0.5f)
            {
                massModifier = 4;
            }
            else if(mass < 1)
            {
                massModifier = 2;
            }

            var propDistance = objectHeldPoint.position - playerEntity.HoldingProp.transform.position;

            var wishVelocity = propDistance / (Time.fixedDeltaTime * mass * massModifier);
            var force = wishVelocity - HoldingPropRigid.Native.velocity;

            HoldingPropRigid.Native.AddForce(force);
        }
    }

    private void OnUseDown(object sender, System.EventArgs e)
    {
        if (playerEntity.HoldingProp != null)
        {
            DropProp();
            return;
        }

        bool isAnyProp = Physics.Raycast(new Ray(_camera.transform.position, _camera.transform.forward), out RaycastHit hittedProp, playerEntity.MaxGrabDistance);
        if (isAnyProp && hittedProp.transform.TryGetComponent(out Prop raycastedProp))
        {
            EntityRigidbody propRigid = raycastedProp.GetComponent<EntityRigidbody>();
            Rigidbody nativePropRigid = propRigid.Native;

            if (propRigid.Static)
            {
                return;
            }

            playerEntity.HoldingProp = raycastedProp;
            propRigid.Native.useGravity = false;

            previousDrag = nativePropRigid.drag;
            nativePropRigid.drag = MovementConstants.DragPower / nativePropRigid.mass;

            raycastedProp.Interact(this, new InteractArgs(playerEntity));
        }
    }

    private void DropProp()
    {
        var propRigid = playerEntity.HoldingProp.GetComponent<EntityRigidbody>();

        rotateMode = false;

        propRigid.Native.drag = previousDrag;
        propRigid.Native.angularDrag = previousAngularDrag;

        propRigid.Native.useGravity = true;

        playerEntity.HoldingProp.Interact(this, new InteractArgs(playerEntity));

        playerEntity.HoldingProp = null;
    }

    private void OnUsePressed(object sender, System.EventArgs e)
    {
        if(playerEntity.HoldingProp == null)
        {
            return;
        }

        var propRigid = playerEntity.HoldingProp.GetComponent<EntityRigidbody>();

        if(propRigid.Native.mass >= 25)
        {
            if(Vector3.Distance(transform.position, playerEntity.HoldingProp.transform.position) > playerEntity.MaxHeldDistance)
            {
                DropProp();
                return;
            }

            var wishPosition = new Vector3(objectHeldPoint.position.x, playerEntity.HoldingProp.transform.position.y, objectHeldPoint.position.z);

            var lerpedPosition = Vector3.Lerp(playerEntity.HoldingProp.transform.position, wishPosition, Time.fixedDeltaTime * (10f / propRigid.Native.mass));

            propRigid.Native.MovePosition(lerpedPosition);
        }
    }

    private void OnUseUp(object sender, System.EventArgs e)
    {
        if(playerEntity.HoldingProp == null)
        {
            return;
        }

        var propRigid = playerEntity.HoldingProp.GetComponent<EntityRigidbody>();
        
        if(propRigid.Native.mass >= 25)
        {
            DropProp();
        }
    }


}