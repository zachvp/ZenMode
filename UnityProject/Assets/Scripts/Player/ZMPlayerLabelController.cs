using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ZMPlayerLabelController : MonoBehaviour {
	public Transform parent;
	
	void Update () {
		ZMPlayerController controller = parent.GetComponent<ZMPlayerController> ();
		Text text = GetComponent<Text> ();
		if (controller && text) {
			text.enabled = (controller.isActiveAndEnabled && controller.light.enabled);
		}
	}
}
