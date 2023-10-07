using UnityEngine;
using Zenject;

[RequireComponent(typeof(PlayerMovement))]
public sealed class PlayerController : MonoBehaviour
{
    [SerializeField]
    [ReadOnlyProperty]
    private Prop holdingProp;
    private float previousDrag;
    private float previousAngularDrag;
    private bool rotateMode;

    private Transform objectHeldPoint;

    KeybindSystem keybindSystem;
    PlayerEntity playerEntity;
    PlayerRigidbody playerRigidbody;
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
        _camera = GetComponentInChildren<Camera>();

        objectHeldPoint = new GameObject(nameof(objectHeldPoint)).transform;
        objectHeldPoint.parent = _camera.transform;
        objectHeldPoint.localPosition = new Vector3(0, 0, playerEntity.MaxGrabDistance);

        keybindSystem[ActionCodes.Use].KeyDown += OnUseDown;
        keybindSystem[ActionCodes.Use].KeyUp += OnUseUp;
        keybindSystem[ActionCodes.Use].FixedKeyPressed += OnUsePressed;
    }

    private void FixedUpdate()
    {
        UseUpdate();
    }

    private void UseUpdate()
    {
        if(holdingProp == null)
        {
            return;
        }

        var holdingPropRigid = holdingProp.GetComponent<EntityRigidbody>();
        
        var mass = holdingPropRigid.Native.mass;

        if(mass < MovementConstants.HeavyProp)
        {
            var distance = Vector3.Distance(transform.position, holdingProp.transform.position);

            var belowPlayer = playerRigidbody.Ground == holdingProp.transform;

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

            var propDistance = objectHeldPoint.position - holdingProp.transform.position;

            var wishVelocity = propDistance / (Time.fixedDeltaTime * mass * massModifier);
            var force = wishVelocity - holdingPropRigid.Native.velocity;

            holdingPropRigid.Native.AddForce(force);
        }
    }

    private void OnUseDown(object sender, System.EventArgs e)
    {
        if (holdingProp != null)
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

            holdingProp = raycastedProp;
            propRigid.Native.useGravity = false;

            previousDrag = nativePropRigid.drag;
            nativePropRigid.drag = MovementConstants.DragPower / nativePropRigid.mass;

            raycastedProp.Interact(this, new InteractArgs(playerEntity));
        }
    }

    private void DropProp()
    {
        var propRigid = holdingProp.GetComponent<EntityRigidbody>();

        rotateMode = false;

        propRigid.Native.drag = previousDrag;
        propRigid.Native.angularDrag = previousAngularDrag;

        propRigid.Native.useGravity = true;

        holdingProp.Interact(this, new InteractArgs(playerEntity));

        holdingProp = null;
    }

    private void OnUsePressed(object sender, System.EventArgs e)
    {
        if(holdingProp == null)
        {
            return;
        }

        var propRigid = holdingProp.GetComponent<EntityRigidbody>();

        if(propRigid.Native.mass >= 25)
        {
            if(Vector3.Distance(transform.position, holdingProp.transform.position) > playerEntity.MaxHeldDistance)
            {
                DropProp();
                return;
            }

            var wishPosition = new Vector3(objectHeldPoint.position.x, holdingProp.transform.position.y, objectHeldPoint.position.z);

            var lerpedPosition = Vector3.Lerp(holdingProp.transform.position, wishPosition, Time.fixedDeltaTime * (10f / propRigid.Native.mass));

            propRigid.Native.MovePosition(lerpedPosition);
        }
    }

    private void OnUseUp(object sender, System.EventArgs e)
    {
        if(holdingProp == null)
        {
            return;
        }

        var propRigid = holdingProp.GetComponent<EntityRigidbody>();
        
        if(propRigid.Native.mass >= 25)
        {
            DropProp();
        }
    }
}