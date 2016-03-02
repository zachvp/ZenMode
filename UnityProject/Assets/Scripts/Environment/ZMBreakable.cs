using UnityEngine;
using ZMPlayer;
using Core;

[RequireComponent(typeof(ZMPlayerInfo))]
[RequireComponent(typeof(Collider))]
public class ZMBreakable : MonoBehaviour
{
	[SerializeField] private ParticleSystem effectTemplate;
	[SerializeField] private bool destroyOnJoin;

	private ZMPlayerInfo _playerInfo;
	private ParticleSystem destructionEffect;
	private Collider2D _collider;
	private Collider2D _childCollider;

	private bool _handlingCollision;

	private bool _active;

	void Awake()
	{
		_playerInfo = GetComponent<ZMPlayerInfo>();
		_collider = GetComponent<Collider2D>();
		_childCollider = transform.GetChild(0).GetComponent<Collider2D>();

		_active = true;

		ZMLobbyController.OnPlayerJoinedEvent += HandleJoinedEvent;
		ZMLobbyController.OnPlayerDropOut += HandleDropOutEvent;
	}

	public void HandleCollision(ZMPlayerInfo playerInfo)
	{
		if (_playerInfo == playerInfo)
		{
			if (!_handlingCollision && _active)
			{
				_handlingCollision = true;
				
				Break();
			}
		}
	}

	private void HandleJoinedEvent(int id)
	{
		if (_playerInfo.ID == id)
		{
			if (destroyOnJoin)
			{
				Break();
			}
		}
	}

	private void HandleDropOutEvent(ZMPlayerInfo info)
	{
		if (_playerInfo == info)
		{
			if (destroyOnJoin)
			{
				SetActive(true);
			}
		}
	}

	private void StopGibs()
	{
		destructionEffect.Stop();
		Destroy(destructionEffect.gameObject, 0.4f);
	}

	private void Break()
	{
		destructionEffect = Instantiate(effectTemplate, transform.position, Quaternion.identity) as ParticleSystem;
		destructionEffect.Play();
		
		Invoke("StopGibs", 0.1f);

		SetActive(false);
	}

	private void SetActive(bool active)
	{
		_collider.enabled = active;
		_childCollider.enabled = active;

		_handlingCollision = active;
		_active = active;

		Utilities.SetVisible(gameObject, active);
	}
}
