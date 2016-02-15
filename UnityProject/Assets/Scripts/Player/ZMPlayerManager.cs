using UnityEngine;
using Core;
using ZMConfiguration;
using ZMPlayer;

public class ZMPlayerManager : MonoBehaviour
{
	[SerializeField] private ZMPlayerController playerTemplate;

	public bool debug = false;
	public int debugPlayerCount = 2;

	public ZMPlayerController[] Players { get { return _players; } }
	public ZMScoreController[] Scores { get { return _scores; } }
	public Transform[] PlayerStartPoints { get { return _playerStartPoints; } }
	public static int LatestJoinIndex  { get { return _latestJoinIndex; } }

	// These should be moved int a StagePlayerManager class.
	public EventHandler OnAllPlayersSpawned;

	protected ZMPlayerController[] _players;
	protected ZMScoreController[] _scores;

	private static int _latestJoinIndex;
	
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
		if (debug) { Settings.MatchPlayerCount.value = debugPlayerCount; }

		ConfigureMonoSingleton();
		InitPlayerData(Settings.MatchPlayerCount.value);
		GetPlayerStartpoints();

		for (int i = 0; i < Settings.MatchPlayerCount.value; ++i) { CreatePlayer(i); }

		Notifier.SendEventNotification(OnAllPlayersSpawned);

		MatchStateManager.OnMatchReset += OnDestroy;
	}

	protected virtual void OnDestroy()
	{
		_instance = null;
	}

	// Allocate space for player array data.
	protected virtual void InitPlayerData(int size)
	{
		_players = new ZMPlayerController[size];
		_scores = new ZMScoreController[size];
	}

	protected void ConfigureMonoSingleton()
	{
		// TODO: Should be assert.
		if (_instance != null)
		{
			Debug.LogError("ZMPlayerManager: More than one instance exists in the scene.");
		}
		
		_instance = this;
	}

	protected ZMPlayerController CreatePlayer(int id)
	{
		var player = ZMPlayerController.Instantiate(playerTemplate) as ZMPlayerController;
		var score = player.GetComponent<ZMScoreController>();
		var input = player.GetComponent<ZMPlayerInputController>();
		
		player.ConfigureItemWithID(transform, id);
		
		_players[player.PlayerInfo.ID] = player;
		_players[player.PlayerInfo.ID].transform.position = _playerStartPoints[player.PlayerInfo.ID].position;
		
		_scores[player.PlayerInfo.ID] = score;
		score.ConfigureItemWithID(player.PlayerInfo.ID);
		
		input.ConfigureItemWithID(player.PlayerInfo.ID);
		_latestJoinIndex = id;

		return player;
	}

	protected void GetPlayerStartpoints()
	{
		var playerStartpoints = GameObject.FindGameObjectsWithTag(Tags.kPlayerStartPositionTag);
		_playerStartPoints = new Transform[playerStartpoints.Length];
		
		for (int i = 0; i < playerStartpoints.Length; ++i)
		{
			var playerInfo = playerStartpoints[i].GetComponent<ZMPlayerInfo>();
			
			_playerStartPoints[playerInfo.ID] = playerInfo.transform;
		}
	}
}
