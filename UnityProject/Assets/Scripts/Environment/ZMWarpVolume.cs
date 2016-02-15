using UnityEngine;

public class ZMWarpVolume : MonoBehaviour
{
	public ZMWarpVolume Sibling { get { return _sibling; } }
	public Vector3 ForwardPrime { get { return _forwardPrime; } }

	private ZMWarpVolume _sibling;
	private Vector3 _forwardPrime;

	private const float _siblingAxisThreshold = 64.0f;

	// Position relative to sibling.
	private bool _isLeft,
				 _isBelow;
	
	void Start()
	{
		_forwardPrime = ComputeForwardPrime();

		// Cast a ray along forward vector to find the most immediate warp volume.
		var dir = new Vector2(_forwardPrime.x, _forwardPrime.y);
		var casts = CheckDirection(dir);

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
	}

	// TODO: Warp object should be transform instead.
	public Vector3 GetWarpPosition(BoxCollider2D warpCollider)
	{
		// Vector from the warp volume position to the object to warp.
		var toWarpObj = warpCollider.transform.position - transform.position;

		// Projected vector of toWarpObj onto this warp volume's up axis.
		var axisOffset = Vector3.Dot(toWarpObj, transform.up) * transform.up;

		// Need to offset the object by its own scale and the sibling scale so warp object does not spawn intersecting
		// the sibling volume.
		var scaleOffset = Sibling.transform.localScale.x + warpCollider.size.x;
		var totalOffset = scaleOffset * Sibling.ForwardPrime + axisOffset;

		return Sibling.transform.position + totalOffset;
		// Warp the object to this volume's sibling using the previous offset calculations.
//		warpObject.transform.position = Sibling.transform.position + totalOffset;
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
