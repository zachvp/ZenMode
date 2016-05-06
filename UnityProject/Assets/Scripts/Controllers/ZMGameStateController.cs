using UnityEngine;
using Core;
using UnityEngine.UI;
using ZMPlayer;
using ZMConfiguration;

public class ZMGameStateController : MonoSingleton<ZMGameStateController>
{
	[SerializeField] private AudioClip audioComplete;
	
	private const float END_GAME_DELAY = 1.0f;

	protected override void Awake()
	{
		base.Awake();

		ZMScoreController.OnReachMaxScore += HandleMaxScoreReached;

		ZMTimedCounterStage.GameTimerEndedEvent += HandleGameTimerEndedEvent;

		ZMCameraOpeningMovement.AtPathEndEvent += HandleAtPathEndEvent;

		MatchStateManager.OnMatchReset += HandleResetGame;
		MatchStateManager.OnMatchExit += HandleSelectQuitEvent;
	}

	private void HandleAtPathEndEvent(ZMWaypointMovement waypointMovement)
	{
		if (waypointMovement.CompareTag(Tags.kMainCamera)) { BeginGame(); }
	}
	
	private void HandleGameTimerEndedEvent()
	{
		Utilities.ExecuteAfterDelay(EndGame, END_GAME_DELAY);

		GetComponent<AudioSource>().PlayOneShot(audioComplete, 2.0f);
	}
	
	private void HandleSelectQuitEvent()
	{	
		SceneManager.LoadScene(ZMSceneIndexList.INDEX_LOBBY);
	}
	
	private void HandleResetGame()
	{
		ResetGame();
	}

	private void HandleMaxScoreReached(ZMPlayerInfo info)
	{
		Utilities.ExecuteAfterDelay(EndGame, END_GAME_DELAY);
//		StartCoroutine(Utilities.ExecuteAfterDelay(EndGame, END_GAME_DELAY));

		GetComponent<AudioSource>().PlayOneShot(audioComplete, 2.0f);
	}
	
	private void BeginGame()
	{		
		MatchStateManager.StartMatch();
	}

	private void ResetGame()
	{
		SceneManager.ResetScene();
	}

	private void EndGame()
	{
		MatchStateManager.EndMatch();
	}
}
