using UnityEngine;
using InControl;
using ZMPlayer;

public class ZMAnalogMovement : MonoBehaviour {
	public float _movementSpeed = 4;

	private Vector3 _deltaPos;

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
		_deltaPos = transform.position;

		_deltaPos.x += InputManager.Devices[_controlIndex].LeftStickX * _movementSpeed;
		_deltaPos.y += InputManager.Devices[_controlIndex].LeftStickY * _movementSpeed;

		transform.position = _deltaPos;
	}
}
