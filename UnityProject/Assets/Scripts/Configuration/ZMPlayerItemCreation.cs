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

	protected virtual void Awake()
	{
		SpawnItems();
	}

	private void SpawnItems()
	{
		for (int i = 0; i < Settings.MatchPlayerCount.value; ++i)
		{
			var item = ZMPlayerItem.Instantiate(template) as ZMPlayerItem;
			var components = item.GetComponents<ZMPlayerItem>();

			for (int c = 0; c < components.Length; ++c)
			{
				components[c].ConfigureItemWithID(transform, i);
			}
		}
	}
}
