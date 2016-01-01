using UnityEngine;
using UnityEngine.UI;

public class ZMLobbyOutputDisplay : MonoBehaviour
{
	public Text text;

	void Awake()
	{
		text.enabled = false;

		ZMLobbyController.PauseGameEvent += HandlePauseGameEvent;
		ZMLobbyController.ResumeGameEvent += HandleResumeGameEvent;
	}

	void HandleResumeGameEvent()
	{
		text.enabled = false;
	}

	void HandlePauseGameEvent (int playerIndex)
	{
		text.text = "P" + (playerIndex + 1) + " PAUSED";
		text.enabled = true;
	}
}
