using UnityEngine;
using Core;

public class ZMPausable : MonoBehaviour
{
	[SerializeField] private bool affectsHierarchy;

	void Awake()
	{
		MatchStateManager.OnMatchPause += HandleMatchPause;
		MatchStateManager.OnMatchResume += HandleMatchResume;
	}

	private void HandleMatchPause()
	{
		SetActive(false);
	}

	private void HandleMatchResume()
	{
		SetActive(true);
	}

	private void SetActive(bool active)
	{
		if (affectsHierarchy) { Utilities.SetEnabledHierarchy(gameObject, active); }
		else { Utilities.SetEnabled(gameObject, active); }
	}
}
