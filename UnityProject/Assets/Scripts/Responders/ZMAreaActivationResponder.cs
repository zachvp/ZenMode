using UnityEngine;
using ZMPlayer;
using ZMConfiguration;

public class ZMAreaActivationResponder : MonoBehaviour
{
	// Objects that will be activated when the trigger is entered.
	[SerializeField] private GameObject[] _onPlayerCreateActivationObjects;

	// Objects that will be deactivated when the trigger is entered.
	[SerializeField] private GameObject[] _onPlayerEnterActivationObjects;

	private ZMPlayerInfo _playerInfo;

	void Awake()
	{
		_playerInfo = GetComponent<ZMPlayerInfo>();

		SetActive(_onPlayerCreateActivationObjects, false);
		SetActive(_onPlayerEnterActivationObjects, false);

		ZMPlayerController.OnPlayerCreate += HandlePlayerCreate;
		ZMLobbyController.OnPlayerDropOut += HandlePlayerDropOut;
	}

	void OnTriggerEnter2D(Collider2D collider)
	{
		// Bail if something besides a player makes it's way in.
		if (!collider.CompareTag(Tags.kPlayerTag)) { return; }

		var checkPlayer = collider.GetComponent<ZMPlayerInfo>();

		if (_playerInfo == checkPlayer)
		{
			SetActive(_onPlayerEnterActivationObjects, true);
			SetActive(_onPlayerCreateActivationObjects, false);
		}
	}

	private void HandlePlayerCreate(ZMPlayerInfoEventArgs args)
	{
		if (_playerInfo == args.info)
		{
			SetActive(_onPlayerCreateActivationObjects, true);
		}
	}

	private void HandlePlayerDropOut(ZMPlayerInfoEventArgs args)
	{
		if (_playerInfo == args.info)
		{
			SetActive(_onPlayerCreateActivationObjects, false);
			SetActive(_onPlayerEnterActivationObjects, false);
		}
	}

	private void SetActive(GameObject[] objects, bool active)
	{
		for (int i = 0; i < objects.Length; ++i)
		{
			objects[i].SetActive(active);
		}
	}
}
