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
		gameObject.SetActive(false);

		ZMLobbyScoreController.OnReachMaxScore += Activate;
	}

	protected override void Start()
	{
		base.Start();

		ConfigureItemWithID(_playerInfo.ID);
	}

	private void Activate(ZMPlayerInfoEventArgs args)
	{
		if (_playerInfo == args.info)
		{
			_text.text = message;
			gameObject.SetActive(true);
		}
	}
}
