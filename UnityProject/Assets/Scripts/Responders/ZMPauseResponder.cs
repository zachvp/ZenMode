using UnityEngine;
using Core;

public class ZMPauseResponder : MonoBehaviour
{
	[SerializeField] private bool _activeOnPause;

	void Awake()
	{
		MatchStateManager.OnMatchPause += HandleMatchPause;
		MatchStateManager.OnMatchResume += HandleMatchResume;

		SetActive(!_activeOnPause);
	}

	private void HandleMatchPause()
	{
		SetActive(_activeOnPause);
	}

	private void HandleMatchResume()
	{
		SetActive(!_activeOnPause);
	}

	private void SetActive(bool active)
	{
		gameObject.SetActive(active);
	}
}
