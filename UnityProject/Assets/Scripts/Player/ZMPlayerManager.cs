using UnityEngine;
using Core;
using ZMConfiguration;
using ZMPlayer;

public class ZMPlayerManager : MonoBehaviour
{
	public bool debug = false;
	public int debugPlayerCount = 2;

	public ZMPlayerController[] Players { get { return _players; } }
	public Transform[] PlayerStartPoints { get { return _playerStartPoints; } }
		
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

	private Transform[] _playerStartPoints;
	
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

		_playerStartPoints = new Transform[Constants.MAX_PLAYERS];
		MatchStateManager.OnMatchReset += OnDestroy;
	}

	protected virtual void Start()
	{
		var playerStartpoints = GameObject.FindGameObjectsWithTag(Tags.kPlayerStartPositionTag);
		
		for (int i = 0; i < playerStartpoints.Length; ++i)
		{
			var playerInfo = playerStartpoints[i].GetComponent<ZMPlayerInfo>();

			_playerStartPoints[playerInfo.ID] = playerInfo.transform;
		}
	}

	protected virtual void OnDestroy()
	{
		_instance = null;
	}

	public void AddPlayer(ZMPlayerController player)
	{
		_players[player.PlayerInfo.ID] = player;
	}
}
