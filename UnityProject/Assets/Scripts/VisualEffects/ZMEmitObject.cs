using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ZMEmitObject : MonoBehaviour {
	public string resourcePath;
	public Color color;
	public int interval = 10;

	private GameObject _resource;
	private int _currentFrame = 0;

	void Awake() {
		ZMGameStateController.GameEndEvent += HandleGameEndEvent;

		_resource = Resources.Load(resourcePath, typeof(GameObject)) as GameObject;
	}

	void HandleGameEndEvent ()
	{
		enabled = false;
	}

	// Update is called once per frame
	void Update () {
		if (_currentFrame > interval) {
			GameObject emitObject = GameObject.Instantiate(_resource) as GameObject;

			emitObject.transform.position = transform.position;
			emitObject.GetComponent<Text>().color = color;
			emitObject.transform.SetParent(transform);

			_currentFrame = 0;
		}

		_currentFrame += 1;
	}

	void OnDestroy() {
		Resources.UnloadUnusedAssets();
	}
}
