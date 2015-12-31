using UnityEngine;
using System.Collections.Generic;
using Core;
using UnityEngine.UI;
using ZMPlayer;
using ZMConfiguration;

public class ZMGameStateController : MonoBehaviour
{
	public AudioClip audioComplete;

	private int _playerCount;

	public enum GameState { NEUTRAL, BEGIN, PAUSE, PAUSED, RESUME, RESET };
	public enum MatchState { PRE_MATCH, BEGIN_COUNTDOWN, MATCH, POST_MATCH, GARBAGE };

	public GameState gameState { get; private set; }
	public MatchState matchState { get; private set; }
	
	private const float END_GAME_DELAY = 1.0f;

	private const string kEndGameMethodName		    = "EndGame";
	private const string kClearOutputTextMethodName = "ClearOutputText";
	private const string kSpawnObjectMethodName 	= "SpawnObject";

	// references
	private List<Transform> _spawnpoints;
	private Queue<ZMPlayerController> _objectsToSpawn;
	private List<ZMPlayerController> _players;
	private List<ZMScoreController> _scoreControllers; public List<ZMScoreController> ScoreControllers { get { return _scoreControllers; } }
	private bool _firedGameEndEvent;
	private Text _outputText;

	// constants
	private Vector3 outputTextPositionUpOffset = new Vector3 (0, 109, 0);
	
	// Events.
	public EventHandler<ZMGameStateController, ZMPlayerController> SpawnObjectEvent;

	public EventHandler StartGameEvent;
	public EventHandler ResetGameEvent;
	public EventHandler GameEndEvent;
	public EventHandler QuitMatchEvent;

	// HACKS!
	private string _victoryMessage;

	public static ZMGameStateController Instance
	{
		get
		{
			if (_instance == null) { Debug.LogError("ZMGameStateController: Instance does not exist in scene."); }

			return _instance;
		}
	}

	private static ZMGameStateController _instance;

	void Awake()
	{
		if (_instance != null) { Debug.LogError("ZMGameStateController: Another instance already exists in the scene."); }

		_instance = this;
		_spawnpoints = new List<Transform>();
		_objectsToSpawn = new Queue<ZMPlayerController>(Constants.MAX_PLAYERS);
		_players =  new List<ZMPlayerController>(Constants.MAX_PLAYERS);
		_scoreControllers = new List<ZMScoreController>(Constants.MAX_PLAYERS);

		// Add delegate handlers
		ZMPlayerController.PlayerDeathEvent += RespawnObject;
		ZMPlayerController.PlayerEliminatedEvent += HandlePlayerEliminatedEvent;

		ZMScoreController.MaxScoreReached += HandleMaxScoreReached;

		ZMTimedCounter.GameTimerEndedEvent += HandleGameTimerEndedEvent;

		ZMWaypointMovement.AtPathEndEvent += HandleAtPathEndEvent;

		MatchStateManager.OnMatchPause += HandleOnMatchPause;
		MatchStateManager.OnMatchResume += HandleOnMatchResume;
		MatchStateManager.OnMatchReset += HandleResetGame;
	}

	void Start()
	{
		_outputText = GameObject.FindGameObjectWithTag(Tags.kOutput).GetComponent<Text>();

		_outputText.text = "";
				
		_playerCount = Settings.MatchPlayerCount.value;
		
		foreach (GameObject spawnpointObject in GameObject.FindGameObjectsWithTag(Tags.kSpawnpointTag))
		{
			_spawnpoints.Add(spawnpointObject.transform);
		}
		
		for (int i = 0; i < _playerCount; ++i)
		{
			_players.Add(null);
			_scoreControllers.Add(null);
		}
		
		foreach (GameObject player in GameObject.FindGameObjectsWithTag(Tags.kPlayerTag))
		{
			var playerController = player.GetComponent<ZMPlayerController>();
			var scoreController = player.GetComponent<ZMScoreController>();
			var index = playerController.PlayerInfo.ID;
			
			playerController.gameObject.SetActive(false);
			
			if (index < _playerCount)
			{
				_players[index] = playerController;
				_scoreControllers[index] = scoreController;
			}
		}
		
		for (int i = 0; i < _playerCount; ++i)
		{
			_players[i].gameObject.SetActive(true);
		}
	}
	
	void OnDestroy()
	{
		_instance = null;
		MatchStateManager.Clear();
	}

	void Update()
	{
		/** State check **/
		if (matchState == MatchState.PRE_MATCH)
		{
			_outputText.text = "Get Ready";
		}
		else if (matchState == MatchState.BEGIN_COUNTDOWN)
		{
			BeginGame();
		}
		else if (matchState == MatchState.POST_MATCH)
		{
			float maxScore = 0.0f;

			foreach (ZMScoreController scoreController in _scoreControllers)
			{
				if (scoreController.TotalScore > maxScore)
				{
					maxScore = scoreController.TotalScore;
					_victoryMessage =  "P" + (scoreController.PlayerInfo.ID + 1) + " WINS!";
				}
			}

			matchState = MatchState.GARBAGE;

			if (!IsInvoking(kEndGameMethodName)) { Invoke(kEndGameMethodName, END_GAME_DELAY); }
		}

		if (gameState == GameState.RESUME)
		{
			if (matchState != MatchState.POST_MATCH)
			{
				gameState = GameState.NEUTRAL;
				_outputText.text = "";
			}
		}
		else if (gameState == GameState.PAUSE)
		{
			if (matchState != MatchState.POST_MATCH && matchState != MatchState.PRE_MATCH)
			{
				gameState = GameState.PAUSED;

				PauseGame();
			}
		}
		else if (gameState == GameState.RESET)
		{
			gameState = GameState.NEUTRAL;
			ResetGame();
		}
	}

	void HandleAtPathEndEvent (ZMWaypointMovement waypointMovement)
	{
		if (waypointMovement.CompareTag("MainCamera")) { matchState = MatchState.BEGIN_COUNTDOWN; }
	}
	
	void HandleGameTimerEndedEvent()
	{
		if (matchState != MatchState.POST_MATCH)
		{
			matchState = MatchState.POST_MATCH;
			audio.PlayOneShot(audioComplete, 2.0f);
			_outputText.text = _victoryMessage;
		}
	}
	
	void HandlePlayerEliminatedEvent (ZMPlayerController playerController)
	{
		_players.Remove(playerController);
	}
	
	private void HandleSelectQuitEvent()
	{		
		Notifier.SendEventNotification(QuitMatchEvent);
		
		Application.LoadLevel(ZMSceneIndexList.INDEX_LOBBY);
	}
	
	private void HandleResetGame()
	{
		ResetGame();
	}
	
	private void HandleOnMatchPause()
	{
		if (gameState == GameState.PAUSE || gameState == GameState.PAUSED)
		{
			gameState = GameState.RESUME;
		}
		else if (matchState != MatchState.PRE_MATCH)
		{
			gameState = GameState.PAUSE;
		}
	}
	
	private void HandleOnMatchResume()
	{
		gameState = GameState.RESUME;
	}

	private void HandleMaxScoreReached(ZMScoreController scoreController)
	{
		matchState = MatchState.POST_MATCH;
		audio.PlayOneShot(audioComplete, 2.0f);
	}
	
	private void BeginGame()
	{
		_outputText.text = "Begin!";
		matchState = MatchState.MATCH;
		
		Notifier.SendEventNotification(StartGameEvent);
		MatchStateManager.StartMatch();

		Invoke(kClearOutputTextMethodName, 1.0f);
	}

	private void PauseGame()
	{
		_outputText.rectTransform.position = outputTextPositionUpOffset;
	}
	
	void ClearOutputText()
	{
		_outputText.text = "";
	}

	private void ResetGame()
	{
		Notifier.SendEventNotification(ResetGameEvent);

		Application.LoadLevel(Application.loadedLevel);
	}

	private void SpawnObject()
	{
		if (matchState == MatchState.POST_MATCH) { return; }

		float maximumDistance = float.MinValue;
		int targetIndex = 0;

		for (int i = 0; i < _spawnpoints.Count; i++)
		{
			Transform point = _spawnpoints[i];
			float distance = 0.0f;

			foreach (ZMPlayerController player in _players)
			{
				if (!player.IsDead())
				{
					distance += Mathf.Abs (point.position.y - player.transform.position.y) + Mathf.Abs (point.position.x - player.transform.position.x);
				}
			}

			if (distance > maximumDistance)
			{
				maximumDistance = distance;
				targetIndex = i;
			}
		}

		ZMPlayerController spawnObject = _objectsToSpawn.Dequeue();

		if (spawnObject != null && spawnObject)
		{
			spawnObject.transform.position = _spawnpoints[targetIndex].position;

			Notifier.SendEventNotification(SpawnObjectEvent, this, spawnObject);
		}
	}

	void EndGame()
	{
		_outputText.rectTransform.position = outputTextPositionUpOffset;

		if (ZMCrownManager.LeadingPlayerIndex < 0) { _victoryMessage = "DRAW!"; }

		_outputText.text = _victoryMessage;

		for (int i = 0; i < _playerCount && i < _players.Count; ++i)
		{
			if (i != ZMCrownManager.LeadingPlayerIndex)
			{
				_players[i].gameObject.SetActive(false);
			}
		}
		
		if (!_firedGameEndEvent)
		{
			_firedGameEndEvent = true;
			
			Notifier.SendEventNotification(GameEndEvent);
		}

		enabled = false;
	}

	// Event handlers
	private void RespawnObject(ZMPlayerController playerController)
	{
		if (!_objectsToSpawn.Contains(playerController))
		{
			_objectsToSpawn.Enqueue(playerController);
			Invoke(kSpawnObjectMethodName, 5.0f); // TODO: Read from PlayerManager constant.
		}
	}	
}
