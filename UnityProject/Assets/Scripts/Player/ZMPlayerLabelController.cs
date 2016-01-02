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

		ZMPlayerManager.Instance.OnPlayerDeath += HandleOnPlayerDeath;
		ZMPlayerManager.Instance.OnPlayerRespawn += HandleOnPlayerRespawn;
	}

	protected void Start()
	{
		_text.text = string.Format("P{0}", _playerInfo.ID + 1);
	}

	protected override void Update()
	{
		base.Update();

		if (_playerController && _text)
		{
			_text.enabled = _playerInfo.ID < Settings.MatchPlayerCount.value && !_playerController.IsDead();
		}
	}

	protected override void InitData(ZMPlayerController controller)
	{
		base.InitData(controller);

		_text = GetComponent<Text>();
	}

	private void HandleOnPlayerDeath(ZMPlayerInfo info)
	{
		if (_playerInfo == info) { _text.enabled = false; }
	}

	private void HandleOnPlayerRespawn(ZMPlayerController controller)
	{
		if (_playerInfo == controller.PlayerInfo) { _text.enabled = true; }
	}
}
