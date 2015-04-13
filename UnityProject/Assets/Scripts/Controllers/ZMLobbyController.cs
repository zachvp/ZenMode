using UnityEngine;
using System.Collections.Generic;
using ZMPlayer;

public class ZMLobbyController : MonoBehaviour {
	public int requiredPlayerCount = 1;

	// how many clients have joined
	private int _currentSignalCount;

	void Awake() {
		_currentSignalCount = 0;
		ZMLobbyScoreController.MaxScoreReachedEvent += PlayerReady;
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
