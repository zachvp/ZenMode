using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using ZMPlayer;

public class ZMMaxScoreMessage : MonoBehaviour {
	public string message = "Ready!";
	public Text text;
	public GameObject slider;

	private ZMPlayerInfo _playerInfo;

	// Use this for initialization
	void Awake () {
		_playerInfo = GetComponent<ZMPlayerInfo>();
		text.gameObject.SetActive(false);
		text.text = "";

		ZMLobbyScoreController.MaxScoreReachedEvent += HandleMaxScoreReachedEvent;
	}

	void HandleMaxScoreReachedEvent(ZMLobbyScoreController lobbyScoreController)
	{
		if (_playerInfo == lobbyScoreController.PlayerInfo)
		{
			text.text = message;
			text.gameObject.SetActive(true);
			Destroy(slider);
		}
	}
}
