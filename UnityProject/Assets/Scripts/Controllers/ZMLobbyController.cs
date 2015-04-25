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

	private bool[] _joinedPlayers;

	void Awake() {
		_currentJoinCount = 0;
		_currentReadyCount = 0;
		_paused = false;
		_joinedPlayers = new bool[4];

		ZMLobbyScoreController.MaxScoreReachedEvent += PlayerReady;

		ZMGameInputManager.StartInputEvent += HandleStartInputEvent;
		ZMGameInputManager.MainInputEvent += HandleMainInputEvent;
		ZMPauseMenuController.SelectResumeEvent += HandleSelectResumeEvent;
		ZMPauseMenuController.SelectQuitEvent += HandleSelectQuitEvent;
	}

	void HandleSelectQuitEvent ()
	{
		Application.LoadLevel(1);
	}

	void HandleMainInputEvent (ZMPlayerInfo.PlayerTag playerTag)
	{
		int playerIndex;

		if (playerTag.Equals(ZMPlayerInfo.PlayerTag.PLAYER_1)) {
			playerIndex = 1;
		} else if (playerTag.Equals(ZMPlayerInfo.PlayerTag.PLAYER_2)) {
			playerIndex = 2;
		} else if (playerTag.Equals(ZMPlayerInfo.PlayerTag.PLAYER_3)) {
			playerIndex = 3;
		} else {
			playerIndex = 4;
		}

		if (!_joinedPlayers[playerIndex]) {
			_currentJoinCount += 1;
			
			if (PlayerJoinedEvent != null) {
				PlayerJoinedEvent(playerTag);
			}

			_joinedPlayers[playerIndex] = true;
		}
	}

	void HandleSelectResumeEvent ()
	{
		_paused = false;
	}

	void OnDestroy() {
		PlayerJoinedEvent = null;
		PauseGameEvent    = null;
	}

	void HandleStartInputEvent (ZMPlayerInfo.PlayerTag playerTag)
	{
		int playerIndex;
		
		if (playerTag.Equals(ZMPlayerInfo.PlayerTag.PLAYER_1)) {
			playerIndex = 1;
		} else if (playerTag.Equals(ZMPlayerInfo.PlayerTag.PLAYER_2)) {
			playerIndex = 2;
		} else if (playerTag.Equals(ZMPlayerInfo.PlayerTag.PLAYER_3)) {
			playerIndex = 3;
		} else {
			playerIndex = 4;
		}

		if (_joinedPlayers[playerIndex]) {
			if (!_paused) {
				if (PauseGameEvent != null) {
					PauseGameEvent();

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
