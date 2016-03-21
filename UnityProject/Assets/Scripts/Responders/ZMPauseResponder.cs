using UnityEngine;
using Core;

public class ZMPauseResponder : ZMResponder
{
	[SerializeField] private bool _activeOnPause;

	protected override void Awake()
	{
		base.Awake();

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


}
