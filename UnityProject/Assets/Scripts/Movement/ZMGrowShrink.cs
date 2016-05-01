using UnityEngine;
using System.Collections;
using Core;

public class ZMGrowShrink : MonoBehaviour
{
	[SerializeField] private bool startEnabled;
	[SerializeField] private float scaleTime = 2.0f;
	[SerializeField] [Range(0.0f, 0.99f)] private float minScale = 0.5f;
	[SerializeField] [Range(0f, 64.0f)] private float maxScale = 0.99f;

	private bool _isGrowing;

	private Vector3 _minLocalScale;
	private Vector3 _maxLocalScale;

	private CoroutineCallback _scaleCoroutineCallback;

	void Awake()
	{
		_minLocalScale = Vector3.Max(new Vector3(0.1f, 0.1f, 0.1f), minScale * transform.localScale);
		_maxLocalScale = maxScale * transform.localScale;
		_scaleCoroutineCallback = new CoroutineCallback(Scale);

		enabled = startEnabled;
	}

	void Start()
	{
		Scale();
	}

	private void Scale()
	{		
		if (_isGrowing)
		{
			_scaleCoroutineCallback.coroutine = StartCoroutine(ScaleToTarget(transform.localScale,
																			 _maxLocalScale, scaleTime));
		}
		else
		{
			_scaleCoroutineCallback.coroutine = StartCoroutine(ScaleToTarget(transform.localScale,
															   _minLocalScale, scaleTime));
		}

		// Flip the growing flag as this class just dumbly grows and shrinks.
		_isGrowing = !_isGrowing;
	}

	// Scales this object's transform from the start scale to the end scale.
	private IEnumerator ScaleToTarget(Vector3 start, Vector3 end, float totalTime)
	{
		// Initialize the t lerp parameter.
		float t = 0;

		while (t < totalTime)
		{
			// Each frame, this while loop will go through one iteration (note the yield return null).
			// Each loop iteration interpolates the attached object's scale.
			t += Time.deltaTime;
			transform.localScale = Vector3.Lerp(start, end, t / totalTime);

			yield return null;
		} 

		// Snap our scale to the desired scale.
		// Flag that the object is fully scaled.
		transform.localScale = end;
		_scaleCoroutineCallback.OnFinished();
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
