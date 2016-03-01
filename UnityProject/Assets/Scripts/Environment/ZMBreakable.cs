using UnityEngine;
using ZMPlayer;

[RequireComponent(typeof(ZMPlayerInfo))]
[RequireComponent(typeof(Renderer))]
[RequireComponent(typeof(Collider))]
public class ZMBreakable : MonoBehaviour
{
	[SerializeField] private ParticleSystem effectTemplate;

	private ParticleSystem destructionEffect;
	private Renderer _renderer;
	private Collider2D _collider;
	private Collider2D _childCollider;

	private bool _handlingCollision;
	private ZMPlayerInfo _playerInfo;

	void Awake()
	{
		_playerInfo = GetComponent<ZMPlayerInfo>();
		_renderer = GetComponent<Renderer>();
		_collider = GetComponent<Collider2D>();
		_childCollider = transform.GetChild(0).GetComponent<Collider2D>();

		ZMLobbyController.OnPlayerDropOut += HandleDropOutEvent;
	}

	public void HandleCollision(ZMPlayerInfo playerInfo)
	{
		if (_playerInfo == playerInfo)
		{
			if (!_handlingCollision)
			{
				Break();
				_handlingCollision = true;
			}
		}
	}

	private void HandleDropOutEvent(ZMPlayerInfo info)
	{
		if (_playerInfo == info)
		{
			Debug.Log(_playerInfo.ID.ToString() + ": droppped out");
			SetActive(true);
		}
	}

	private void StopGibs()
	{
		destructionEffect.Stop();
		Destroy(destructionEffect.gameObject, 0.4f);
	}

	private void Break()
	{
		destructionEffect = Instantiate(effectTemplate) as ParticleSystem;
		destructionEffect.transform.position = transform.position;
		destructionEffect.Play();
		
		Invoke("StopGibs", 0.1f);

		SetActive(false);
	}

	private void SetActive(bool active)
	{
		_renderer.enabled = active;
		_collider.enabled = active;
		_childCollider.enabled = active;

		_handlingCollision = active;
	}
}
