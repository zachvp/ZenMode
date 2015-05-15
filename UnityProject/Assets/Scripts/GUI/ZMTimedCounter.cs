using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ZMTimedCounter : MonoBehaviour {
	public enum DisplayType { PLAIN, TIME };
	public DisplayType displayType;
	public int startValue;
	public int min, max;
	public int valueIncrement;
	public float timeIncrement;
	public bool shouldClearOnCompletion;
	public bool start;
	public bool juicy;
	public Text counterUIText;
	public string minMessage, maxMessage;
	public AudioClip audioTick;

	private const string kCountMethodName = "Count";
	private int _value;

	public delegate void GameTimerEndedAction(); public static event GameTimerEndedAction GameTimerEndedEvent;
	
	void Awake () {
		_value = startValue;

		ZMGameStateController.StartGameEvent += HandleStartGameEvent;
		ZMGameStateController.GameEndEvent += HandleGameEndEvent;
	}

	void HandleGameEndEvent ()
	{
		CancelInvoke(kCountMethodName);
	}

	void HandleStartGameEvent ()
	{
		BeginCount();
	}

	void OnDestroy() {
		GameTimerEndedEvent = null;
	}

	void Start() {
		UpdateText();
	}

	void UpdateText() {
		if (minMessage != null && _value == min) {
			counterUIText.text = minMessage;
		} else if (maxMessage != null && _value == max) {
			counterUIText.text = maxMessage;
		} else if (displayType == DisplayType.PLAIN) {
			counterUIText.text = _value.ToString();
		} else if (displayType == DisplayType.TIME) {
			int minutes = Mathf.FloorToInt(_value / 60F);
			int seconds = Mathf.FloorToInt(_value - minutes * 60);

			counterUIText.text =  string.Format("{0:0}:{1:00}", minutes, seconds); _value.ToString ();

			if (juicy) {
				if (_value <= 30) {
					counterUIText.color = new Color(0.905f, 0.698f, 0.635f, 0.75f);
					audio.PlayOneShot(audioTick, (_value <= 10 ? 1.0f : 0.25f));
				} else {
					counterUIText.color = new Color(1.000f, 1.000f, 1.000f, 0.75f);
				}
			}

		}
	}

	void StartCount() {
		InvokeRepeating (kCountMethodName, 0.1f, timeIncrement);
	}

	void Count() {
		_value += valueIncrement;
		UpdateText ();

		if (_value == min || _value == max) {
			CancelInvoke(kCountMethodName);

			if (GameTimerEndedEvent != null) {
				GameTimerEndedEvent();
			}

			if (shouldClearOnCompletion)
				Invoke("ClearText", timeIncrement);
		}
	}

	void ClearText() {
		counterUIText.text = "";
	}
	
	private void BeginCount() {
		UpdateText ();
		StartCount ();
	}
}
