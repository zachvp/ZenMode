using UnityEngine;
using UnityEngine.UI;
using Core;

public class ZMTimedCounter : MonoBehaviour
{
	[SerializeField] private float startValue;
	[SerializeField] private float timeIncrement;
	[SerializeField] protected int min, max;
	[SerializeField] private bool isClockFormat;

	protected DisplayText counterUIText;
	protected float _value;
	protected Coroutine _timerCoroutine;

	protected virtual void Awake()
	{
		_value = startValue;

		counterUIText = new DisplayText(this);
	}

	public void StartTimer()
	{
		UpdateText();

		enabled = true;

		_timerCoroutine = Utilities.ExecuteAfterDelay(Count, Mathf.Abs(timeIncrement));
	}

	public void PauseTimer()
	{
		if (_timerCoroutine != null) { StopCoroutine(_timerCoroutine); }
		enabled = false;
	}

	public void Reset()
	{
		_value = startValue;
		UpdateText();
		PauseTimer();
	}

	protected virtual void UpdateText()
	{
		if (counterUIText == null) { return; }

		int minutes = Mathf.FloorToInt(_value / 60F);
		int seconds = Mathf.FloorToInt(_value - minutes * 60);

		counterUIText.Text =  isClockFormat ? string.Format("{0:0}:{1:00}", minutes, seconds) : _value.ToString();
	}

	protected virtual void Count()
	{
		_value += timeIncrement;
		UpdateText();
		_timerCoroutine = Utilities.ExecuteAfterDelay(Count, Mathf.Abs(timeIncrement));
	}

	private void ClearText()
	{
		counterUIText.Text = "";
	}
}
