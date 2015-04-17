using UnityEngine;
using System.Collections.Generic;
using ZMPlayer;

public class ZMLobbyController : MonoBehaviour {
	public int requiredPlayerCount = 1;

	public delegate void PlayerJoinedAction(ZMPlayer.ZMPlayerInfo.PlayerTag playerTag);
	public static event PlayerJoinedAction PlayerJoinedEvent;

	// how many clients have joined
	private int _currentSignalCount;

	void Awake() {
		_currentSignalCount = 0;

		ZMLobbyScoreController.MaxScoreReachedEvent += PlayerReady;

		ZMGameInputManager.StartInputEvent += HandleStartInputEvent;
	}

	void OnDestroy() {
		PlayerJoinedEvent = null;
	}

	void HandleStartInputEvent (ZMPlayerInfo.PlayerTag playerTag)
	{
		if (PlayerJoinedEvent != null) {
			PlayerJoinedEvent(playerTag);
		}
	}

	private void PlayerReady(ZMLobbyScoreController scoreController) {
		_currentSignalCount += 1;

		if(_currentSignalCount == requiredPlayerCount) {
			Invoke("LoadLevel", 1.0f);
		}
	}

	void LoadLevel() {
		Application.LoadLevel(3);
	}
}
