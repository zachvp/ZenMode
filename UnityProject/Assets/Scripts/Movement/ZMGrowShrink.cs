using UnityEngine;
using System.Collections;

public class ZMGrowShrink : MonoBehaviour {
	public float rate = 100.0f;
	public float minScale = 0.5f;
	public float maxScale = 0.99f;

	private Vector3 _baseScale;
	private Vector3 _modScale;

	private enum State { NONE, GROWING, SHRINKING }

	private State _state;

	// Use this for initialization
	void Awake () {
		_baseScale = transform.localScale;
	}
	
	// Update is called once per frame
	void Update () {
		_modScale = transform.localScale;

		if (_modScale.x > _baseScale.x * maxScale) {
			_state = State.SHRINKING;

		} else if (_modScale.x < _baseScale.x * minScale) {
			_state = State.GROWING;
		}

		if (_state == State.SHRINKING) {
			_modScale.x -= rate * Time.deltaTime;
			_modScale.y -= rate * Time.deltaTime;
		} else if (_state == State.GROWING) {
			_modScale.x += rate * Time.deltaTime;
			_modScale.y += rate * Time.deltaTime;
		}

		transform.localScale = _modScale;
	}
}
