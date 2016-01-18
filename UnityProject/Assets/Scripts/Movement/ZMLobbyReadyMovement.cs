using UnityEngine;
using UnityEngine.UI;
using ZMPlayer;

[RequireComponent(typeof(Text))]
public class ZMLobbyReadyMovement : ZMAnalogMovement
{
	private Text _text;

	private const string message = "Ready!";

	protected override void Awake()
	{
		base.Awake();

		_text = GetComponent<Text>();
		_text.text = "";
		ZMLobbyScoreController.OnMaxScoreReached += Activate;
		gameObject.SetActive(false);
	}

	protected override void Start()
	{
		base.Start();

		ConfigureItemWithID(_playerInfo.ID);
	}

	private void Activate(ZMPlayerInfo info)
	{
		if (_playerInfo == info)
		{
			_text.text = message;
			gameObject.SetActive(true);
		}
	}
}
