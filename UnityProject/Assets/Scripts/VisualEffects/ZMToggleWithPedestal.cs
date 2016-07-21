using UnityEngine;
using Core;

public class ZMToggleWithPedestal : MonoBehaviour
{
	void Awake ()
	{
		ZMPedestalController.OnActivateEvent += HandleActivateEvent;
		ZMPedestalController.OnDeactivateEvent += HandleDeactivateEvent;

		GetComponent<Light>().enabled = false;
	}

	void HandleActivateEvent(MonoBehaviourEventArgs args)
	{
		Debug.Log("toggle activate");
		enabled = true;
	}

	void HandleDeactivateEvent(MonoBehaviourEventArgs args)
	{
		GetComponent<Light>().enabled = false;
	}
}
