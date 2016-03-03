using UnityEngine;
using ZMPlayer;
using Core;

public class ZMAnalogMovement : ZMDirectionalInput
{
	public float _movementSpeed = 100;
	[Range (0, 10)] public float bounce = 1;

	private Vector3 _deltaPos;
	private Vector3 _forward;

	private Vector2 _previousVelocity;

	private bool _shouldBounce;

	private float _slowFactor;
	
	private Color _baseColor;

	protected override void Awake()
	{
		base.Awake();

		_playerInfo = GetComponent<ZMPlayerInfo>();
		_slowFactor = 1;

		MatchStateManager.OnMatchPause += Disable;
		MatchStateManager.OnMatchResume += Enable;
	}

	protected virtual void Start()
	{
		if (GetComponent<Renderer>() != null) { _baseColor = Utilities.GetRGB(GetComponent<Renderer>().material.color, _playerInfo.standardColor); }
	}

	void Update()
	{
		if (!_shouldBounce)
		{
			_deltaPos = transform.position;
			_forward = new Vector3(_deltaPos.x, _deltaPos.y, _deltaPos.z);

			_deltaPos += new Vector3 (_movement.x, _movement.y, 0.0f);

			_forward = (_deltaPos - _forward) * _slowFactor;

			GetComponent<Rigidbody2D>().velocity = _forward * _movementSpeed;
		}
	}

	void OnTriggerEnter2D(Collider2D collider)
	{
		if (collider.gameObject.layer.Equals(LayerMask.NameToLayer("Barrier")))
		{
			_shouldBounce = true;
			Invoke("CancelBounce", 0.25f);
		}
		else if (collider.gameObject.layer.Equals(LayerMask.NameToLayer("Ground")))
		{
			Color faded = _baseColor;
			faded.a = 0.3f;

			_slowFactor = 0.4f;

			if (GetComponent<Renderer>() != null) { GetComponent<Renderer>().material.color = faded; }
		}
	}

	void OnTriggerExit2D(Collider2D collider)
	{
		if (collider.gameObject.layer.Equals(LayerMask.NameToLayer("Ground")))
		{
			_slowFactor = 1f;

			if (GetComponent<Renderer>() != null) { GetComponent<Renderer>().material.color = _baseColor; }
		}
	}


	protected override bool IsValidInputControl(ZMInput input)
	{
		return _playerInfo.ID == input.ID;
	}

	private void Disable()
	{
		enabled = false;
		_previousVelocity = GetComponent<Rigidbody2D>().velocity;
		GetComponent<Rigidbody2D>().velocity = Vector2.zero;
	}
	
	private void Enable()
	{
		GetComponent<Rigidbody2D>().velocity = _previousVelocity;
		enabled = true;
	}

	private void CancelBounce()
	{
		_shouldBounce = false;
	}
}
