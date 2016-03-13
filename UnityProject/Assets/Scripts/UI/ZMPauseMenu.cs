using UnityEngine;
using Core;
using ZMPlayer;

public class ZMPauseMenu : ZMTextMenu
{
	public static EventHandler<ZMPlayerInfo> OnPlayerPauseGame;

	protected bool _active;

	protected override void Awake()
	{
		base.Awake();

		MatchStateManager.OnMatchEnd += ClearActivationEvents;
	}

	void OnDestroy()
	{
		OnPlayerPauseGame = null;
	}

	private void HandleTogglePause(int controlIndex)
	{
		if (_active && IsValidInputControl(controlIndex))
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
		ClearInputEvents();
	}

	protected override void ToggleActive(bool active)
	{
		base.ToggleActive(active);

		_active = active;
		gameObject.SetActive(_active);
	}

	protected void PauseGame()
	{
		if (IsAbleToPause())
		{
			AcceptInputEvents();
			ShowMenu();

			Notifier.SendEventNotification(OnPlayerPauseGame, _playerInfo);
			MatchStateManager.PauseMatch();
		}
	}
	
	protected void ResumeGame()
	{
		if (IsAbleToPause())
		{
			ClearInputEvents();
			ToggleActive(false);
			MatchStateManager.ResumeMatch();
		}
	}

	protected virtual bool IsAbleToPause()
	{
		return MatchStateManager.IsMain() || MatchStateManager.IsPause();
	}
}
