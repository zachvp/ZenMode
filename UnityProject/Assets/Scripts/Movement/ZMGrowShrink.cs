using UnityEngine;

public class ZMGrowShrink : MonoBehaviour
{
	public bool startEnabled = true;
	public float rate = 100.0f;
	public float minScale = 0.5f;
	public float maxScale = 0.99f;

	private Vector3 _baseScale;
	private Vector3 _modScale;

	private enum State { NONE, GROWING, SHRINKING } private State _state; private State _prevState;

	private bool _enabled;

	void Awake ()
	{
		_enabled = startEnabled;
		_baseScale = new Vector3(transform.localScale.x, transform.localScale.y, transform.localScale.z);
	}
	
	void Update ()
	{
		if (_enabled) {
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

	void Stop() {
		_enabled = false;
	}

	void Resume() {
		_enabled = true;
	}
}
