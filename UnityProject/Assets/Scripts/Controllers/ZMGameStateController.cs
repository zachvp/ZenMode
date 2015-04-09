using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using ZMPlayer;

public class ZMGameStateController : MonoBehaviour {
	public ZMTimedCounter countdownTimer;
	public ZMPlayerController[] players;
	public Text outputText;
	public static int PlayerCount = 2;

	private enum GameState { BEGIN, NEUTRAL, PAUSE, PAUSED, RESUME, RESET }
	GameState _gameState;

	private enum MatchState { PRE_MATCH, BEGIN_COUNTDOWN, COUNTDOWN, MATCH, POST_MATCH };
	private MatchState _matchState;
	private List<Transform> _spawnpoints;
	private int _spawnpointIndex;
	private Queue<GameObject> _objectsToSpawn;

	private const string kSpawnpointTag = "Spawnpoint";

	// delegates
	public delegate void SpawnObjectAction(ZMGameStateController gameStateController, GameObject spawnObject);
	public static event SpawnObjectAction SpawnObjectEvent;

	public delegate void StartGameAction();
	public static event StartGameAction StartGameEvent;

	// Use this for initialization
	void Awake () {
		_matchState = MatchState.PRE_MATCH;
		_gameState  = GameState.NEUTRAL;
		_spawnpoints = new List<Transform>();
		_objectsToSpawn = new Queue<GameObject>();
		_spawnpointIndex = 0;

		foreach (GameObject spawnpointObject in GameObject.FindGameObjectsWithTag(kSpawnpointTag)) {
			_spawnpoints.Add(spawnpointObject.transform);
		}

		// Add delegate handlers
		ZMPlayerController.PlayerDeathEvent += RespawnObject;
		ZMScoreController.MaxScoreReached += MatchWon;
		ZMGameInputManager.StartInputEvent += HandleStartInputEvent;
		ZMGameInputManager.BackInputEvent += HandleBackInputEvent;
	}

	void HandleBackInputEvent ()
	{
		if (_matchState == MatchState.PRE_MATCH) {
			_matchState = MatchState.BEGIN_COUNTDOWN;
		}
		
		if (_gameState == GameState.BEGIN) {
			_gameState = GameState.NEUTRAL;
		} else {
			_gameState = GameState.RESET;
		}
	}

	void HandleStartInputEvent ()
	{
		if (_matchState != MatchState.PRE_MATCH) {
			if (_gameState == GameState.PAUSE || _gameState == GameState.PAUSED) {
				_gameState =  GameState.RESUME;
			} else {
				//_previousState = _matchState;
				_gameState = GameState.PAUSE;
			}
		} else {
			_matchState = MatchState.BEGIN_COUNTDOWN;


			Time.timeScale = 1.0f;
		}
	}

	void Start() {
		outputText.text = "";

		DisableGameObjects();
		DeactivatePedestal();
	}

	void OnDestroy() {
		SpawnObjectEvent = null;
		StartGameEvent   = null;
	}

	void Update() {
		/** State set **/
		/*if (Input.GetKeyDown(KeyCode.Space)) {
			if (_matchState == MatchState.PRE_MATCH) {
				_matchState = MatchState.BEGIN_COUNTDOWN;
			}

			if (_gameState == GameState.BEGIN) {
				_gameState = GameState.NEUTRAL;
			} else {
				_gameState = GameState.RESET;
			}
		} else if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.Return)) {
			if (_matchState != MatchState.PRE_MATCH) {
				if (_gameState == GameState.PAUSE || _gameState == GameState.PAUSED) {
					_gameState =  GameState.RESUME;
				} else {
					//_previousState = _matchState;
					_gameState = GameState.PAUSE;
				}
			}
		}*/

		/** State check **/
		if (_matchState == MatchState.BEGIN_COUNTDOWN) {
			_matchState = MatchState.COUNTDOWN;
			countdownTimer.BeginCount();
		} else if (_matchState == MatchState.POST_MATCH) {
			outputText.text = "Match Ended!";
			PauseGame();
		}

		if (_gameState == GameState.RESUME) {
			if (_matchState != MatchState.POST_MATCH) {
				_gameState = GameState.NEUTRAL;
				outputText.text = "";
				ResumeGame();
			}
		} else if (_gameState == GameState.PAUSE) {
			if (_matchState != MatchState.POST_MATCH) {
				_gameState = GameState.PAUSED;
				outputText.text = "Paused\nPress 'back' to reset";
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
		foreach (ZMPlayerController playerController in players) {
			playerController.DisablePlayer();
		}
	}

	private void EnableGameObjects() {
		foreach (ZMPlayerController playerController in players) {
			playerController.EnablePlayer();
		}
	}

	private void DeactivatePedestal() {
		//_pedestalController.Disable();
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

		GameObject spawnObject = _objectsToSpawn.Dequeue();
		spawnObject.transform.position	= _spawnpoints[_spawnpointIndex].position;

		if (SpawnObjectEvent != null) {
			SpawnObjectEvent(this, spawnObject);
		}
		//DeactivatePedestal();
	}

	// Event handlers
	private void RespawnObject(ZMPlayerController playerController) {
		GameObject respawnObject = playerController.gameObject;
		
		if (!_objectsToSpawn.Contains(respawnObject)) {
			_objectsToSpawn.Enqueue(respawnObject);
			Invoke("SpawnObject", 2.0f);
		}
	}
}
