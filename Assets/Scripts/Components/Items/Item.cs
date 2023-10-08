using System;
using UnityEngine;
using Zenject;

public abstract class Item : MonoBehaviour
{
    public ushort Count = 0;

    public abstract string Name { get; }

    public event EventHandler OnUse1Down = delegate { };
    public event EventHandler OnUse1Up = delegate { };
    public event EventHandler OnUse1Pressed = delegate { };
    public event EventHandler OnUse1NotPressed = delegate { };
    
    public event EventHandler OnUse2Down = delegate { };
    public event EventHandler OnUse2Up = delegate { };
    public event EventHandler OnUse2Pressed = delegate { };
    public event EventHandler OnUse2NotPressed = delegate { };
    
    public event EventHandler OnHold = delegate { };
    public event EventHandler OnHolding = delegate { };

    protected PlayerEntity Player { get; private set; }

    KeybindSystem keybindSystem;

    public void StartHold(object sender)
    {
        OnHold.Invoke(sender, EventArgs.Empty);
    }

    public void Holding(object sender)
    {
        OnHolding.Invoke(sender, EventArgs.Empty);
    }

    [Inject]
    private void Construct(KeybindSystem keybindSystem)
    {
        this.keybindSystem = keybindSystem;
    }

    private void Awake()
    {
        Player = GetComponentInParent<PlayerEntity>();

        keybindSystem[ActionCodes.Use1].KeyDown += (s, e) => OnUse1Down(s, e);
        keybindSystem[ActionCodes.Use1].KeyUp += (s, e) => OnUse1Up(s, e);
        keybindSystem[ActionCodes.Use1].FixedKeyPressed += (s, e) => OnUse1Pressed(s, e);
        keybindSystem[ActionCodes.Use1].FixedKeyNotPressed += (s, e) => OnUse1NotPressed(s, e);

        keybindSystem[ActionCodes.Use2].KeyDown += (s, e) => OnUse2Down(s, e);
        keybindSystem[ActionCodes.Use2].KeyUp += (s, e) => OnUse2Up(s, e);
        keybindSystem[ActionCodes.Use2].FixedKeyPressed += (s, e) => OnUse2Pressed(s, e);
        keybindSystem[ActionCodes.Use2].FixedKeyNotPressed += (s, e) => OnUse2NotPressed(s, e);

        OnAwake();
    }

    protected abstract void OnAwake();
}