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
	private static GameState _gameState; public static GameState Game_State { get { return _gameState; } }

	public enum MatchState { PRE_MATCH, BEGIN_COUNTDOWN, MATCH, POST_MATCH, GARBAGE };
	private static MatchState _matchState; public static MatchState Match_State { get { return _matchState; } }

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
	
	// delegates
	public static EventHandler<ZMGameStateController, ZMPlayerController> SpawnObjectEvent;

	public static EventHandler StartGameEvent;
	public static EventHandler ResetGameEvent;
	public static EventHandler GameEndEvent;
	public static EventHandler QuitMatchEvent;

	// HACKS!
	private string _victoryMessage;

	void Awake()
	{
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
		SpawnObjectEvent    = null;
		StartGameEvent      = null;
		GameEndEvent	    = null;
		ResetGameEvent 		= null;
		QuitMatchEvent		= null;

		_gameState = GameState.NEUTRAL;
		_matchState = MatchState.PRE_MATCH;
		MatchStateManager.Clear();
	}

	void Update()
	{
		/** State check **/
		if (_matchState == MatchState.PRE_MATCH)
		{
			_outputText.text = "Get Ready";
		}
		else if (_matchState == MatchState.BEGIN_COUNTDOWN)
		{
			BeginGame();
		}
		else if (_matchState == MatchState.POST_MATCH)
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

			_matchState = MatchState.GARBAGE;

			if (!IsInvoking(kEndGameMethodName)) { Invoke(kEndGameMethodName, END_GAME_DELAY); }
		}

		if (_gameState == GameState.RESUME)
		{
			if (_matchState != MatchState.POST_MATCH)
			{
				_gameState = GameState.NEUTRAL;
				_outputText.text = "";
			}
		}
		else if (_gameState == GameState.PAUSE)
		{
			if (_matchState != MatchState.POST_MATCH && _matchState != MatchState.PRE_MATCH)
			{
				_gameState = GameState.PAUSED;

				PauseGame();
			}
		}
		else if (_gameState == GameState.RESET)
		{
			_gameState = GameState.NEUTRAL;
			ResetGame();
		}
	}

	void HandleAtPathEndEvent (ZMWaypointMovement waypointMovement)
	{
		if (waypointMovement.CompareTag("MainCamera")) { _matchState = MatchState.BEGIN_COUNTDOWN; }
	}
	
	void HandleGameTimerEndedEvent()
	{
		if (_matchState != MatchState.POST_MATCH)
		{
			_matchState = MatchState.POST_MATCH;
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
		if (_gameState == GameState.PAUSE || _gameState == GameState.PAUSED)
		{
			_gameState = GameState.RESUME;
		}
		else if (_matchState != MatchState.PRE_MATCH)
		{
			_gameState = GameState.PAUSE;
		}
	}
	
	private void HandleOnMatchResume()
	{
		_gameState = GameState.RESUME;
	}

	private void HandleMaxScoreReached(ZMScoreController scoreController)
	{
		_matchState = MatchState.POST_MATCH;
		audio.PlayOneShot(audioComplete, 2.0f);
	}
	
	private void BeginGame()
	{
		_outputText.text = "Begin!";
		_matchState = MatchState.MATCH;
		
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
		if (_matchState == MatchState.POST_MATCH) { return; }

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
