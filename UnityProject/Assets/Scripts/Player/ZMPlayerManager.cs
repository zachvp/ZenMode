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

	public EventHandler OnAllPlayersSpawned;
	public EventHandler<ZMPlayerController> OnPlayerDeath;
	public EventHandler<ZMPlayerController> OnPlayerRespawn;
	public EventHandler<ZMPlayerController> OnPlayerCreate;

	protected ZMPlayerController[] _players;
	protected ZMScoreController[] _scores;
	
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
		if (debug)
		{
			Settings.MatchPlayerCount.value = debugPlayerCount;
		}

		_players = new ZMPlayerController[Settings.MatchPlayerCount.value];
		_scores = new ZMScoreController[Settings.MatchPlayerCount.value];

		// TODO: Should be assert.
		if (_instance != null)
		{
			Debug.LogError("ZMPlayerManager: More than one instance exists in the scene.");
		}

		_instance = this;

		var playerStartpoints = GameObject.FindGameObjectsWithTag(Tags.kPlayerStartPositionTag);
		_playerStartPoints = new Transform[playerStartpoints.Length];
		
		for (int i = 0; i < playerStartpoints.Length; ++i)
		{
			var playerInfo = playerStartpoints[i].GetComponent<ZMPlayerInfo>();
			
			_playerStartPoints[playerInfo.ID] = playerInfo.transform;
		}

		for (int i = 0; i < Settings.MatchPlayerCount.value; ++i)
		{
			var player = ZMPlayerController.Instantiate(playerTemplate) as ZMPlayerController;
			var score = player.GetComponent<ZMScoreController>();
			var input = player.GetComponent<ZMPlayerInputController>();

			player.ConfigureItemWithID(transform, i);
			player.PlayerDeathEvent += SendDeathEvent;
			player.PlayerRespawnEvent += SendRespawnEvent;

			_players[player.PlayerInfo.ID] = player;
			_players[player.PlayerInfo.ID].transform.position = _playerStartPoints[player.PlayerInfo.ID].position;

			_scores[player.PlayerInfo.ID] = score;
			score.ConfigureItemWithID(player.PlayerInfo.ID);

			input.ConfigureItemWithID(player.PlayerInfo.ID);
		}

		Notifier.SendEventNotification(OnAllPlayersSpawned);

		MatchStateManager.OnMatchReset += OnDestroy;
	}

	protected virtual void Start()
	{

	}

	protected virtual void OnDestroy()
	{
		_instance = null;
	}

	private void SendDeathEvent(ZMPlayerController controller)
	{
		Notifier.SendEventNotification(OnPlayerDeath, controller);
	}

	private void SendRespawnEvent(ZMPlayerController controller)
	{
		Notifier.SendEventNotification(OnPlayerRespawn, controller);
	}
}
