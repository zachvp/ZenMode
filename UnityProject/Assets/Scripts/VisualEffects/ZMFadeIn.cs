using UnityEngine;
using UnityEngine.UI;
using ZMPlayer;

public class ZMFadeIn : MonoBehaviour {
	public Image fadedImage;
	public float interval = 0.2f;
	public float transitionUpperLimit = 100.0f;

	// delegates
	public delegate void MaxFadeAction(); public static event MaxFadeAction MaxFadeEvent;

	// Use this for initialization
	void Start () {
		Color newcolor = fadedImage.color;
		newcolor.a = 0;

		fadedImage.color = newcolor;
	}

	void OnDestroy() {
		MaxFadeEvent = null;
	}
	
	// Update is called once per frame
	void Update () {
		Color newcolor = fadedImage.color;

		newcolor.a += interval;
		fadedImage.color += newcolor * Time.deltaTime;

		if (fadedImage.color.a >= transitionUpperLimit) {
			if (MaxFadeEvent != null) {
				MaxFadeEvent();
			}
		}
	}
}
