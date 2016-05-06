using UnityEngine;
using System.Collections;

public class ZMVibrate : MonoBehaviour {
	private Vector2 _basePosition;
	private bool _shouldVibrate = false;

	// Use this for initialization
	void Start () {
		//_basePosition = this.GetComponent<RectTransform> ().anchoredPosition;
	}
	
	// Update is called once per frame
	void Update () {
		if (_shouldVibrate) {
			//this.GetComponent<RectTransform> ().anchoredPosition = _basePosition + new Vector2(Random.Range (-2, 2), Random.Range (-2, 2));
		}
	}

	public void VibrateStart() {
		_shouldVibrate = true;
	}

	public void VibrateStop() {
		_shouldVibrate = false;
		//this.GetComponent<RectTransform> ().anchoredPosition = _basePosition;
	}
}
