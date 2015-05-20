using UnityEngine;
using InControl;
using ZMPlayer;

public class ZMAnalogMovement : MonoBehaviour {
	public float _movementSpeed = 100;
	[Range (0, 10)] public float bounce = 1;

	private Vector3 _deltaPos;
	private Vector3 _forward;
	private bool _shouldBounce;

	// references
	private ZMPlayerInfo _playerInfo;
	private int _controlIndex;	

	void Awake () {
		_playerInfo = GetComponent<ZMPlayerInfo>();
		_controlIndex = (int) _playerInfo.playerTag;

		if (_controlIndex >= InputManager.Devices.Count) {
			enabled = false;
		}
	}

	void Update () {
		if (!_shouldBounce) {
			_deltaPos = transform.position;
			_forward =  new Vector3(_deltaPos.x, _deltaPos.y, _deltaPos.z);

			_deltaPos.x += InputManager.Devices[_controlIndex].LeftStickX;
			_deltaPos.y += InputManager.Devices[_controlIndex].LeftStickY;

			_forward = _deltaPos - _forward;

			rigidbody2D.velocity = _forward * _movementSpeed;
		}
	}

	void OnTriggerEnter2D(Collider2D collider) {
		if (collider.gameObject.layer.Equals(LayerMask.NameToLayer("Barrier"))) {
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
	}

	void CancelBounce() {
		_shouldBounce = false;
	}
}
