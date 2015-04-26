﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using ZMPlayer;

public class ZMGameStateController : MonoBehaviour {
	public ZMTimedCounter countdownTimer;
	public Text outputText;
	public static int PlayerCount = 2;

	private enum GameState { BEGIN, NEUTRAL, PAUSE, PAUSED, RESUME, RESET }
	GameState _gameState;

	private enum MatchState { PRE_MATCH, BEGIN_COUNTDOWN, COUNTDOWN, MATCH, POST_MATCH };
	private MatchState _matchState;
	private List<Transform> _spawnpoints;
	private int _spawnpointIndex;
	private int _pausedPlayer;
	private Queue<ZMPlayerController> _objectsToSpawn;
	private List<ZMPlayerController> _players;
	private bool _firedGameEndEvent;
	private const string kSpawnpointTag = "Spawnpoint";
	private const string kPlayerTag 	= "Player";

	// delegates
	public delegate void SpawnObjectAction(ZMGameStateController gameStateController, ZMPlayerController spawnObject); public static event SpawnObjectAction SpawnObjectEvent;
	public delegate void StartGameAction(); public static event StartGameAction StartGameEvent;
	public delegate void PauseGameAction(); public static event PauseGameAction PauseGameEvent;
	public delegate void ResumeGameAction(); public static event ResumeGameAction ResumeGameEvent;
	public delegate void GameEndAction();   public static event GameEndAction   GameEndEvent;
	public delegate void BeginCountdownAction(); public static event BeginCountdownAction BeginCountdownEvent;


	// Use this for initialization
	void Awake () {
		_matchState = MatchState.PRE_MATCH;
		_gameState  = GameState.NEUTRAL;
		_spawnpoints = new List<Transform>();
		_objectsToSpawn = new Queue<ZMPlayerController>(4);
		_players =  new List<ZMPlayerController>(4);
		_spawnpointIndex = 0;

		foreach (GameObject spawnpointObject in GameObject.FindGameObjectsWithTag(kSpawnpointTag)) {
			_spawnpoints.Add(spawnpointObject.transform);
		}

		foreach (GameObject player in GameObject.FindGameObjectsWithTag(kPlayerTag)) {
			_players.Add(player.GetComponent<ZMPlayerController>());
		}

		// Add delegate handlers
		ZMPlayerController.PlayerDeathEvent += RespawnObject;
		ZMPlayerController.PlayerEliminatedEvent += HandlePlayerEliminatedEvent;

		ZMScoreController.MaxScoreReached += MatchWon;

		ZMGameInputManager.StartInputEvent += HandleStartInputEvent;

		ZMLobbyPedestalController.FullPathCycleEvent += HandleFullPathCycleEvent;

		ZMPauseMenuController.SelectResumeEvent += HandleSelectResumeEvent;
		ZMPauseMenuController.SelectRestartEvent += HandleSelectRestartEvent;
		ZMPauseMenuController.SelectQuitEvent += HandleSelectQuitEvent;
	}

	void HandlePlayerEliminatedEvent (ZMPlayerController playerController)
	{
		_players.Remove(playerController);
	}

	void HandleSelectQuitEvent ()
	{
		Time.timeScale = 1.0f;
		Application.LoadLevel(1);
	}

	void HandleSelectRestartEvent ()
	{
		_gameState =  GameState.RESET;
	}

	void HandleSelectResumeEvent ()
	{
		_gameState =  GameState.RESUME;
	}

	void HandleFullPathCycleEvent (ZMLobbyPedestalController lobbyPedestalController)
	{
		if (_matchState == MatchState.PRE_MATCH) {
			_matchState = MatchState.BEGIN_COUNTDOWN;
			
			Time.timeScale = 1.0f;
		}
	}

	void HandleStartInputEvent (ZMPlayerInfo.PlayerTag playerTag)
	{
		if (_matchState == MatchState.PRE_MATCH) {
			_matchState = MatchState.BEGIN_COUNTDOWN;
		} else if (_gameState == GameState.PAUSE || _gameState == GameState.PAUSED) {
			_gameState =  GameState.RESUME;
		} else if (_matchState != MatchState.COUNTDOWN) {
			_pausedPlayer = (int)playerTag;
			_gameState = GameState.PAUSE;
		}
	}

	void Start() {
		outputText.text = "";

		DisableGameObjects();
		Time.timeScale = 1.0f;
	}

	void OnDestroy() {
		SpawnObjectEvent    = null;
		StartGameEvent      = null;
		PauseGameEvent	    = null;
		GameEndEvent	    = null;
		BeginCountdownEvent = null;
		ResumeGameEvent 	= null;
	}

	void Update() {
		/** State check **/
		if (_matchState == MatchState.BEGIN_COUNTDOWN) {
			_matchState = MatchState.COUNTDOWN;
			countdownTimer.BeginCount();
		} else if (_matchState == MatchState.POST_MATCH) {
			Invoke("EndGame", 3.0f);
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
			if (_matchState != MatchState.POST_MATCH) {
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

	// public methods
	public void CountdownEnded() {
		_matchState = MatchState.MATCH;
		EnableGameObjects();

		if (StartGameEvent != null) {
			StartGameEvent();
		}
	}

	public void MatchWon(ZMScoreController scoreController) {
		_matchState = MatchState.POST_MATCH;
	}

	// Private methods
	private void DisableGameObjects() {
		foreach (ZMPlayerController playerController in _players) {
			playerController.DisablePlayer();
		}
	}

	private void EnableGameObjects() {
		foreach (ZMPlayerController playerController in _players) {
			if (playerController.gameObject != null) {
				playerController.EnablePlayer();
			}
		}
	}

	private void PauseGame() {
		DisableGameObjects();
		countdownTimer.GetComponent<Text>().enabled = false;

		Time.timeScale = 0.0f;
	}

	private void ResumeGame() {
		EnableGameObjects();
		countdownTimer.GetComponent<Text>().enabled = true;

		Time.timeScale = 1.0f;
	}

	private void ResetGame() {
		Application.LoadLevel(Application.loadedLevelName);
	}

	private void SpawnObject() {
		_spawnpointIndex += 1;
		_spawnpointIndex %= 4;

		ZMPlayerController spawnObject = _objectsToSpawn.Dequeue();
		spawnObject.transform.position	= _spawnpoints[_spawnpointIndex].position;

		if (SpawnObjectEvent != null && spawnObject != null) {
			SpawnObjectEvent(this, spawnObject);
		}
	}

	void EndGame() {
		outputText.text = "Match Ended!";
		PauseGame();
		
		if (GameEndEvent != null && !_firedGameEndEvent) {
			_firedGameEndEvent = true;
			
			GameEndEvent();
		}
	}

	// Event handlers
	private void RespawnObject(ZMPlayerController playerController) {
		if (!_objectsToSpawn.Contains(playerController)) {
			_objectsToSpawn.Enqueue(playerController);
			Invoke("SpawnObject", 2.0f);
		}
	}
}
