using System;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public sealed class KeybindSystem : ITickable, IFixedTickable
{
    private readonly List<Keybind> Keybinds = new List<Keybind>();

    public event EventHandler<AxisArgs> AxisChangedUpdate = delegate { };
    public event EventHandler<AxisArgs> AxisChangedFixedUpdate = delegate { };

    public event EventHandler<AxisArgs> MouseAxisChangedUpdate = delegate { };
    public event EventHandler<AxisArgs> MouseAxisChangedFixedUpdate = delegate { };

    public void Add(Keybind keybind)
    {
        Keybinds.Add(keybind);
    }

    public Keybind this[string actionCode]
    {
        get
        {
            Keybind foundKeybind = Keybinds.Find(x => x.ActionCode == actionCode);

            return foundKeybind == null
                ? throw new ArgumentNullException(nameof(actionCode))
                : foundKeybind;
        }
    }

    public void Tick()
    {
        var axisArgs = new AxisArgs(
                Input.GetAxisRaw(InputConstants.RawAxisHorizontal),
                Input.GetAxisRaw(InputConstants.RawAxisVertical)
            );

        var mouseAxisArgs = new AxisArgs(
                Input.GetAxisRaw(InputConstants.MouseX),
                Input.GetAxisRaw(InputConstants.MouseY)
            );

        AxisChangedUpdate.Invoke(this, axisArgs);
        MouseAxisChangedUpdate.Invoke(this, mouseAxisArgs);

        foreach (var keybind in Keybinds)
        {
            keybind.Tick();
        }
    }

    public void FixedTick()
    {
        var axisArgs = new AxisArgs(
                Input.GetAxisRaw(InputConstants.RawAxisHorizontal),
                Input.GetAxisRaw(InputConstants.RawAxisVertical)
            );

        var mouseAxisArgs = new AxisArgs(
                Input.GetAxisRaw(InputConstants.MouseX),
                Input.GetAxisRaw(InputConstants.MouseY)
            );

        AxisChangedFixedUpdate.Invoke(this, axisArgs);
        MouseAxisChangedFixedUpdate.Invoke(this, mouseAxisArgs);

        foreach (var keybind in Keybinds)
        {
            keybind.FixedTick();
        }
    }

    private KeybindSystem() { }
}
