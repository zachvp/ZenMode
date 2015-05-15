using UnityEngine;
using UnityEngine.UI;

public class ZMDisableRenderer : MonoBehaviour {

	// Use this for initialization
	void Start () {
		Image image = GetComponent<Image>();

		if (renderer != null) {
			renderer.enabled = false;
		}

		if (image != null) {
			image.enabled = false;
		}
	}
}
