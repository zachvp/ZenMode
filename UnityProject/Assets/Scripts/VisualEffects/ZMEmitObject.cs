using UnityEngine;
using System.Collections;

public class ZMEmitObject : MonoBehaviour {
	public GameObject emitObject;
	public int interval = 10;

	private int _currentFrame = 0;

	void Awake() {
//		enabled = false;

//		Invoke("Enable", 5);
	}
	
	// Update is called once per frame
	void Update () {
		if (_currentFrame > interval) {
			emitObject = GameObject.Instantiate(emitObject,
			                                    transform.position,
			                                    emitObject.transform.rotation) as GameObject;

			if (emitObject != null)
				emitObject.transform.SetParent(transform);

			_currentFrame = 0;
		}

		_currentFrame += 1;
	}

	void Enable() {
		enabled = true;
	}
}
