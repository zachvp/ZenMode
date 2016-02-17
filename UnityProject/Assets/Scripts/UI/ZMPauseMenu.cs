using UnityEngine;
using Core;
using ZMPlayer;

public class ZMPauseMenu : ZMTextMenu
{
	[SerializeField] private GameObject _overlay;

	public static EventHandler<ZMPlayerInfo> OnPlayerPauseGame;

	protected bool _active;

	protected override void Awake()
	{
		base.Awake();

		_playerInfo = GetComponent<ZMPlayerInfo>();
	}

	void OnDestroy()
	{
		OnPlayerPauseGame = null;
	}

	private void HandleTogglePause(int controlIndex)
	{
		// Bail out if the match hasn't started yet
		if (!IsAbleToPause(controlIndex)) { return; }

		if (_active && IsCorrectInputControl(controlIndex))
		{
			ResumeGame();
		}
		else if (!_active)
		{
			_playerInfo.ID = controlIndex;
			PauseGame();
		}
	}

	protected override void AcceptActivationEvents()
	{
		ZMGameInputManager.StartInputEvent += HandleTogglePause;
	}

	protected override void ClearActivationEvents()
	{
		ZMGameInputManager.StartInputEvent -= HandleTogglePause;
	}

	protected override void ToggleActive(bool active)
	{
		base.ToggleActive(active);

		_active = active;
		gameObject.SetActive(_active);
		_overlay.SetActive(_active);	// TODO: NOT WORKING??
	}

	protected void PauseGame()
	{
		ShowMenu();

		Notifier.SendEventNotification(OnPlayerPauseGame, _playerInfo);
		MatchStateManager.PauseMatch();
	}
	
	protected void ResumeGame()
	{
		ToggleActive(false);
		MatchStateManager.ResumeMatch();
	}

	protected virtual bool IsAbleToPause(int controlIndex)
	{
		return !MatchStateManager.IsNone();
	}
}
