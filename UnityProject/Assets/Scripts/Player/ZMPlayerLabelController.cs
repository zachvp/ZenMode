using UnityEngine;
using UnityEngine.UI;
using ZMConfiguration;

[RequireComponent(typeof(Text))]
public class ZMPlayerLabelController : MonoBehaviour
{
	public Transform parent;

	private ZMPlayer.ZMPlayerInfo _playerInfo;
	private ZMPlayerController _controller;
	private Text _text;
	
	void Start()
	{
		_controller = parent.GetComponent<ZMPlayerController>();
		_text = GetComponent<Text>();
		_playerInfo = GetComponent<ZMPlayer.ZMPlayerInfo>();
	}

	void Update()
	{
		if (_controller && _text)
		{
			_text.enabled = _playerInfo.ID < Settings.MatchPlayerCount.value && !_controller.IsDead();
		}
	}
}
