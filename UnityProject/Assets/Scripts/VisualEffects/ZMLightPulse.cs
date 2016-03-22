using UnityEngine;
using Core;

public class ZMLightPulse : MonoBehaviour
{
	public bool Pulsing { get { return _pulsing; } set { _pulsing = value; } }

	public float interval;

	private float _baseIntensity;
	private float _theta;
	private float _differenceHalf;

	private bool _pulsing;

	private Light _light;

	void Awake()
	{
		_light = GetComponent<Light>();

		MatchStateManager.OnMatchEnd += HandleGameEndEvent;
	}

	void Start ()
	{
		_baseIntensity = _light.intensity;
	}
	
	void Update ()
	{
		_light.intensity = 6.0f + 3.0f * Mathf.Sin( _theta);

		_theta += interval;
		_theta %= 2 * Mathf.PI;
	}

	void HandleGameEndEvent()
	{
		enabled = false;
	}

	void SetPulsingOn()
	{
		_pulsing = true;
	}

	void SetPulsingOff()
	{
		_pulsing = false;
		_light.intensity = _baseIntensity;
	}
}
