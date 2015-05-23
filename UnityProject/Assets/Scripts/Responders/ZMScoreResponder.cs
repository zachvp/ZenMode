using UnityEngine;

public class ZMScoreResponder : MonoBehaviour {
	public bool activeOnScore = true;

	// Use this for initialization
	void Awake () {
		ZMPlayer.ZMScoreController.CanScoreEvent += HandleCanScoreEvent;
		ZMPlayer.ZMScoreController.StopScoreEvent += HandleStopScoreEvent;

		gameObject.SetActive(!activeOnScore);
	}

	void HandleCanScoreEvent(ZMPlayer.ZMScoreController scoreController) {
		gameObject.SetActive(activeOnScore);
	}

	void HandleStopScoreEvent (ZMPlayer.ZMScoreController scoreController) {
		gameObject.SetActive(!activeOnScore);
	}
}
