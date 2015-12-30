using UnityEngine;
using ZMConfiguration;
using ZMPlayer;

// This is a general class for creating player-associated objects and configuring them accordingly.
public class ZMPlayerItemCreation : MonoBehaviour
{
	// The object to create.
	[SerializeField] private ZMPlayerItem template;

	void Awake()
	{
		for (int i = 0; i < Settings.MatchPlayerCount.value; ++i)
		{
			var item = ZMPlayerItem.Instantiate(template) as ZMPlayerItem;

			item.GetComponent<ZMPlayerInfo>().ID = i;
		}
	}
}
