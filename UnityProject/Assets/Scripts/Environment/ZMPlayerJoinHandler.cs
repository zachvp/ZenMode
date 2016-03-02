using UnityEngine;
using UnityEngine.UI;
using ZMPlayer;
using Core;

public class ZMPlayerJoinHandler : MonoBehaviour
{
	// TODO: Make configurable. Should be able to say what happens to object on join & drop out.
	[SerializeField] protected bool _activateOnJoin;
	[SerializeField] protected bool _deactivateOnJoin;

	[SerializeField] protected bool _activateOnDrop;
	[SerializeField] protected bool _deactivateOnDrop;

	protected ZMPlayerInfo _playerInfo;

	protected virtual void Awake()
	{
		_playerInfo = GetComponent<ZMPlayerInfo>();
		Utilities.SetVisible(gameObject, !_activateOnJoin);

		ZMLobbyController.OnPlayerJoinedEvent += HandleJoinedEvent;
		ZMLobbyController.OnPlayerDropOut += HandleDropOutEvent;

		Debug.AssertFormat(VerifySettings(), "{0}: Unable to use given editor settings.", name);
	}

	private void HandleDropOutEvent(ZMPlayerInfo info)
	{
		if (_playerInfo == info)
		{
			SetActive(_activateOnDrop || !_deactivateOnDrop);

			HandleDropOutEvent();
		}
	}

	private void HandleJoinedEvent(int controlIndex)
	{
		if (_playerInfo.ID == controlIndex )
		{
			SetActive(_activateOnJoin || !_deactivateOnJoin);

			HandleJoinedEvent();
		}
	}

	private bool VerifySettings()
	{
		var anyjoin = _activateOnJoin || _deactivateOnJoin;
		var anydrop = _activateOnDrop || _deactivateOnDrop;

		return (!anyjoin  || (_activateOnJoin ^ _deactivateOnJoin)) &&
			   (!anydrop) || (_activateOnDrop ^ _deactivateOnDrop);
	}

	protected virtual void SetActive(bool active)
	{
		Utilities.SetVisible(gameObject, active);
	}

	protected virtual void HandleDropOutEvent() { }
	protected virtual void HandleJoinedEvent()  { }

}
