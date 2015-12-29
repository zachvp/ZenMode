using UnityEngine;
using UnityEngine.UI;

public class ZMHandlePlayerJoin : MonoBehaviour {
	public string methodAction;
	public bool sendOnce = true;

	private ZMPlayer.ZMPlayerInfo _playerInfo;
	private bool _sent;

	// Use this for initialization
	void Awake () {
		_playerInfo = GetComponent<ZMPlayer.ZMPlayerInfo>();

		ZMLobbyController.PlayerJoinedEvent += HandlePlayerJoinedEvent;
		ZMLobbyController.DropOutEvent += HandleDropOutEvent;
	}

	void HandleDropOutEvent (int playerIndex)
	{
		if (playerIndex == (int) _playerInfo.playerTag) {
			_sent = false;
		}
	}

	void HandlePlayerJoinedEvent(int controlIndex)
	{
		if ((int) _playerInfo.playerTag == controlIndex )
		{
			if (sendOnce)
			{
				if (!_sent)
				{
					SendMessage(methodAction);
					_sent = true;
				}
			}
			else
			{
				SendMessage(methodAction);
			}
		}
	}

	void Disable() {
		gameObject.SetActive(false);
	}

	void Enable() {
		Image image = GetComponent<Image>();
		Text text = GetComponent<Text>();

		gameObject.SetActive(true);

		if (image != null) {
			image.enabled = true;
		}

		if (text != null) {
			Color visible = new Color(text.color.r, text.color.g, text.color.b, 1);

			text.color = visible;
		}
	}
}
