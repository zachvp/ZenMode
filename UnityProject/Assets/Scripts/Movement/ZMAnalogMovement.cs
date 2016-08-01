using UnityEngine;
using ZMPlayer;
using Core;

[RequireComponent(typeof(ZMDirectionalInput))]
[RequireComponent(typeof(Rigidbody2D))]
public class ZMAnalogMovement : ZMPlayerItem
{
	[SerializeField] private bool startActive;
	[SerializeField] private float _movementSpeed = 100;

	private Rigidbody2D _rigidbody;

	private Renderer _renderer;
	private Vector2 _previousVelocity;
	private Vector2 _movement;
	private Color _baseColor;
	private ZMDirectionalInputEventNotifier _inputEventNotifier;

	private bool _shouldBounce;
	private float _slowFactor;

	protected override void Awake()
	{
		base.Awake();

		_playerInfo = GetComponent<ZMPlayerInfo>();
		_rigidbody = GetComponent<Rigidbody2D>();
		_renderer = GetComponent<Renderer>();

		_slowFactor = 1;

		MatchStateManager.OnMatchPause += Disable;
		MatchStateManager.OnMatchResume += Enable;
	}

	protected virtual void Start()
	{
		var directionalInput = GetComponent<ZMDirectionalInput>();

		Debug.AssertFormat(directionalInput != null, "{0} requires directional input component.",
						   Utilities.GetClassNameForObject(this));

		_inputEventNotifier = directionalInput._inputEventNotifier;
		_inputEventNotifier.OnMoveEvent += HandleMove;

		if (startActive)
		{
			directionalInput.ConfigureItemWithID(_playerInfo.ID);
		}

		if (_renderer != null)
		{
			_baseColor = Utilities.GetRGB(_renderer.material.color, _playerInfo.standardColor);
		}
	}

	void Update()
	{
		if (!_shouldBounce)
		{
			var deltaPos = transform.position;
			var forward = new Vector3(deltaPos.x, deltaPos.y, deltaPos.z);

			deltaPos += new Vector3 (_movement.x, _movement.y, 0.0f);

			forward = (deltaPos - forward) * _slowFactor;

			_rigidbody.velocity = forward * _movementSpeed;
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
			var faded = _baseColor;
			faded.a = 0.3f;

			_slowFactor = 0.4f;

			if (_renderer != null) { _renderer.material.color = faded; }
		}
	}

	void OnTriggerExit2D(Collider2D collider)
	{
		if (collider.gameObject.layer.Equals(LayerMask.NameToLayer("Ground")))
		{
			_slowFactor = 1f;

			if (_renderer != null) { _renderer.material.color = _baseColor; }
		}
	}

	private void HandleMove(Vector2EventArgs args)
	{
		_movement = args.value;
	}

	private void Disable()
	{
		enabled = false;
		_previousVelocity = _rigidbody.velocity;
		_rigidbody.velocity = Vector2.zero;
	}
	
	private void Enable()
	{
		_rigidbody.velocity = _previousVelocity;
		enabled = true;
	}

	private void CancelBounce()
	{
		_shouldBounce = false;
	}
}
