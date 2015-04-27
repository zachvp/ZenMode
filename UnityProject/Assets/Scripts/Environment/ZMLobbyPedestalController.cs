using UnityEngine;
using System.Collections;

public class ZMLobbyPedestalController : MonoBehaviour {
	// ID
	private ZMPlayer.ZMPlayerInfo _playerInfo; public ZMPlayer.ZMPlayerInfo PlayerInfo { get { return _playerInfo; } }

	private bool _active; public bool Active { get { return _active; } }

	// Use this for initialization
	void Awake() {
		_active = false;
		gameObject.SetActive(false);

		_playerInfo = GetComponent<ZMPlayer.ZMPlayerInfo>();

		ZMLobbyScoreController.MaxScoreReachedEvent += HandleMaxScoreReachedEvent;
		ZMLobbyController.PlayerJoinedEvent += HandlePlayerJoinedEvent;
		ZMWaypointMovement.AtPathEndEvent += HandleAtPathEndEvent;
	}

	void HandleAtPathEndEvent (ZMWaypointMovement waypointMovement)
	{
		if (waypointMovement.gameObject.Equals(gameObject)) {
			_active = true;
		}
	}

	void HandlePlayerJoinedEvent (ZMPlayer.ZMPlayerInfo.PlayerTag playerTag)
	{
		if (playerTag.Equals(_playerInfo.playerTag)) {
			gameObject.SetActive(true);
		}
	}

	/*void OnDestroy() {
		ZMLobbyScoreController.MaxScoreReachedEvent -= HandleMaxScoreReachedEvent;
	}*/

	void HandleMaxScoreReachedEvent (ZMLobbyScoreController lobbyScoreController)
	{
		if (lobbyScoreController.GetComponent<ZMPlayer.ZMPlayerInfo>().playerTag.Equals(GetComponent<ZMPlayer.ZMPlayerInfo>().playerTag)) {
			gameObject.SetActive(false);
		}
	}
}
