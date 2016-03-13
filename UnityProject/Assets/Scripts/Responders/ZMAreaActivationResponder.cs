using UnityEngine;
using ZMPlayer;

public class ZMAreaActivationResponder : ZMPlayerItem
{
	// Objects that will be activated when the trigger is entered
	public GameObject[] activateObjects;

	// Objects that will be deactivated when the trigger is entered
	public GameObject[] deactivateObjects;

	protected override void Awake()
	{
		base.Awake();

		ToggleObjectsActivation(activateObjects, false);
	}

	void OnTriggerEnter2D(Collider2D collider)
	{
		var checkPlayer = collider.GetComponent<ZMPlayerInfo>();

		if (_playerInfo == checkPlayer)
		{
			ToggleObjectsActivation(activateObjects, true);
			ToggleObjectsActivation(deactivateObjects, false);
		}
	}

	private void ToggleObjectsActivation(GameObject[] objects, bool active)
	{
		for (int i = 0; i < objects.Length; ++i)
		{
			objects[i].SetActive(active);
		}
	}
}
