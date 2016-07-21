using UnityEngine;
using UnityEngine.UI;
using ZMConfiguration;
using ZMPlayer;

[RequireComponent(typeof(Text))]
public class ZMPlayerLabelController : ZMFudgeParentToObject
{
	private Text _text;

	protected override void Awake()
	{
		base.Awake();

		_text = GetComponent<Text>();

		AcceptEvents();
	}

	public override void ConfigureItemWithID(Transform parent, int id)
	{
		base.ConfigureItemWithID(parent, id);

		_text.text = string.Format("P{0}", _playerInfo.ID + 1);
		_text.color = Color.white;
	}

	private void Deactivate(ZMPlayerInfoEventArgs args)
	{
		ClearEvents();
		Destroy(gameObject);
	}

	private void AcceptEvents()
	{
		ZMLobbyScoreController.OnReachMaxScore += Deactivate;
		ZMLobbyController.OnPlayerDropOut += Deactivate;

		ZMPlayerController.OnPlayerDeath += Deactivate;
		ZMPlayerController.OnPlayerRespawn += HandleOnPlayerRespawn;
	}

	private void ClearEvents()
	{
		ZMLobbyScoreController.OnReachMaxScore -= Deactivate;
		ZMLobbyController.OnPlayerDropOut -= Deactivate;

		ZMPlayerController.OnPlayerDeath -= Deactivate;
		ZMPlayerController.OnPlayerRespawn -= HandleOnPlayerRespawn;
	}

	private void HandleOnPlayerRespawn(ZMPlayerControllerEventArgs args)
	{
		if (_playerInfo == args.controller.PlayerInfo)
		{
			_text.enabled = true;
		}
	}
}
