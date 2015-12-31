using UnityEngine;
using System.Collections.Generic;
using Core;
using UnityEngine.UI;
using ZMPlayer;
using ZMConfiguration;

public class ZMGameStateController : MonoBehaviour
{
	[SerializeField] private AudioClip audioComplete;
	
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
	private Text _outputText;

	// constants
	private Vector3 outputTextPositionUpOffset = new Vector3 (0, 109, 0);
	
	// Events.
	public EventHandler StartGameEvent;
	public EventHandler ResetGameEvent;
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
				
		foreach (GameObject spawnpointObject in GameObject.FindGameObjectsWithTag(Tags.kSpawnpointTag))
		{
			_spawnpoints.Add(spawnpointObject.transform);
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

	private void HandleAtPathEndEvent (ZMWaypointMovement waypointMovement)
	{
		if (waypointMovement.CompareTag("MainCamera")) { matchState = MatchState.BEGIN_COUNTDOWN; }
	}
	
	private void HandleGameTimerEndedEvent()
	{
		if (matchState != MatchState.POST_MATCH)
		{
			matchState = MatchState.POST_MATCH;
			audio.PlayOneShot(audioComplete, 2.0f);
			_outputText.text = _victoryMessage;
		}
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
	
	private void ClearOutputText()
	{
		_outputText.text = "";
	}

	private void ResetGame()
	{
		Notifier.SendEventNotification(ResetGameEvent);

		Application.LoadLevel(Application.loadedLevel);
	}

	public Vector3 GetSpawnPosition()
	{
		float maximumDistance = float.MinValue;
		int targetIndex = 0;

		for (int i = 0; i < _spawnpoints.Count; i++)
		{
			Transform point = _spawnpoints[i];
			float distance = 0.0f;

			foreach (ZMPlayerController player in ZMPlayerManager.Instance.Players)
			{
				if (!player.IsDead())
				{
					distance += Mathf.Abs(point.position.y - player.transform.position.y) + Mathf.Abs (point.position.x - player.transform.position.x);
				}
			}

			if (distance > maximumDistance)
			{
				maximumDistance = distance;
				targetIndex = i;
			}
		}

		return _spawnpoints[targetIndex].position;
	}

	private void EndGame()
	{
		_outputText.rectTransform.position = outputTextPositionUpOffset;

		if (ZMCrownManager.LeadingPlayerIndex < 0) { _victoryMessage = "DRAW!"; }

		float maxScore = 0.0f;
		
		foreach (ZMScoreController scoreController in ZMPlayerManager.Instance.Scores)
		{
			if (scoreController.TotalScore > maxScore)
			{
				maxScore = scoreController.TotalScore;
				_victoryMessage =  "P" + (scoreController.PlayerInfo.ID + 1) + " WINS!";
			}
		}

		_outputText.text = _victoryMessage;

		MatchStateManager.EndMatch();
		enabled = false;
	}
}
