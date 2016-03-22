using UnityEngine;

public class ZMGrowShrink : MonoBehaviour
{
	public bool startEnabled = true;
	public float rate = 10.0f;
	public float minScale = 0.5f;
	public float maxScale = 0.99f;

	private Vector3 _baseScale;

	private enum State { NONE, GROWING, SHRINKING }

	private State _state;
	private State _prevState;

	void Awake()
	{
		_baseScale = new Vector3(transform.localScale.x, transform.localScale.y, transform.localScale.z);
		enabled = startEnabled;
	}
	
	void Update()
	{
		var modScale = transform.localScale;
		var modScaleMagnitude = Vector3.SqrMagnitude(modScale);
		var baseScaleMagnitude = Vector3.SqrMagnitude(_baseScale);
		var increment = new Vector3(1, 1, 0);

		increment *= rate * Time.deltaTime;

		// Check if current scale is above or below specified threshold.
		if (modScaleMagnitude > baseScaleMagnitude * (maxScale * maxScale))
		{
			_state = State.SHRINKING;
		}
		else if (modScaleMagnitude < baseScaleMagnitude * (minScale * minScale))
		{
			_state = State.GROWING;
		}

		if (_state == State.SHRINKING)
		{
			modScale -= increment;
		}
		else if (_state == State.GROWING)
		{
			modScale += increment;
		}

		transform.localScale = modScale;
	}

	public void Stop()
	{
		enabled = false;
	}

	public void Resume()
	{
		enabled = true;
	}
}
