using UnityEngine;
using UnityEngine.UI;
using Core;

public class ZMTimedCounterStage : ZMTimedCounter
{
	public AudioClip audioTick;

	public static EventHandler GameTimerEndedEvent;
	private const float VALUE_WARNING = 30;
	
	protected override void Awake()
	{
		base.Awake();

		MatchStateManager.OnMatchStart += StartTimer;
		MatchStateManager.OnMatchEnd += HandleGameEndEvent;
		MatchStateManager.OnMatchPause += PauseTimer;
		MatchStateManager.OnMatchResume += StartTimer;

		UpdateText();
	}

	protected override void UpdateText()
	{
		base.UpdateText();

		if (_value <= VALUE_WARNING)
		{
			counterUIText.DisplayColor = new Color(0.905f, 0.698f, 0.635f, 0.75f);
			GetComponent<AudioSource>().PlayOneShot(audioTick, (_value <= 10 ? 1.5f : 0.66f));
		}
		else
		{
			counterUIText.DisplayColor = new Color(1.000f, 1.000f, 1.000f, 0.75f);
		}
	}

	protected override void Count()
	{
		base.Count();

		if (_value == min || _value == max)
		{
			PauseTimer();

			Notifier.SendEventNotification(GameTimerEndedEvent);
		}
	}

	private void HandleGameEndEvent()
	{
		PauseTimer();
	}
	
	private void OnDestroy()
	{
		GameTimerEndedEvent = null;
	}
}
