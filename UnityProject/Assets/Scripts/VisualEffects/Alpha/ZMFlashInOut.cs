using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ZMFlashInOut : MonoBehaviour {

	private Text _text;
	private bool _isVisible;

	// Use this for initialization
	void Start () {
		_text = GetComponent<Text>();
		_isVisible = true;
		InvokeRepeating ("Flash", 0.0f, 0.1f);
	}

	void Flash () {
		Color fadeColor = _text.color;
		fadeColor.a = (_isVisible ? 0.66f : 0.0f);
		_text.color = fadeColor;
		_isVisible = !_isVisible;
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
