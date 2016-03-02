using UnityEngine;
using ZMPlayer;
using Core;

[RequireComponent(typeof(ZMPlayerInfo))]
[RequireComponent(typeof(Collider))]
public class ZMBreakable : ZMPlayerJoinHandler
{
	[SerializeField] private ParticleSystem effectTemplate;
//	[SerializeField] private bool destroyOnJoin;

	private ParticleSystem destructionEffect;
	private Collider2D _collider;
	private Collider2D _childCollider;

	private bool _handlingCollision;

	private bool _active;

	protected override void Awake()
	{
		base.Awake();

		_collider = GetComponent<Collider2D>();
		_childCollider = transform.GetChild(0).GetComponent<Collider2D>();

		_active = true;
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

	protected override void HandleJoinedEvent()
	{
		if (_deactivateOnJoin)
		{
			Break();
		}
	}

	private void StopGibs()
	{
		destructionEffect.Stop();
		Destroy(destructionEffect.gameObject, 0.4f);
		_handlingCollision = false;
	}

	private void Break()
	{
		destructionEffect = Instantiate(effectTemplate, transform.position, Quaternion.identity) as ParticleSystem;
		destructionEffect.Play();
		
		Invoke("StopGibs", 0.1f);
		SetActive(false);
	}

	protected override void SetActive(bool active)
	{
		base.SetActive(active);

		_collider.enabled = active;
		_childCollider.enabled = active;

		_active = active;
	}
}
