using UnityEngine;
using ZMPlayer;

public class ZMBreakable : MonoBehaviour
{
	public ParticleSystem destructionEffect;

	private bool _handlingCollision;
	private ZMPlayer.ZMPlayerInfo _playerInfo;

	void Awake() {
		_playerInfo = GetComponent<ZMPlayer.ZMPlayerInfo>();

		ZMLobbyController.DropOutEvent += HandleDropOutEvent;

	}

	void HandleDropOutEvent(ZMPlayerInfo info)
	{
		if (_playerInfo == info)
		{
			Debug.Log(_playerInfo.ID.ToString() + ": droppped out");
			gameObject.SetActive(true);
		}
	}

	public void HandleCollision(ZMPlayer.ZMPlayerInfo playerInfo)
	{
		if (_playerInfo == playerInfo)
		{
			if (!_handlingCollision)
			{
				Break ();
			}
			_handlingCollision = true;
		}
	}

	void StopGibs() {
		destructionEffect.Stop();
		Destroy(destructionEffect.gameObject, 0.8f);
		//Destroy(gameObject, 0.2f);

		_handlingCollision = false;
	}

	void Break() {
		destructionEffect = Instantiate(destructionEffect) as ParticleSystem;
		destructionEffect.transform.position = transform.position;
		destructionEffect.Play();
		
		Invoke ("StopGibs", 0.1f);
		
		gameObject.SetActive(false);
	}
}
