using UnityEngine;
using Core;
using ZMPlayer;

public class ZMPauseMenu : ZMTextMenu
{
	public static EventHandler<ZMPlayerInfoEventArgs> OnPlayerPauseGame;

	protected bool _active;

	protected override void Awake()
	{
		base.Awake();

		MatchStateManager.OnMatchEnd += ClearActivationEvents;
	}

	protected override void OnDestroy()
	{
		base.OnDestroy();

		OnPlayerPauseGame = null;
	}

	private void HandleTogglePause(IntEventArgs args)
	{
		if (_active && IsValidInputControl(args.value))
		{
			ResumeGame();
		}
		else if (!_active)
		{
			_playerInfo.ID = args.value;
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
			var infoArgs = new ZMPlayerInfoEventArgs(_playerInfo);

			AcceptInputEvents();
			ShowMenu();

			Notifier.SendEventNotification(OnPlayerPauseGame, infoArgs);
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
