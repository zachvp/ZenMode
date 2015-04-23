using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ZMFadeInOut : MonoBehaviour {
	public bool startVisible = true;
	public float maxAlpha = 1;
	public float minAlpha = 0;
	public float fadeSpeed = 0.2f;

	private Color _fadeColor;
	private Image _image;
	private bool _fadingIn = true;
	private int _currentFrame;
	private int _fadeFrame;

	// Use this for initialization
	void Start () {
		_image = GetComponent<Image>();
		_currentFrame = 0;
		_fadeFrame = 5;

		_fadeColor = _image.color;
		_fadeColor.a = startVisible ? 1.0f : 0.0f;
		_image.color = _fadeColor;
		_fadeColor = _image.color;

		maxAlpha = Mathf.Min(maxAlpha, 1.0f);
	}
	
	// Update is called once per frame
	void Update () {
		if (_fadingIn) {
			if (_fadeColor.a < maxAlpha) {
				if (_currentFrame > _fadeFrame) {
					_fadeColor.a += fadeSpeed * Time.deltaTime;
					_image.color = _fadeColor;

					_currentFrame = 0;
				} else {
					_currentFrame += 1;
				}
			} else {
				_fadingIn = false;
			}
		} else {
			if (_fadeColor.a > minAlpha) {
				if (_currentFrame > _fadeFrame) {
					_fadeColor.a -= fadeSpeed * Time.deltaTime;
					_image.color = _fadeColor;
					
					_currentFrame = 0;
				} else {
					_currentFrame += 1;
				}
			} else {
				_fadingIn = true;
			}
		}
	}
}
