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
		renderer.material.color = new Color(renderer.material.color.r, renderer.material.color.g, renderer.material.color.b, 1);

		rigidbody2D.AddForce(force);
		rigidbody2D.AddTorque(torque);

		_familyParticleSystem = GetComponentInChildren<ParticleSystem>();

		if (BaseEmissionRate == 0) {
			BaseEmissionRate = _familyParticleSystem.emissionRate;;
		}

		_sprayRate = BaseEmissionRate;
		_familyParticleSystem.renderer.material.color = _particleColor;

		Invoke ("Despawn", LIFETIME);
	}

	void Update() {
		if (_sprayRate > MIN_SPRAY_RATE) {
			_sprayRate -= DISSAPATE_RATE * Time.deltaTime;
			_familyParticleSystem.emissionRate = _sprayRate;

			_familyParticleSystem.renderer.material.color = _particleColor;
			_familyParticleSystem.startColor = _particleColor;
		}

		if (_despawning) {
			renderer.material.color = Color.Lerp(renderer.material.color, Color.clear, FADE_SPEED * Time.deltaTime);

			if (renderer.material.color.a < 0.05f) {
				Destroy(gameObject);
			}
		}
	}

	void Despawn() {
		_despawning = true;
	}

	public void AddForce(Vector2 force) {
		rigidbody2D.AddForce(force);
	}
}
