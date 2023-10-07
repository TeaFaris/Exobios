using UnityEngine;
using Zenject;

[RequireComponent(typeof(Camera))]
public sealed class PlayerCamera : MonoBehaviour
{   
    public Camera Camera { get; private set; }

    [HideInInspector]
    public bool LockCameraRotation = false;

    private Vector3 sensitivity = Vector3.zero;

    private Vector3 cameraAngles = Vector3.zero;

    KeybindSystem keybindSystem;

    [Inject]
    private void Construct(KeybindSystem keybindSystem)
    {
        this.keybindSystem = keybindSystem;
    }

    private void Awake()
    {
        Camera = GetComponent<Camera>();

        sensitivity = new Vector3(
            #if UNITY_EDITOR
                20,
                20
            #else
                PlayerPrefs.GetFloat(PlayerPrefsConstants.Sensitivity),
                PlayerPrefs.GetFloat(PlayerPrefsConstants.Sensitivity)
            #endif
            );

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = true;

        keybindSystem.MouseAxisChangedUpdate += CameraRotation;
    }

    private void CameraRotation(object sender, AxisArgs e)
    {
        if (LockCameraRotation)
            return;

        Vector3 Rotation = new(-(e.Y * sensitivity.y * Time.deltaTime), e.X * sensitivity.x * Time.deltaTime);
        Rotation = cameraAngles + Rotation;
        Rotation.x = Mathf.Clamp(Rotation.x, -90, 90);
        cameraAngles = Rotation;

        Camera.transform.rotation = Quaternion.Euler(cameraAngles);
    }
}