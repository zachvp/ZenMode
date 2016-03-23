using UnityEngine;
using System.Collections;

public class ZMToggleWithPedestal : MonoBehaviour {

	// Use this for initialization
	void Awake () {
		ZMPedestalController.OnActivateEvent += HandleActivateEvent;
		ZMPedestalController.OnDeactivateEvent += HandleDeactivateEvent;

		GetComponent<Light>().enabled = false;
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void HandleActivateEvent(ZMPedestalController pedestalController) {
		Debug.Log("toggle activate");
		enabled = true;
	}

	void HandleDeactivateEvent(ZMPedestalController pedestalController) {
		GetComponent<Light>().enabled = false;
	}
}
