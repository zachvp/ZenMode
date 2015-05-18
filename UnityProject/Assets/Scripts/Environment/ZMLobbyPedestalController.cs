using UnityEngine;
using System.Collections;

public class ZMLobbyPedestalController : MonoBehaviour {
	// ID
	private ZMPlayer.ZMPlayerInfo _playerInfo; public ZMPlayer.ZMPlayerInfo PlayerInfo { get { return _playerInfo; } }

	public delegate void ActivateAction(ZMLobbyPedestalController lobbyPedestalController); public static event ActivateAction ActivateEvent;

	// Use this for initialization
	void Awake() {
		gameObject.SetActive(false);

		_playerInfo = GetComponent<ZMPlayer.ZMPlayerInfo>();

		ZMLobbyScoreController.MaxScoreReachedEvent += HandleMaxScoreReachedEvent;
		ZMLobbyController.PlayerJoinedEvent += HandlePlayerJoinedEvent;
		ZMWaypointMovement.AtPathEndEvent += HandleAtPathEndEvent;
	}

	void OnDestroy() {
		ActivateEvent = null;
	}

	void HandleAtPathEndEvent (ZMWaypointMovement waypointMovement)
	{
		if (waypointMovement.gameObject.Equals(gameObject)) {
			if (ActivateEvent != null) {
				ActivateEvent(this);
			}
		}
	}

	void HandlePlayerJoinedEvent (ZMPlayer.ZMPlayerInfo.PlayerTag playerTag)
	{
		if (playerTag.Equals(_playerInfo.playerTag)) {
			gameObject.SetActive(true);
		}
	}

	void HandleMaxScoreReachedEvent (ZMLobbyScoreController lobbyScoreController)
	{
		if (lobbyScoreController.GetComponent<ZMPlayer.ZMPlayerInfo>().playerTag.Equals(GetComponent<ZMPlayer.ZMPlayerInfo>().playerTag)) {

			GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
			
			foreach (GameObject player in players) {
				ZMPlayerController playerController = player.GetComponent<ZMPlayerController>();
				if (playerController.PlayerInfo.playerTag.Equals(GetComponent<ZMPlayer.ZMPlayerInfo>().playerTag)) {
					playerController.DisablePlayer();
					playerController.renderer.enabled = false;
					playerController.transform.position = new Vector3(9000.0f, 9000.0f, 9000.0f);
					break;
				}
			}

			gameObject.SetActive(false);

		}
	}
}
