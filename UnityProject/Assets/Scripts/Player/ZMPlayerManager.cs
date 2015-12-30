using UnityEngine;
using Core;
using ZMConfiguration;
using ZMPlayer;

public class ZMPlayerManager : MonoBehaviour
{
	public bool debug = false;
	public int debugPlayerCount = 2;

	public ZMPlayerController[] Players { get { return _players; } }
		
	protected ZMPlayerController[] _players = new ZMPlayerController[Constants.MAX_PLAYERS];

	public static ZMPlayerManager Instance
	{
		get
		{
			// TODO: Should be assert.
			if (_instance == null)
			{
				Debug.LogError("ZMPlayerManager: no instance exists in the scene.");
			}

			return _instance;
		}
	}

	protected static ZMPlayerManager _instance;
	
	protected virtual void Awake()
	{
		// TODO: Should be assert.
		if (_instance != null)
		{
			Debug.LogError("ZMPlayerManager: More than one instance exists in the scene.");
		}

		_instance = this;

		if (debug)
		{
			Settings.MatchPlayerCount.value = debugPlayerCount;
		}

		MatchStateManager.OnMatchReset += OnDestroy;
	}

	protected virtual void OnDestroy()
	{
		_instance = null;
	}

	public void AddPlayer(ZMPlayerController player)
	{
		var playerTag = player.GetComponent<ZMPlayerInfo>().playerTag;

		_players[(int) playerTag] = player;
	}
}
