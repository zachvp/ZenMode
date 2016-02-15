using UnityEngine;
using Core;

public class ZMLightPulse : MonoBehaviour {
	public float interval;
	
	private float _baseIntensity;

	private float _theta;
	private float _differenceHalf;

	private bool _pulsing; public bool Pulsing { get { return _pulsing; } set { _pulsing = value; } }

	void Awake()
	{
		MatchStateManager.OnMatchEnd += HandleGameEndEvent;
	}

	void HandleGameEndEvent ()
	{
		enabled = false;
	}


	void Start () {
		_baseIntensity = GetComponent<Light>().intensity;
	}
	
	// Update is called once per frame
	void Update () {
		GetComponent<Light>().intensity = 6.0f + 3.0f * Mathf.Sin( _theta);

		_theta += interval;
		_theta %= 2 * Mathf.PI;
	}

	void SetPulsingOn() {
		_pulsing = true;
	}

	void SetPulsingOff() {
		_pulsing = false;
		GetComponent<Light>().intensity = _baseIntensity;
	}
}
