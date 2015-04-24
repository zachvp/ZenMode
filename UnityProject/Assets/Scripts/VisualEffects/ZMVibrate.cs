using UnityEngine;
using System.Collections;

public class ZMVibrate : MonoBehaviour {
	public float minRotate = -5;
	public float maxRotate = 5;
	public float speed = 6.0f;
	public int switchLimit = 60;

	private bool _rotateToMin;
	private bool _shouldVibrate;
	private int _switchCounter = 0;
	private Quaternion _baseRotation;

	// Use this for initialization
	void Start () {
		_rotateToMin = true;
		_baseRotation = transform.rotation;
	}
	
	// Update is called once per frame
	void Update () {
		if (_shouldVibrate) {
			if (_rotateToMin) {
				Quaternion target = Quaternion.Euler(0, 0, minRotate);

				//transform.rotation = target;
				transform.rotation = Quaternion.Slerp(transform.rotation, target, Time.deltaTime * speed);

				_switchCounter += 1;

				if (_switchCounter > switchLimit) {
					Debug.Log("Other way!");
					_rotateToMin = false;
					_switchCounter = 0;
				}
			} else {
				Quaternion target = Quaternion.Euler(0, 0, maxRotate);
				
				//transform.rotation = target;
				transform.rotation = Quaternion.Slerp(transform.rotation, target, Time.deltaTime * speed);
				
				_switchCounter += 1;
				
				if (_switchCounter > switchLimit) {
					_rotateToMin = true;
					_switchCounter = 0;
				}
			}
		}
	}

	public void VibrateStart() {
		_shouldVibrate = true;
	}

	public void VibrateStop() {
		_shouldVibrate = false;
		transform.rotation = _baseRotation;
	}
}
