using UnityEngine;
using System.Collections;

public class ZMToggleWithPedestal : MonoBehaviour {

	// Use this for initialization
	void Awake () {
		ZMPedestalController.ActivateEvent += HandleActivateEvent;
		ZMPedestalController.DeactivateEvent += HandleDeactivateEvent;

		light.enabled = false;
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void HandleActivateEvent(ZMPedestalController pedestalController) {
		Debug.Log("toggle activate");
		enabled = true;
	}

	void HandleDeactivateEvent(ZMPedestalController pedestalController) {
		light.enabled = false;
	}
}
