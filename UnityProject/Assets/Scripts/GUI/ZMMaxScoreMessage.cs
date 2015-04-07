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
		text.text = "";

		ZMLobbyScoreController.MaxScoreReachedEvent += HandleMaxScoreReachedEvent;
	}

	void HandleMaxScoreReachedEvent (ZMLobbyScoreController lobbyScoreController)
	{
		if (lobbyScoreController.GetComponent<ZMPlayerInfo>().playerTag.Equals(_playerInfo.playerTag)) {
			text.text = message;
			Destroy(slider);
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
