using UnityEngine;
using InControl;
using ZMPlayer;

public class ZMAnalogMovement : MonoBehaviour {
	public float _movementSpeed = 100;

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
	
	// Update is called once per frame
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
			Vector2 bounceVector = new Vector2(_forward.x, _forward.y);
			bounceVector *= -1 * 600;

			rigidbody2D.velocity = bounceVector;
			_shouldBounce = true;
			Invoke("CancelBounce", 0.2f);
		}
	}

	void CancelBounce() {
		_shouldBounce = false;
	}
}
