using UnityEngine;
using Zenject;

[RequireComponent(typeof(PlayerEntity))]
public sealed class PlayerInventory : MonoBehaviour
{
	ItemRepository itemRepository;

	private Item[] items;

	[Inject]
	private void Consturct(ItemRepository itemRepository)
	{
		this.itemRepository = itemRepository;
	}

	private void Awake()
	{
		items = itemRepository.SetupInventory(this);
	}
}