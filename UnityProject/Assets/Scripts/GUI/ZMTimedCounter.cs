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
	public Text counterUIText;
	public string minMessage, maxMessage;

	private const string kCountMethodName = "Count";
	private int _value;

	public delegate void GameTimerEndedAction(); public static event GameTimerEndedAction GameTimerEndedEvent;
	
	void Awake () {
		_value = startValue;

		ZMGameStateController.StartGameEvent += HandleStartGameEvent;
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

	// public facing methods
	public void BeginCount() {
		UpdateText ();
		StartCount ();
	}
}
