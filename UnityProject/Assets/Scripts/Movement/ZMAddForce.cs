using UnityEngine;

public class ZMAddForce : MonoBehaviour {
	public Vector2 force;
	public float torque;
	private Color _particleColor = Color.red; public Color ParticleColor { get { return _particleColor; } set { _particleColor = value; } }

	private ParticleSystem _familyParticleSystem;
	private float _sprayRate;
	private bool _despawning;

	private const float DISSAPATE_RATE = 20;
	private const float MIN_SPRAY_RATE = 0;
	private const float LIFETIME = 15f;
	private const float FADE_SPEED = 0.6f;

	private static float BaseEmissionRate = 0;

	void Start () {
		GetComponent<Renderer>().material.color = new Color(GetComponent<Renderer>().material.color.r, GetComponent<Renderer>().material.color.g, GetComponent<Renderer>().material.color.b, 1);

		GetComponent<Rigidbody2D>().AddForce(force);
		GetComponent<Rigidbody2D>().AddTorque(torque);

		_familyParticleSystem = GetComponentInChildren<ParticleSystem>();

		if (BaseEmissionRate == 0) {
			BaseEmissionRate = _familyParticleSystem.emission.rate.curveScalar;
		}

		_sprayRate = BaseEmissionRate;
		_familyParticleSystem.GetComponent<Renderer>().material.color = _particleColor;

		Invoke ("Despawn", LIFETIME);
	}

	void Update()
	{
		if (_sprayRate > MIN_SPRAY_RATE)
		{
			var rate = _familyParticleSystem.emission.rate;

			_sprayRate -= DISSAPATE_RATE * Time.deltaTime;
			rate.curveScalar = _sprayRate;

			_familyParticleSystem.GetComponent<Renderer>().material.color = _particleColor;
			_familyParticleSystem.startColor = _particleColor;
		}

		if (_despawning)
		{
			GetComponent<Renderer>().material.color = Color.Lerp(GetComponent<Renderer>().material.color, Color.clear, FADE_SPEED * Time.deltaTime);

			if (GetComponent<Renderer>().material.color.a < 0.05f) {
				Destroy(gameObject);
			}
		}
	}

	void Despawn() {
		_despawning = true;
	}

	public void AddForce(Vector2 force) {
		GetComponent<Rigidbody2D>().AddForce(force);
	}
}
