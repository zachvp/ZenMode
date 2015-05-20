using UnityEngine;
using UnityEngine.UI;

public class ZMDisableRenderer : MonoBehaviour {

	// Use this for initialization
	void Start () {
		Image image = GetComponent<Image>();
		Text text = GetComponent<Text>();

		if (renderer != null) {
			renderer.enabled = false;
		}

		if (image != null) {
			image.enabled = false;
		}

		if (text != null) {
			Color invisible = new Color(text.color.r, text.color.g, text.color.b, 0);
			
			text.color = invisible;
		}
	}
}
