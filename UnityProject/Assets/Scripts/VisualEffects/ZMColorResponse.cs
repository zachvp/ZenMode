using UnityEngine;
using System.Collections;

public class ZMColorResponse : MonoBehaviour {
	private Color _baseColor;
	private Color _paintedColor;

	private enum State { ASLEEP, AWAKE, ALIVE };

	private State _state;

	// Use this for initialization
	void Start () {
		_baseColor = renderer.material.color;
	}
	
	// Update is called once per frame
	void Update () {
		//Ray ray = new Ray(); 
		if (_state == State.AWAKE) {
			renderer.material.color = _paintedColor;
		}

		if (_state == State.ALIVE) {
			renderer.material.color = Color.Lerp(renderer.material.color, _baseColor, 0.05f);
		}
	}

	public void Awaken(Color highlight) {
		_state = State.AWAKE;
		_paintedColor = highlight;

		float offset = 0.3f;
		_paintedColor.r += offset;
		_paintedColor.g += offset;
		_paintedColor.b += offset;

		Invoke("Enliven", 0.3f);
	}

	void Enliven() {
		_state = State.ALIVE;
	}
}
