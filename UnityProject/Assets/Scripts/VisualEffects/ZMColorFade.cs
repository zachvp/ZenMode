using UnityEngine;
using UnityEngine.UI;
using ZMPlayer;

public class ZMColorFade : MonoBehaviour {
	public bool looping = false;
	public float fadeSpeed = 3f;
	public Color fadeColor = Color.white;

	private ZMPlayerInfo _playerInfo; public ZMPlayerInfo PlayerInfo { get { return _playerInfo; } }
	private Image _image;
	private Color _baseColor;
	private bool _fadingIn;

	private const float TARGET_THRESHOLD = 0.005f;

	void Awake () {
		_playerInfo = GetComponent<ZMPlayerInfo>();
		_image = GetComponentsInChildren<Image>()[1]; // HACKED!

		_baseColor = _image.color;

		ZMScoreController.CanScoreEvent += HandleCanScoreEvent;
		ZMScoreController.StopScoreEvent += HandleStopScoreEvent;
	}

	void HandleStopScoreEvent (ZMPlayer.ZMScoreController scoreController)
	{
		if (scoreController.PlayerInfo.playerTag.Equals(_playerInfo.playerTag)) {
			_fadingIn = false;
		}
	}

	void HandleCanScoreEvent (ZMPlayer.ZMScoreController scoreController)
	{
		if (scoreController.PlayerInfo.playerTag.Equals(_playerInfo.playerTag)) {
			_fadingIn = true;
		}
	}
	
	// Update is called once per frame
	void Update () {
		if (_fadingIn) {
			if (ColorDifference(_image.color, fadeColor) < TARGET_THRESHOLD) {
				if (looping)
					_fadingIn = false;
			} else {
				_image.color = Color.Lerp(_image.color, fadeColor, fadeSpeed * Time.deltaTime);
			}
		} else {
			if (ColorDifference(_image.color, _baseColor) < TARGET_THRESHOLD) {
				if (looping)
					_fadingIn = true;
			} else {
				_image.color = Color.Lerp(_image.color, _baseColor, fadeSpeed * Time.deltaTime);
			}
		}
	}

	float ColorDifference(Color a, Color b) {
		Vector4 va = new Vector4(a.r, a.g, a.b, a.a);
		Vector4 vb = new Vector4(b.r, b.g, b.b, b.a);

		return Vector4.SqrMagnitude(va - vb);
	}
}
