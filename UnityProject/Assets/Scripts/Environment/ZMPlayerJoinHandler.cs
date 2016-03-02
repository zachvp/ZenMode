using UnityEngine;
using UnityEngine.UI;
using ZMPlayer;
using Core;

public class ZMPlayerJoinHandler : MonoBehaviour
{
	// TODO: Make configurable. Should be able to say what happens to object on join & drop out.
	[SerializeField] private bool _activeOnJoin;

	protected ZMPlayerInfo _playerInfo;

	protected virtual void Awake()
	{
		_playerInfo = GetComponent<ZMPlayerInfo>();
		Utilities.SetVisible(gameObject, !_activeOnJoin);

		ZMLobbyController.OnPlayerJoinedEvent += HandleJoinedEvent;
		ZMLobbyController.OnPlayerDropOut += HandleDropOutEvent;
	}

	private void HandleDropOutEvent(ZMPlayerInfo info)
	{
		if (_playerInfo == info)
		{
			Utilities.SetVisible(gameObject, !_activeOnJoin);

			HandleDropOutEvent();
		}
	}

	private void HandleJoinedEvent(int controlIndex)
	{
		if (_playerInfo.ID == controlIndex )
		{
			Utilities.SetVisible(gameObject, _activeOnJoin);

			HandleJoinedEvent();
		}
	}

	protected virtual void HandleDropOutEvent() { }
	protected virtual void HandleJoinedEvent()  { }

}
