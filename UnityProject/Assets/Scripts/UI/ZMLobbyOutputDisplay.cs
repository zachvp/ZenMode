using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class ZMLobbyOutputDisplay : MonoBehaviour
{
	private Text _text;

	void Awake()
	{
		_text = GetComponent<Text>();
		_text.enabled = false;

		ZMLobbyController.PauseGameEvent += HandlePauseGameEvent;
		ZMLobbyController.ResumeGameEvent += HandleResumeGameEvent;
	}

	void HandleResumeGameEvent()
	{
		_text.enabled = false;
	}

	void HandlePauseGameEvent (int playerIndex)
	{
		_text.text = "P" + (playerIndex + 1) + " PAUSED";
		_text.enabled = true;
	}
}
