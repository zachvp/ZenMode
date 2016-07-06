using UnityEngine;
using Core;

public class ZMScalePulse : ZMScaleBehavior
{
	[SerializeField] private bool startEnabled;
	[SerializeField] private float scaleTime = 2.0f;
	[SerializeField] [Range(0.0f, 0.99f)] private float minScale = 0.5f;
	[SerializeField] [Range(0f, 64.0f)] private float maxScale = 0.99f;

	private bool _isGrowing;

	// Scales relative to the serialized min and max members.
	private Vector3 _minLocalScale;
	private Vector3 _maxLocalScale;

	protected override void Awake()
	{
		base.Awake();

		_scaleCoroutineCallback.OnFinished += Pulse;
		_minLocalScale = Vector3.Max(new Vector3(0.1f, 0.1f, 0.1f), minScale * transform.localScale);
		_maxLocalScale = maxScale * transform.localScale;

		enabled = startEnabled;
	}

	void Start()
	{
		if (startEnabled)
		{
			Pulse();
		}
	}

	protected void StopPulsing()
	{
		StopScale();
		_scaleCoroutineCallback.OnFinished -= Pulse;
	}

	private void Pulse()
	{
		if (_isGrowing)
		{
			ScaleToTargetOverTime(_maxLocalScale, scaleTime);
		}
		else
		{
			ScaleToTargetOverTime(_minLocalScale, scaleTime);
		}

		// Flip the growing flag as this class just dumbly grows and shrinks.
		_isGrowing = !_isGrowing;
	}
}
