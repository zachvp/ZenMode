using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using ZMPlayer;

public class ZMGameStateController : MonoBehaviour {
	public Text outputText;
	private int _playerCount;

	private enum GameState { BEGIN, NEUTRAL, PAUSE, PAUSED, RESUME, RESET }
	GameState _gameState;

	private enum MatchState { PRE_MATCH, BEGIN_COUNTDOWN, MATCH, POST_MATCH };
	private MatchState _matchState;

	// references
	private List<Transform> _spawnpoints;
	private int _pausedPlayer;
	private Queue<ZMPlayerController> _objectsToSpawn;
	private List<ZMPlayerController> _players;
	private List<ZMScoreController> _scoreControllers; public List<ZMScoreController> ScoreControllers { get { return _scoreControllers; } }
	private bool _firedGameEndEvent;

	// constants
	private const string kSpawnpointTag = "Spawnpoint";
	private const string kPlayerTag 	= "Player";
	
	private Vector3 outputTextPositionUpOffset = new Vector3 (0, 109, 0);

	private const float END_GAME_DELAY = 1.0f;

	// delegates
	public delegate void SpawnObjectAction(ZMGameStateController gameStateController, ZMPlayerController spawnObject); public static event SpawnObjectAction SpawnObjectEvent;
	public delegate void StartGameAction(); public static event StartGameAction StartGameEvent;
	public delegate void PauseGameAction(); public static event PauseGameAction PauseGameEvent;
	public delegate void ResumeGameAction(); public static event ResumeGameAction ResumeGameEvent;
	public delegate void ResetGameAction(); public static event ResetGameAction ResetGameEvent;
	public delegate void GameEndAction();   public static event GameEndAction GameEndEvent;
	public delegate void QuitMatchAction(); public static event QuitMatchAction QuitMatchEvent;

	// pause menu handling
	private int RESUME_OPTION  = 0;
	private int RESTART_OPTION = 1;
	private int QUIT_OPTION	   = 2;

	// HACKS!
	private string _victoryMessage;

	// Use this for initialization
	void Awake () {
		_matchState = MatchState.PRE_MATCH;
		_gameState  = GameState.NEUTRAL;
		_spawnpoints = new List<Transform>();
		_objectsToSpawn = new Queue<ZMPlayerController>(ZMPlayerManager.MAX_PLAYERS);
		_players =  new List<ZMPlayerController>(ZMPlayerManager.MAX_PLAYERS);
		_scoreControllers = new List<ZMScoreController>(ZMPlayerManager.MAX_PLAYERS);

		// Add delegate handlers
		ZMPlayerController.PlayerDeathEvent += RespawnObject;
		ZMPlayerController.PlayerEliminatedEvent += HandlePlayerEliminatedEvent;

		ZMScoreController.MaxScoreReached += HandleMaxScoreReached;

		ZMGameInputManager.StartInputEvent += HandleStartInputEvent;

		ZMMenuOptionController.SelectOptionEvent += HandleSelectOptionEvent;
		ZMTimedCounter.GameTimerEndedEvent += HandleGameTimerEndedEvent;
		ZMWaypointMovement.AtPathEndEvent += HandleAtPathEndEvent;
	}

	void Start() {
		outputText.text = "";
		
		Time.timeScale = 1.0f;
		
		_playerCount = ZMPlayerManager.PlayerCount;
		
		foreach (GameObject spawnpointObject in GameObject.FindGameObjectsWithTag(kSpawnpointTag)) {
			_spawnpoints.Add(spawnpointObject.transform);
		}
		
		for (int i = 0; i < _playerCount; ++i) {
			_players.Add(null);
			_scoreControllers.Add(null);
		}
		
		foreach (GameObject player in GameObject.FindGameObjectsWithTag(kPlayerTag)) {
			ZMPlayerController playerController = player.GetComponent<ZMPlayerController>();
			ZMScoreController scoreController = player.GetComponent<ZMScoreController>();
			
			int index = (int) playerController.PlayerInfo.playerTag;
			
			playerController.gameObject.SetActive(false);
			
			if (index < _playerCount) {
				_players[index] = playerController;
				_scoreControllers[index] = scoreController;
			}
		}
		
		for (int i = 0; i < _playerCount; ++i) {
			_players[i].gameObject.SetActive(true);
		}

		DisableGameObjects();
	}
	
	void OnDestroy() {
		SpawnObjectEvent    = null;
		StartGameEvent      = null;
		PauseGameEvent	    = null;
		GameEndEvent	    = null;
		ResetGameEvent 		= null;
		ResumeGameEvent 	= null;
		QuitMatchEvent		= null;
	}

	void HandleAtPathEndEvent (ZMWaypointMovement waypointMovement)
	{
		if (waypointMovement.CompareTag("MainCamera"))
			_matchState = MatchState.BEGIN_COUNTDOWN;
	}

	void HandleGameTimerEndedEvent ()
	{
		if (_matchState != MatchState.POST_MATCH) {
			_matchState = MatchState.POST_MATCH;
			outputText.text = _victoryMessage;
		}
	}

	void HandlePlayerEliminatedEvent (ZMPlayerController playerController)
	{
		_players.Remove(playerController);
	}

	void HandleSelectOptionEvent(int optionIndex) {
		if (optionIndex == RESUME_OPTION) {
			HandleSelectResumeEvent();
		} else if (optionIndex == RESTART_OPTION) {
			HandleSelectRestartEvent();
		} else if (optionIndex == QUIT_OPTION) {
			HandleSelectQuitEvent();
		}
	}

	void HandleSelectResumeEvent ()
	{
		_gameState =  GameState.RESUME;
	}
	
	void HandleSelectQuitEvent ()
	{
		Time.timeScale = 1.0f;

		if (QuitMatchEvent != null) {
			QuitMatchEvent();
		}

		Application.LoadLevel(ZMSceneIndexList.INDEX_LOBBY);
	}

	void HandleSelectRestartEvent ()
	{
		ResetGame();
	}

	void HandleStartInputEvent (ZMPlayerInfo.PlayerTag playerTag)
	{
		if (_gameState == GameState.PAUSE || _gameState == GameState.PAUSED) {
			_gameState =  GameState.RESUME;
		} else if (_matchState != MatchState.PRE_MATCH) {
			_pausedPlayer = (int) playerTag;
			_gameState = GameState.PAUSE;
		}
	}

	void Update() {
		/** State check **/
		if (_matchState == MatchState.PRE_MATCH) {
			outputText.text = "Get Ready";
		} else if (_matchState == MatchState.BEGIN_COUNTDOWN) {
			BeginGame();
		} else if (_matchState == MatchState.POST_MATCH) {
			float maxScore = 0.0f;

			foreach (ZMScoreController scoreController in _scoreControllers) {
				if (scoreController.TotalScore > maxScore) {
					maxScore = scoreController.TotalScore;
					_victoryMessage =  "P" + (int) (scoreController.PlayerInfo.playerTag + 1) + " WINS!";
				}
			}

			if (!IsInvoking("EndGame"))
				Invoke("EndGame", END_GAME_DELAY);
		}

		if (_gameState == GameState.RESUME) {
			if (_matchState != MatchState.POST_MATCH) {
				_gameState = GameState.NEUTRAL;
				outputText.text = "";

				if (ResumeGameEvent != null) {
					ResumeGameEvent();
				}

				ResumeGame();
			}
		} else if (_gameState == GameState.PAUSE) {
			if (_matchState != MatchState.POST_MATCH && _matchState != MatchState.PRE_MATCH) {
				_gameState = GameState.PAUSED;
				outputText.text = "P" + (_pausedPlayer + 1).ToString() + " PAUSED";

				if (PauseGameEvent != null) {
					PauseGameEvent();
				}

				PauseGame();
			}
		} else if (_gameState == GameState.RESET) {
			_gameState = GameState.NEUTRAL;
			ResetGame();
		}
	}

	private void HandleMaxScoreReached(ZMScoreController scoreController) {
		_matchState = MatchState.POST_MATCH;
	}

	// Private methods
	private void DisableGameObjects() {
		foreach (ZMPlayerController playerController in _players) {
			if (playerController.gameObject != null) {
				playerController.DisablePlayer();
			}
		}
	}

	private void EnableGameObjects() {
		foreach (ZMPlayerController playerController in _players) {
			if (playerController.gameObject != null) {
				playerController.EnablePlayer();
			}
		}
	}
	
	private void BeginGame() {
		outputText.text = "Begin!";
		_matchState = MatchState.MATCH;
		EnableGameObjects();
		
		if (StartGameEvent != null) {
			StartGameEvent();
		}

		Invoke("ClearOutputText", 1.0f);
	}

	private void PauseGame() {
		outputText.rectTransform.position = outputTextPositionUpOffset;
		DisableGameObjects();

		Time.timeScale = 0.0f;
	}

	private void ResumeGame() {
		EnableGameObjects();

		Time.timeScale = 1.0f;
	}

	void ClearOutputText() {
		outputText.text = "";
	}

	private void ResetGame() {
		Application.LoadLevel(Application.loadedLevelName);

		if (ResetGameEvent != null) {
			ResetGameEvent();
		}
	}

	private void SpawnObject() {
		if (_matchState == MatchState.POST_MATCH) return;

		float maximumDistance = float.MinValue;
		int targetIndex = 0;

		for (int i = 0; i < _spawnpoints.Count; i++) {
			Transform point = _spawnpoints[i];
			float distance = 0.0f;
			foreach (ZMPlayerController player in _players) {
				if (!player.IsDead()) {
					distance += Mathf.Abs (point.position.y - player.transform.position.y) + Mathf.Abs (point.position.x - player.transform.position.x);
				}
			}

			if (distance > maximumDistance) {
				maximumDistance = distance;
				targetIndex = i;
			}
		}

		ZMPlayerController spawnObject = _objectsToSpawn.Dequeue();

		if (spawnObject != null && spawnObject) {
			spawnObject.transform.position = _spawnpoints[targetIndex].position;

			if (SpawnObjectEvent != null) {
				SpawnObjectEvent(this, spawnObject);
			}
		}
	}

	void EndGame() {
		outputText.text = _victoryMessage;

		DisableGameObjects();

		// Vulgar hacks
		RESUME_OPTION  = -1;
		RESTART_OPTION = 0;
		QUIT_OPTION    = 1;
		
		if (GameEndEvent != null && !_firedGameEndEvent) {
			_firedGameEndEvent = true;
			
			GameEndEvent();
		}

		ZMMenuOptionController.SelectOptionEvent += HandleSelectOptionEvent;
	}

	// Event handlers
	private void RespawnObject(ZMPlayerController playerController) {
		if (!_objectsToSpawn.Contains(playerController)) {
			_objectsToSpawn.Enqueue(playerController);
			Invoke("SpawnObject", 5.0f);
		}
	}
}
