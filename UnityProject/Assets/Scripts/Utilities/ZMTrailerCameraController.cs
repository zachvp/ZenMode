using UnityEngine;
using System.Collections;

public class ZMTrailerCameraController : MonoBehaviour {
	public float speed = 128.0f;
	public Transform[] focusPoints;
	public AudioClip mainAudio;

	private Vector3 _movePosition;
	private int _focusIndex = 0;
	private bool _toggleMusic;

	private Vector3 _basePos;
	private float _baseZoom;

	void Start() {
		_basePos = transform.position;
		_baseZoom = camera.orthographicSize;
	}

	// Update is called once per frame
	void Update () {
		if (Input.GetKey(KeyCode.H)) {
			_movePosition = transform.position;
			_movePosition.x += speed * Time.deltaTime;

			transform.position = _movePosition;
		} else if (Input.GetKey(KeyCode.F)) {
			_movePosition = transform.position;
			_movePosition.x -= speed * Time.deltaTime;
			
			transform.position = _movePosition;
		} else if (Input.GetKey(KeyCode.T)) {
			_movePosition = transform.position;
			_movePosition.y += speed * Time.deltaTime;
			
			transform.position = _movePosition;
		} else if (Input.GetKey(KeyCode.G)) {
			_movePosition = transform.position;
			_movePosition.y -= speed * Time.deltaTime;
			
			transform.position = _movePosition;
		} else if (Input.GetKey(KeyCode.I)) {
			Application.LoadLevel(Application.loadedLevel);
		}

		if (Input.GetKeyDown(KeyCode.Alpha1)) {
			transform.position = focusPoints[0].position;
			camera.orthographicSize = 90;
		} else if (Input.GetKeyDown(KeyCode.Alpha2)) {
			transform.position = focusPoints[1].position;
			camera.orthographicSize = 90;
		} else if (Input.GetKeyDown(KeyCode.Alpha3)) {
			transform.position = focusPoints[2].position;
			camera.orthographicSize = 200;
		} else if (Input.GetKeyDown(KeyCode.Alpha4)) {
			_movePosition = focusPoints[3].position;
			_movePosition.z = -192;

			transform.position = _movePosition;
			camera.orthographicSize = 100;
		} else if (Input.GetKeyDown(KeyCode.Alpha0)) {
			_movePosition = Vector3.zero;
			_movePosition.z = -192;

			transform.position = _movePosition;
			camera.orthographicSize = 432;
		} else if (Input.GetKeyDown(KeyCode.Alpha9)) {
			_movePosition = _basePos;
			
			transform.position = _movePosition;
			camera.orthographicSize = _baseZoom;
		}

		if (Input.GetKeyDown(KeyCode.U)) {
			if (!_toggleMusic)
				audio.Play();
			else
				audio.Stop();

			_toggleMusic = !_toggleMusic;
		}
	}
}
