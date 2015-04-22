using UnityEngine;
using System.Collections;

public class ZMTrailerCameraController : MonoBehaviour {
	public float speed = 128.0f;
	public Transform[] focusPoints;

	private Vector3 _movePosition;
	private int _focusIndex = 0;

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
		}

		if (Input.GetKey(KeyCode.Alpha1)) {
			transform.position = focusPoints[0].position;
			camera.orthographicSize = 90;
		} else if (Input.GetKey(KeyCode.Alpha2)) {
			transform.position = focusPoints[1].position;
			camera.orthographicSize = 90;
		} else if (Input.GetKey(KeyCode.Alpha3)) {
			transform.position = focusPoints[2].position;
			camera.orthographicSize = 200;
		}
	}


}
