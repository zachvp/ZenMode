using UnityEngine;
using System.Collections.Generic;
using ZMPlayer;

public class ZMLobbyController : MonoBehaviour {
	public int requiredPlayerCount = 2;

	public delegate void PlayerJoinedAction(ZMPlayer.ZMPlayerInfo.PlayerTag playerTag); public static event PlayerJoinedAction PlayerJoinedEvent;
	public delegate void PlayerReadyAction(ZMPlayer.ZMPlayerInfo.PlayerTag playerTag); public static event PlayerReadyAction PlayerReadyEvent;
	public delegate void PauseGameAction(); public static event PauseGameAction PauseGameEvent;

	// how many clients have readied
	private int _currentJoinCount; // i.e. how many  have pressed a button to join
	private int _currentReadyCount; // i.e. how many have actually readied up

	private bool _paused;

	private bool[] _joinedPlayers;

	void Awake() {
		_currentJoinCount = 0;
		_currentReadyCount = 0;
		_paused = false;
		_joinedPlayers = new bool[4];

		ZMLobbyScoreController.MaxScoreReachedEvent += HandleMaxScoreReachedEvent;

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
		PlayerReadyEvent  = null;
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

	private void HandleMaxScoreReachedEvent(ZMLobbyScoreController scoreController) {
		_currentReadyCount += 1;

		if (PlayerReadyEvent != null) {
			PlayerReadyEvent(scoreController.PlayerInfo.playerTag);
		}

		if(_currentReadyCount == requiredPlayerCount) {
			Invoke("LoadLevel", 3.0f);
		}
	}

	void LoadLevel() {
		Application.LoadLevel(3);
	}
}
