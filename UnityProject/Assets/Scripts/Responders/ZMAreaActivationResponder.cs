using UnityEngine;

public class ZMAreaActivationResponder : MonoBehaviour {
	// objects that will be activated when the trigger is entered
	public GameObject[] activateObjects;

	// objects that will be deactivated when the trigger is entered
	public GameObject[] deactivateObjects;

	void Start() {
		ToggleObjectsActivation(activateObjects, false);
	}

	void OnTriggerEnter2D(Collider2D collider) {
		ToggleObjectsActivation(activateObjects, true);
		ToggleObjectsActivation(deactivateObjects, false);
	}

	private void ToggleObjectsActivation(GameObject[] objects, bool active) {
		for (int i = 0; i < objects.Length; ++i) {
			objects[i].SetActive(active);
		}
	}
}
