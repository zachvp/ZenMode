using UnityEngine;
using Core;
using ZMConfiguration;
using ZMPlayer;

public class ZMPlayerManager : MonoSingleton<ZMPlayerManager>
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
	
	private Transform[] _playerStartPoints;
	
	protected override void Awake()
	{
		base.Awake();

		Init();
	}

	// Handles all initializing for this class and sublasses.
	protected virtual void Init()
	{
		if (debug) { ConfigureDebugSettings(); }

		InitPlayerData(Settings.MatchPlayerCount.value);
		CreateAllPlayers();
		Notifier.SendEventNotification(OnAllPlayersSpawned);

		MatchStateManager.OnMatchReset += OnDestroy;
	}

	// Allocate space for player array data.
	protected virtual void InitPlayerData(int size)
	{
		_players = new ZMPlayerController[size];
		_scores = new ZMScoreController[size];

		InitPlayerStartpoints();
	}

	protected virtual void CreateAllPlayers()
	{
		for (int i = 0; i < Settings.MatchPlayerCount.value; ++i) { CreatePlayer(i); }
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

	protected void InitPlayerStartpoints()
	{
		var playerStartpoints = GameObject.FindGameObjectsWithTag(Tags.kPlayerStartPositionTag);
		_playerStartPoints = new Transform[playerStartpoints.Length];
		
		for (int i = 0; i < playerStartpoints.Length; ++i)
		{
			var playerInfo = playerStartpoints[i].GetComponent<ZMPlayerInfo>();
			
			_playerStartPoints[playerInfo.ID] = playerInfo.transform;
		}
	}

	private void ConfigureDebugSettings()
	{
		Settings.MatchPlayerCount.value = debugPlayerCount;
	}
}
