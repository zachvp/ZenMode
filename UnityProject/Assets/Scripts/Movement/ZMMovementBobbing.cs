using UnityEngine;
using System.Collections;

public class ZMMovementBobbing : MonoBehaviour {
	public float amplitude = 2.0f;
	public float speed	   = 0.1f;

	private float _theta;
	private Vector3 _basePosition;
	private Vector3 _updatedPosition;

	// Use this for initialization
	void Awake () {
		_theta = 0;
	}

	void Start() {
		_updatedPosition = _basePosition = transform.position;
	}
	
	// Update is called once per frame
	void Update () {
		_updatedPosition.y = _basePosition.y + amplitude * Mathf.Sin(_theta);

		transform.position = _updatedPosition;

		_theta += speed * Time.deltaTime;
		_theta %= 360;
	}
}
