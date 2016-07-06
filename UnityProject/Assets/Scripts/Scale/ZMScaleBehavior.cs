using UnityEngine;
using System.Collections;
using Core;

public class ZMScaleBehavior : MonoBehaviour
{
	protected CoroutineCallback _scaleCoroutineCallback;

	protected virtual void Awake()
	{
		_scaleCoroutineCallback = new CoroutineCallback();
	}

	public void Pause()
	{
		enabled = false;
	}

	public void Resume()
	{
		enabled = true;
	}

	public void StopScale()
	{
		if (_scaleCoroutineCallback != null)
		{
			StopCoroutine(_scaleCoroutineCallback.coroutine);
		}
	}

	public virtual CoroutineCallback ScaleToTargetOverTime(Vector3 start, Vector3 end, float growTime)
	{
		_scaleCoroutineCallback.coroutine = StartCoroutine(ScaleToTargetOverTimeInternal(start, end, growTime));

		return _scaleCoroutineCallback;
	}

	public virtual CoroutineCallback ScaleToTargetOverTime(Vector3 target, float growTime)
	{
		return ScaleToTargetOverTime(transform.localScale, target, growTime);
	}

	// Scales this object's transform from the start scale to the end scale.
	private IEnumerator ScaleToTargetOverTimeInternal(Vector3 start, Vector3 end, float totalTime)
	{
		// Initialize the t lerp parameter.
		float t = 0;

		while (t < totalTime)
		{
			// Each frame, this while loop will go through one iteration (note the yield return null).
			// Each loop iteration interpolates the attached object's scale.
			transform.localScale = Vector3.Lerp(start, end, t / totalTime);

			yield return null;

			t += Time.deltaTime;
		} 

		// Snap our scale to the desired scale.
		// Flag that the object is fully scaled.
		transform.localScale = end;
		Notifier.SendEventNotification(_scaleCoroutineCallback.OnFinished);
		yield break;
	}
}
