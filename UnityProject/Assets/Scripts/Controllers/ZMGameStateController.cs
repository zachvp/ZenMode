using UnityEngine;
using Core;
using UnityEngine.UI;
using ZMPlayer;
using ZMConfiguration;

public class ZMGameStateController : MonoBehaviour
{
	[SerializeField] private AudioClip audioComplete;
	
	private const float END_GAME_DELAY = 1.0f;
	
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

		ZMScoreController.MaxScoreReached += HandleMaxScoreReached;

		ZMTimedCounter.GameTimerEndedEvent += HandleGameTimerEndedEvent;

		ZMCameraOpeningMovement.AtPathEndEvent += HandleAtPathEndEvent;

		MatchStateManager.OnMatchReset += HandleResetGame;
		MatchStateManager.OnMatchExit += HandleSelectQuitEvent;
	}
	
	void OnDestroy()
	{
		_instance = null;
		MatchStateManager.Clear();
	}

	private void HandleAtPathEndEvent(ZMWaypointMovement waypointMovement)
	{
		if (waypointMovement.CompareTag(Tags.kMainCamera)) { BeginGame(); }
	}
	
	private void HandleGameTimerEndedEvent()
	{
		StartCoroutine(Utilities.ExecuteAfterDelay(EndGame, END_GAME_DELAY));

		audio.PlayOneShot(audioComplete, 2.0f);
	}
	
	private void HandleSelectQuitEvent()
	{		
		Application.LoadLevel(ZMSceneIndexList.INDEX_LOBBY);
	}
	
	private void HandleResetGame()
	{
		ResetGame();
	}

	private void HandleMaxScoreReached(ZMScoreController scoreController)
	{
		StartCoroutine(Utilities.ExecuteAfterDelay(EndGame, END_GAME_DELAY));

		audio.PlayOneShot(audioComplete, 2.0f);
	}
	
	private void BeginGame()
	{		
		MatchStateManager.StartMatch();
	}

	private void ResetGame()
	{
		Application.LoadLevel(Application.loadedLevel);
	}

	private void EndGame()
	{
		MatchStateManager.EndMatch();
	}
}
