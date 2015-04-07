using UnityEngine;
using System.Collections.Generic;
using ZMPlayer;

public class ZMScorePool : MonoBehaviour {
	// editor exposed members
	public static float MaxScore = 100;
	public float scoreAmount;

	// class members
	public static float CurrentScorePool = 50;

	HashSet<ZMScoreController> _gainingAgents;
	HashSet<ZMScoreController> _drainingAgents;

	void Awake () {
		_gainingAgents  = new HashSet<ZMScoreController>();
		_drainingAgents = new HashSet<ZMScoreController>();

		ZMScoreController.CanScoreEvent += HandleCanScoreEvent;
		ZMScoreController.CanDrainEvent += HandleCanDrainEvent;
		ZMScoreController.StopScoreEvent += HandleStopScoreEvent;

		ZMPedestalController.DeactivateEvent += HandleDeactivateEvent;
	}

	// Update is called once per frame
	void Update () {
		if (_gainingAgents.Count > 0) {
			foreach (ZMScoreController scoreController in _gainingAgents) {
				//Debug.Log ("gain agent count " + _gainingAgents.Count.ToString());
				scoreController.AddToScore(scoreAmount / _gainingAgents.Count);
				CurrentScorePool -= scoreAmount;
			}
		}

		if (_drainingAgents.Count > 0) {
			foreach (ZMScoreController scoreController in _drainingAgents) {
				//Debug.Log ("drain agent count " + _drainingAgents.Count.ToString());
				scoreController.AddToScore(-scoreAmount / _drainingAgents.Count);
				CurrentScorePool += scoreAmount;
			}
		}
	}

	void HandleCanDrainEvent (ZMScoreController scoreController)
	{
		if (!_drainingAgents.Contains(scoreController)) {
			_drainingAgents.Add(scoreController);
			_gainingAgents.Remove(scoreController);
		}
	}

	void HandleCanScoreEvent (ZMScoreController scoreController)
	{
		if (!_gainingAgents.Contains(scoreController)) {
			_gainingAgents.Add(scoreController);
			_drainingAgents.Remove(scoreController);
		}
	}

	void HandleStopScoreEvent (ZMScoreController scoreController)
	{
		_gainingAgents.Remove(scoreController);	
		_drainingAgents.Remove(scoreController);
	}

	void HandleDeactivateEvent(ZMPedestalController pedestalController) {
		_gainingAgents.Clear();
		_drainingAgents.Clear();
	}
}
