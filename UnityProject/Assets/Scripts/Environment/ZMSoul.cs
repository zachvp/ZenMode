using UnityEngine;
using System.Collections;
using ZMPlayer;

public class ZMSoul : MonoBehaviour {
	private ZMScoreController _scoreController;

	private ZMPlayerInfo _playerInfo; public ZMPlayerInfo PlayerInfo { get { return _playerInfo; } }
	//private float _currentZen; public float CurrentZen { get { return _currentZen; } set { _currentZen = value; } }

	public delegate void SoulDestroyedAction(ZMSoul soul); public static event SoulDestroyedAction SoulDestroyedEvent;

	void Awake () {
		_playerInfo = GetComponent<ZMPlayerInfo>();

		ZMScoreController.MinScoreReached += HandleMinScoreReached;
	}

	void OnDestroy() {
		SoulDestroyedEvent = null;
	}

	void HandleMinScoreReached (ZMScoreController scoreController)
	{
		if (scoreController.PlayerInfo.playerTag.Equals(_playerInfo.playerTag)) {
			if (SoulDestroyedEvent != null) {
				SoulDestroyedEvent(this);
			}
		}
	}

	void Start() {
		GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
		
		foreach (GameObject player in players) {
			ZMScoreController scoreController = player.GetComponent<ZMScoreController>();
			
			if (scoreController.PlayerInfo.playerTag.Equals(_playerInfo.playerTag)) {
				_scoreController = scoreController;
			}
		}
	}

	public void AddZen(float amount) {
		_scoreController.AddToScore(amount);
	}

	public float GetZen() {
		return _scoreController.TotalScore;
	}

	public void SetZen(float amount) {
		_scoreController.SetScore(amount);
	}
}
