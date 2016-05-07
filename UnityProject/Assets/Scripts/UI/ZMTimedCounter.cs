using UnityEngine;
using UnityEngine.UI;
using Core;

public class ZMTimedCounter : MonoBehaviour
{
	[SerializeField] private float startValue;
	[SerializeField] private float timeIncrement;
	[SerializeField] protected int min, max;

	protected Text counterUIText;
	protected float _value;
	protected Coroutine _timerCoroutine;

	protected virtual void Awake()
	{
		_value = startValue;

		counterUIText = GetComponent<Text>();
	}

	protected virtual void UpdateText()
	{
		int minutes = Mathf.FloorToInt(_value / 60F);
		int seconds = Mathf.FloorToInt(_value - minutes * 60);

		counterUIText.text =  string.Format("{0:0}:{1:00}", minutes, seconds); _value.ToString();
	}

	protected void StartTimer()
	{
		UpdateText();

		enabled = true;

		_timerCoroutine = Utilities.ExecuteAfterDelay(Count, Mathf.Abs(timeIncrement));
	}

	protected void PauseTimer()
	{
		StopCoroutine(_timerCoroutine);
		enabled = false;
	}

	protected virtual void Count()
	{
		_value += timeIncrement;
		UpdateText();
		_timerCoroutine = Utilities.ExecuteAfterDelay(Count, Mathf.Abs(timeIncrement));
	}

	private void ClearText()
	{
		counterUIText.text = "";
	}
}
