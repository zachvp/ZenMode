using UnityEngine;
using UnityEngine.UI;
using ZMConfiguration;
using ZMPlayer;
using Core;

// This is a general class for creating player-associated objects and configuring them accordingly.
public class ZMPlayerItemCreation : MonoBehaviour
{
	// The object to create.
	[SerializeField] private ZMPlayerItem template;
	[SerializeField] private Transform startTransform;

	protected virtual void Awake()
	{
		ZMPlayerController.OnPlayerCreate += HandlePlayerCreate;
	}

	private void HandlePlayerCreate(ZMPlayerInfo info)
	{
		SpawnItem(info.ID);
	}

	private void SpawnItems()
	{
		for (int i = 0; i < Settings.MatchPlayerCount.value; ++i)
		{
			SpawnItem(i);
		}
	}

	private ZMPlayerItem SpawnItem(int id)
	{
		ZMPlayerItem item;

		if (startTransform == null) { item = ZMPlayerItem.Instantiate(template) as ZMPlayerItem; }
		else { item = ZMPlayerItem.Instantiate(template, startTransform.position, Quaternion.identity) as ZMPlayerItem; }

		var components = item.GetComponents<ZMPlayerItem>();
				
		for (int c = 0; c < components.Length; ++c)
		{
			components[c].ConfigureItemWithID(transform, id);
		}

		return item;
	}
}
