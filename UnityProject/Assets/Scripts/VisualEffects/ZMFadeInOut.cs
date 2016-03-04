using UnityEngine;
using UnityEngine.UI;

public class ZMFadeInOut : MonoBehaviour
{
	[SerializeField] private bool startVisible = true;
	[SerializeField] private bool startFading = true;
	[SerializeField] private bool destroyOnFadeComplete;

	[Range (0.001f, 1.0f)] public float maxAlpha = 1;
	[Range (0.001f, 1.0f)] public float minAlpha = 0;

	public float fadeSpeed = 0.2f;

	private Color _fadeColor;
	private Image _image;
	private SpriteRenderer _renderer;

	private bool _fadingIn;
	private int _currentFrame;
	private int _fadeFrame;

	// Use this for initialization
	void Start ()
	{
		_image = GetComponent<Image>();
		_renderer = GetComponent<SpriteRenderer>();

		_currentFrame = 0;
		_fadeFrame = 2;
		_fadingIn = startFading;

		_fadeColor.a = startVisible ? 1.0f : 0.0f;

		if (_image != null) { _fadeColor = _image.color; }
		if (_renderer != null) { _fadeColor = _renderer.color; }

		maxAlpha = Mathf.Min(maxAlpha, 1.0f);
		minAlpha = Mathf.Max(minAlpha, 0.0f);
	}
	
	void Update ()
	{
		if (_fadingIn)
		{
			if (_fadeColor.a < maxAlpha) {
				if (_currentFrame > _fadeFrame) {
					_fadeColor.a += (fadeSpeed * Time.deltaTime) / 10.0f;

					if (_image != null) { _image.color = _fadeColor; }
					if (_renderer != null) { _renderer.color = _fadeColor; }

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
					_fadeColor.a -= (fadeSpeed * Time.deltaTime) / 10.0f;


					if (_image != null) { _image.color = _fadeColor; }
					if (_renderer != null) { _renderer.color = _fadeColor; }
					
					_currentFrame = 0;
				} else {
					_currentFrame += 1;
				}
			} else {
				if (destroyOnFadeComplete)
				{
					Destroy(gameObject);
				}
				else
				{
					_fadingIn = true;
				}
			}
		}
	}
}
