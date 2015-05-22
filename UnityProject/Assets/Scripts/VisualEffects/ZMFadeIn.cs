using UnityEngine;
using UnityEngine.UI;
using ZMPlayer;

public class ZMFadeIn : MonoBehaviour {
	public enum FadeMode { FADE_IN, FADE_OUT }; public FadeMode fadeMode;
	public float interval = 1;
	public float fadeLimit = 100.0f;

	// members
	private bool _fading;

	// delegates
	public delegate void FadeLimitAction(); public static event FadeLimitAction FadeLimitEvent;

	// references
	private MaskableGraphic _maskableGraphic;

	void Awake() {
		Color baseColor;

		_fading = true;
		_maskableGraphic = GetComponent<MaskableGraphic>();

		if (_maskableGraphic == null) {
			Debug.Log(gameObject.name + ": No maskable graphic attached!");
		}

		baseColor = new Color(_maskableGraphic.color.r, _maskableGraphic.color.g, _maskableGraphic.color.b, 1);
		_maskableGraphic.color = baseColor;
	}

	void Start () {
		Color newcolor = _maskableGraphic.color;

		if (fadeMode == FadeMode.FADE_IN) { 
			newcolor.a = 0;
		}

		_maskableGraphic.color = newcolor;
	}

	void OnDestroy() {
		FadeLimitEvent = null;
	}
	
	// Update is called once per frame
	void Update () {
		Color newcolor = _maskableGraphic.color;

		if (fadeMode == FadeMode.FADE_IN) {
			newcolor.a += interval;
			_maskableGraphic.color += newcolor * Time.deltaTime;

			if (_maskableGraphic.color.a >= fadeLimit) {
				_fading = false;

				if (FadeLimitEvent != null) {
					FadeLimitEvent();
				}
			}
		} else {
			newcolor.a -= interval * Time.deltaTime;
			_maskableGraphic.color = newcolor ;
			
			if (_maskableGraphic.color.a <= fadeLimit) {
				_fading = false;
				Destroy(gameObject);

				if (FadeLimitEvent != null) {
					FadeLimitEvent();
				}
			}
		}

		if (!_fading) {
			enabled = false;
		}
	}
}
