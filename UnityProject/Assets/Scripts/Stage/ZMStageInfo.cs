using UnityEngine;
using UnityEngine.UI;
using ZMConfiguration;

public class ZMStageInfo : MonoBehaviour
{
	public Rect StageRect { get { return _stageRect; } }
	public Transform Origin { get { return _origin; } }

	private Camera _camera;

	public static ZMStageInfo Instance
	{
		get
		{
			Debug.Assert(_instance != null, "ZMStageInfo: Instance is null.");
			return _instance;
		}
	}

	private static ZMStageInfo _instance;

	private Transform _origin;
	private Rect _stageRect;

	void Awake()
	{
		Debug.Assert(_instance == null, "ZMStageInfo: Instance already exists in scene.");
		_instance = this;

		ZMCameraController.OnCameraStart += HandleCameraStart;
	}

	void Start()
	{
		_origin = GameObject.FindGameObjectWithTag(Tags.kOrigin).transform;
	}

	void OnDestroy()
	{
		_instance = null;
	}

//	void OnDrawGizmos()
//	{
//		if (_stageRect != null)
//		{
//			Gizmos.color = Color.green;
//			Gizmos.DrawWireCube(_stageRect.center, new Vector3(_stageRect.size.x, _stageRect.size.y, 4.0f));
//		}
//	}

	private void HandleCameraStart(Camera camera)
	{
		_stageRect = GetStageRect(camera, _origin);
	}

	private Rect GetStageRect(Camera camera, Transform origin)
	{
		// Get screen dimensions.
		var height = 2.0f * camera.orthographicSize;
		var width = height * Screen.width / Screen.height;
		var size = new Vector2(width, height);

		// Get vectors from origin to bottommost & leftmost points of the screen.
		var widthOffset = origin.position - (width / 2.0f) * Vector3.right;
		var heightOffset = origin.position + (height / 2.0f) * Vector3.down;
		var corner = _origin.position + widthOffset + heightOffset;

		return new Rect(corner, size);
	}
}
