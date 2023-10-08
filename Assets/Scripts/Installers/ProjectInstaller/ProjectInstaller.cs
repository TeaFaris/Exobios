using System.Collections.Generic;
using UnityEngine;
using Zenject;

[CreateAssetMenu(fileName = "ProjectInstaller", menuName = "Installers/ProjectInstaller")]
public class ProjectInstaller : ScriptableObjectInstaller<ProjectInstaller>
{
    public List<Keybind> Keybinds = new List<Keybind>();
    public List<Item> ItemsPrefabs = new List<Item>();

    public override void InstallBindings()
    {
        var keybindSystem = new KeybindSystem();

        for (int i = 0; i < Keybinds.Count; i++)
        {
            Keybind keybind = Keybinds[i];
            keybindSystem.Add(keybind);
        }

        var itemRepository = new ItemRepository(ItemsPrefabs);

        Container.BindInterfacesAndSelfTo<KeybindSystem>().FromInstance(keybindSystem).AsSingle().NonLazy();
        Container.Bind<ItemRepository>().FromInstance(itemRepository).AsSingle();
    }
}