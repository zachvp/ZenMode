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

		ZMLobbyScoreController.OnMaxScoreReached += Deactivate;

		ZMPlayerController.PlayerDeathEvent += Deactivate;
		ZMPlayerController.PlayerRespawnEvent += HandleOnPlayerRespawn;
	}

	public override void ConfigureItemWithID(Transform parent, int id)
	{
		base.ConfigureItemWithID(parent, id);

		_text.text = string.Format("P{0}", _playerInfo.ID + 1);
		_text.color = Color.white;
	}

	private void Deactivate(ZMPlayerInfo info)
	{
		if (_playerInfo == info) { _text.enabled = false; }
	}

	private void HandleOnPlayerRespawn(ZMPlayerController controller)
	{
		if (_playerInfo == controller.PlayerInfo) { _text.enabled = true; }
	}
}
