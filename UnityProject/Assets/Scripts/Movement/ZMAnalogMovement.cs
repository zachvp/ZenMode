using UnityEngine;
using ZMPlayer;

public class ZMAnalogMovement : ZMDirectionalInput
{
	public float _movementSpeed = 100;
	[Range (0, 10)] public float bounce = 1;

	private Vector3 _deltaPos;
	private Vector3 _forward;
	
	private bool _shouldBounce;
	private float _slowFactor;

	// References.
	private ZMPlayerInfo _playerInfo;

	private Color _baseColor;

	protected override void Awake()
	{
		base.Awake();

		_playerInfo = GetComponent<ZMPlayerInfo>();
		_slowFactor = 1;

		if (renderer != null) { _baseColor = renderer.material.color; }
	}

	void Update()
	{
		if (!_shouldBounce)
		{
			_deltaPos = transform.position;
			_forward =  new Vector3(_deltaPos.x, _deltaPos.y, _deltaPos.z);

			_deltaPos += new Vector3 (_movement.x, _movement.y, 0.0f);

			_forward = (_deltaPos - _forward) * _slowFactor;

			rigidbody2D.velocity = _forward * _movementSpeed;
		}
	}

	void OnTriggerEnter2D(Collider2D collider)
	{
		if (collider.gameObject.layer.Equals(LayerMask.NameToLayer("Barrier")))
		{
			ZMSurfaceNormalHack surfaceNormalHack;
			Vector2 reflection;
			Vector2 normal;

			surfaceNormalHack = collider.GetComponent<ZMSurfaceNormalHack>();
			normal = surfaceNormalHack.normal;
			reflection = rigidbody2D.velocity - 2 * normal * (Vector2.Dot(rigidbody2D.velocity, normal));

			rigidbody2D.velocity = Vector2.ClampMagnitude(reflection * bounce, _movementSpeed * 6);

			_shouldBounce = true;
			Invoke("CancelBounce", 0.2f);
		}
		else if (collider.gameObject.layer.Equals(LayerMask.NameToLayer("Ground")))
		{
			Color faded = _baseColor;
			faded.a = 0.3f;

			_slowFactor = 0.4f;

			if (renderer != null) { renderer.material.color = faded; }
		}
	}

	void OnTriggerExit2D(Collider2D collider)
	{
		if (collider.gameObject.layer.Equals(LayerMask.NameToLayer("Ground")))
		{
			_slowFactor = 1f;

			if (renderer != null) { renderer.material.color = _baseColor; }
		}
	}

	protected override bool IsCorrectInputControl(ZMInput input)
	{
		return (int) _playerInfo.playerTag == input.ID;
	}

	private void CancelBounce()
	{
		_shouldBounce = false;
	}
}
