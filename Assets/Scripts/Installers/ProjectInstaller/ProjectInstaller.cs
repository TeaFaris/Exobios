using System.Collections.Generic;
using UnityEngine;
using Zenject;

[CreateAssetMenu(fileName = "ProjectInstaller", menuName = "Installers/ProjectInstaller")]
public class ProjectInstaller : ScriptableObjectInstaller<ProjectInstaller>
{
    public List<Keybind> keybinds = new List<Keybind>();

    public override void InstallBindings()
    {
        var keybindSystem = new KeybindSystem();

        for (int i = 0; i < keybinds.Count; i++)
        {
            Keybind keybind = keybinds[i];
            keybindSystem.Add(keybind);
        }

        Container.BindInterfacesAndSelfTo<KeybindSystem>().FromInstance(keybindSystem).AsSingle().NonLazy();
    }
}