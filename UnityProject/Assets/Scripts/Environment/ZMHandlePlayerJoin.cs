using UnityEngine;
using UnityEngine.UI;
using ZMPlayer;

public class ZMHandlePlayerJoin : MonoBehaviour {
	public string methodAction;
	public bool sendOnce = true;

	private ZMPlayer.ZMPlayerInfo _playerInfo;
	private bool _sent;

	// Use this for initialization
	void Awake () {
		_playerInfo = GetComponent<ZMPlayer.ZMPlayerInfo>();

		ZMLobbyController.OnPlayerJoinedEvent += HandlePlayerJoinedEvent;
		ZMLobbyController.DropOutEvent += HandleDropOutEvent;
	}

	void HandleDropOutEvent(ZMPlayerInfo info)
	{
		if (_playerInfo == info) { _sent = false; }
	}

	void HandlePlayerJoinedEvent(int controlIndex)
	{
		if (_playerInfo.ID == controlIndex )
		{
			if (sendOnce)
			{
				if (!_sent)
				{
					SendMessage(methodAction, SendMessageOptions.DontRequireReceiver);
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
