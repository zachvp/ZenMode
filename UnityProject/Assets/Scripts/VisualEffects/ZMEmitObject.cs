using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ZMEmitObject : MonoBehaviour {
	public GameObject emitObject;
	public Color color;
	public int interval = 10;

	private int _currentFrame = 0;
	
	// Update is called once per frame
	void Update () {
		if (_currentFrame > interval) {
			emitObject = GameObject.Instantiate(emitObject,
			                        	  transform.position,
			                         	  emitObject.transform.rotation) as GameObject;

			emitObject.GetComponent<Text>().color = color;
			emitObject.transform.SetParent(transform);

			_currentFrame = 0;
		}

		_currentFrame += 1;
	}
}
