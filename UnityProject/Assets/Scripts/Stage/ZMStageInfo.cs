using UnityEngine;
using UnityEngine.UI;
using ZMConfiguration;
using Core;

public class ZMStageInfo : MonoSingleton<ZMStageInfo>
{
	public Rect StageRect { get { return _stageRect; } }
	public Transform Origin { get { return _origin; } }

	private Camera _camera;

	private Transform _origin;
	private Rect _stageRect;

	protected override void Awake()
	{
		base.Awake();

		ZMCameraController.OnCameraStart += HandleCameraStart;
	}

	void Start()
	{
		_origin = GameObject.FindGameObjectWithTag(Tags.kOrigin).transform;
	}

//	void OnDrawGizmos()
//	{
//		if (_stageRect != null)
//		{
//			Gizmos.color = Color.green;
//			Gizmos.DrawWireCube(_stageRect.center, new Vector3(_stageRect.size.x, _stageRect.size.y, 4.0f));
//		}
//	}

	private void HandleCameraStart(UnityObjectEventArgs args)
	{
		var camera = args.arg as Camera;

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
