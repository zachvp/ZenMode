using UnityEngine;
using UnityEngine.UI;
using ZMConfiguration;

[RequireComponent(typeof(Text))]
public class ZMPlayerLabelController : ZMFudgeParentToObject
{
	private Text _text;
	private ZMPlayerController _controller;

	void Start()
	{
		_text.text = string.Format("P{0}", _playerInfo.ID + 1);
	}

	protected override void Update()
	{
		base.Update();

		if (_controller && _text)
		{
			_text.enabled = _playerInfo.ID < Settings.MatchPlayerCount.value && !_controller.IsDead();
		}
	}

	protected override void InitData(ZMPlayerController controller)
	{
		base.InitData(controller);

		if (_playerInfo == controller.PlayerInfo)
		{
			_controller = controller;
			_text = GetComponent<Text>();
		}		
	}
}
