using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ZMEmitObject : MonoBehaviour {
	public string resourcePath;
	public Color color;
	public int interval = 10;

	private GameObject _emitObject;
	private int _currentFrame = 0;
	
	// Update is called once per frame
	void Update () {
		if (_currentFrame > interval) {
			GameObject resource = Resources.Load(resourcePath, typeof(GameObject)) as GameObject;

			_emitObject = GameObject.Instantiate(resource) as GameObject;

			_emitObject.transform.position = transform.position;
			_emitObject.GetComponent<Text>().color = color;
			_emitObject.transform.SetParent(transform);

			_currentFrame = 0;
		}

		_currentFrame += 1;
	}

	void OnDestroy() {
		Resources.UnloadUnusedAssets();
	}
}
