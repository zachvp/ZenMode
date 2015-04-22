using UnityEngine;
using System.Collections.Generic;
using ZMPlayer;

public class ZMLobbyController : MonoBehaviour {
	public int requiredPlayerCount = 1;

	public delegate void PlayerJoinedAction(ZMPlayer.ZMPlayerInfo.PlayerTag playerTag); public static event PlayerJoinedAction PlayerJoinedEvent;
	public delegate void PauseGameAction(); public static event PauseGameAction PauseGameEvent;

	// how many clients have readied
	private int _currentJoinCount;
	private int _currentReadyCount;

	private bool _paused;

	void Awake() {
		_currentJoinCount = 0;
		_currentReadyCount = 0;
		_paused = false;

		ZMLobbyScoreController.MaxScoreReachedEvent += PlayerReady;

		ZMGameInputManager.StartInputEvent += HandleStartInputEvent;
		ZMPauseMenuController.SelectResumeEvent += HandleSelectResumeEvent;
	}

	void HandleSelectResumeEvent ()
	{
		Time.timeScale = 1;
		_paused = false;
	}

	void OnDestroy() {
		PlayerJoinedEvent = null;
		PauseGameEvent    = null;
	}

	void HandleStartInputEvent (ZMPlayerInfo.PlayerTag playerTag)
	{
		_currentJoinCount += 1;

		if (PlayerJoinedEvent != null) {
			PlayerJoinedEvent(playerTag);
		}

		if (_currentJoinCount >= requiredPlayerCount) {
			if (!_paused) {
				if (PauseGameEvent != null) {
					PauseGameEvent();

					Time.timeScale = 0;

					_paused = true;
				}
			}
		}
	}

	private void PlayerReady(ZMLobbyScoreController scoreController) {
		_currentReadyCount += 1;

		if(_currentReadyCount == requiredPlayerCount) {
			Invoke("LoadLevel", 3.0f);
		}
	}

	void LoadLevel() {
		Application.LoadLevel(3);
	}
}
