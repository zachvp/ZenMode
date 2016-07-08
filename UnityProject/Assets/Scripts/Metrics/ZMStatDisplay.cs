using UnityEngine;
using UnityEngine.UI;
using Core;
using ZMPlayer;
using ZMConfiguration;

public class ZMStatDisplay : MonoBehaviour {
	public Text killCount, grassCutCount;

	private ZMPlayerInfo[] _allPlayerInfo;

	void Awake ()
	{
		_allPlayerInfo = new ZMPlayerInfo[Constants.MAX_PLAYERS];

		MatchStateManager.OnMatchEnd += HandleGameEndEvent;

		gameObject.SetActive(false);
	}

	void Start()
	{
		for (int i = 0; i < _allPlayerInfo.Length && i < ZMPlayerManager.Instance.Players.Length; ++i)
		{
			var player = ZMPlayerManager.Instance.Players[i];

			_allPlayerInfo[i] = player.GetComponent<ZMPlayerInfo>();
		}
	}

	void HandleGameEndEvent ()
	{
		int[] maxKills = ZMStatTracker.Kills.GetMax();
		int[] maxGrassCuts = ZMStatTracker.GrassCuts.GetMax();
		
		killCount.text = string.Format("Player {0} kills: {1}", maxKills[0] + 1, maxKills[1]);
		grassCutCount.text = string.Format("Player {0} grass cuts: {1}", maxGrassCuts[0] + 1, maxGrassCuts[1]);

		gameObject.SetActive(true);
	}
}
