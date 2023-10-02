using System;
using UnityEngine;

[CreateAssetMenu(fileName = "New " + nameof(Keybind), menuName = nameof(Keybind))]
public sealed class Keybind : ScriptableObject
{
    public KeyCode DefaultKey;

    [field: SerializeField, ReadOnlyProperty]
    public KeyCode? Key { get; private set; } = null;

    public string ActionCode;

    public Keybind(string actionCode, KeyCode defaultKey)
    {
        ActionCode = actionCode;
        DefaultKey = defaultKey;
    }

    public event EventHandler KeyDown = delegate { };
    public event EventHandler KeyUp = delegate { };
    public event EventHandler KeyPressed = delegate { };
    public event EventHandler KeyNotPressed = delegate { };

    public bool IsKeyDown { get; private set; }
    public bool IsKeyUp { get; private set; }
    public bool IsKeyPressed { get; private set; }

    public event EventHandler FixedKeyDown = delegate { };
    public event EventHandler FixedKeyUp = delegate { };
    public event EventHandler FixedKeyPressed = delegate { };
    public event EventHandler FixedKeyNotPressed = delegate { };

    public bool IsFixedKeyDown { get; private set; }
    public bool IsFixedKeyUp { get; private set; }
    public bool IsFixedKeyPressed { get; private set; }

    public void Tick()
    {
        KeyCode keyboardKey = Key ?? DefaultKey;

        IsKeyDown = Input.GetKeyDown(keyboardKey);
        IsKeyUp = Input.GetKeyUp(keyboardKey);
        IsKeyPressed = Input.GetKey(keyboardKey);

        if (IsKeyDown)
        {
            KeyDown?.Invoke(this, EventArgs.Empty);
        }
        if (IsKeyUp)
        {
            KeyUp?.Invoke(this, EventArgs.Empty);
        }
        if (IsKeyPressed)
        {
            KeyPressed?.Invoke(this, EventArgs.Empty);
        }
        else
        {
            KeyNotPressed?.Invoke(this, EventArgs.Empty);
        }
    }

    public void FixedTick()
    {
        KeyCode keyboardKey = Key ?? DefaultKey;

        IsFixedKeyDown = Input.GetKeyDown(keyboardKey);
        IsFixedKeyUp = Input.GetKeyUp(keyboardKey);
        IsFixedKeyPressed = Input.GetKey(keyboardKey);

        if (IsKeyDown)
        {
            FixedKeyDown?.Invoke(this, EventArgs.Empty);
        }
        if (IsKeyUp)
        {
            FixedKeyUp?.Invoke(this, EventArgs.Empty);
        }
        if (IsKeyPressed)
        {
            FixedKeyPressed?.Invoke(this, EventArgs.Empty);
        }
        else
        {
            FixedKeyNotPressed?.Invoke(this, EventArgs.Empty);
        }
    }
}