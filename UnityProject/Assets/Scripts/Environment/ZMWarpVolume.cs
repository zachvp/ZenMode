using UnityEngine;

public class ZMWarpVolume : MonoBehaviour
{
	public ZMWarpVolume Sibling { get { return _sibling; } }

	private ZMWarpVolume _sibling;
	private Vector3 _forwardPrime;

	private const float _siblingAxisThreshold = 64.0f;

	// Position relative to sibling.
	private bool _isLeft,
				 _isBelow;
	
	void Start()
	{
		// Cast a ray along forward vector to find the most immediate warp volume.
		var dir = new Vector2(_forwardPrime.x, _forwardPrime.y);
		var casts = CheckDirection(dir);

		_forwardPrime = ComputeForwardPrime();

		for (int i = 0; i < casts.Length; ++i)
		{
			ZMWarpVolume checkSibling = casts[i].collider.GetComponent<ZMWarpVolume>();

			// (Could add transform forward check).
			if (checkSibling != null && checkSibling != this)
			{
				// Found the sibling volume.
				_sibling = checkSibling;
//				Debug.Log(string.Format("{0} found sibling: {1}", name, checkSibling.name));
			}
		}

		// Kind of a brittle system, but works for now.
		// Depends on X-axis in editor be pointed in proper forward direction.
//		var fudge = 0.1f;
//		var threshhold = 0.99;
//
//		_isLeft = Vector3.Dot(Vector3.right, _forwardPrime) + fudge > threshhold;
//		_isBelow = Vector3.Dot(Vector3.up, _forwardPrime) + fudge > threshhold;
	}

	// TODO: Warp object should be transform instead.
	public void Warp(GameObject warpObject)
	{
		var toWarpObj = warpObject.transform - transform;


	}

	public static Vector3 ExtractTranslationFromMatrix(ref Matrix4x4 matrix)
	{
		Vector3 translate;

		translate.x = matrix.m03;
		translate.y = matrix.m13;
		translate.z = matrix.m23;

		return translate;
	}

	RaycastHit2D[] CheckDirection(Vector2 dir)
	{
		var origin = new Vector2(transform.position.x, transform.position.y);

		return Physics2D.RaycastAll(origin, dir);
	}

	Vector3 ComputeForwardPrime()
	{
		return transform.right;
	}
}
