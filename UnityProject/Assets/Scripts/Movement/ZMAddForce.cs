using UnityEngine;

public class ZMAddForce : MonoBehaviour {
	public Vector2 force;
	public float torque;
	private Color _particleColor = Color.red; public Color ParticleColor { get { return _particleColor; } set { _particleColor = value; } }

	private ParticleSystem _familyParticleSystem;
	private float _sprayRate;

	private const float DISSAPATE_RATE = 20;
	private const float MIN_SPRAY_RATE = 2;
	private const float LIFETIME = 3f;

	private static float BaseEmissionRate = 0;
	private static int InstanceCount = 0;

	void Start () {
		InstanceCount += 1;

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
	}

	void Despawn() {
		if (InstanceCount > 0) {
			InstanceCount -= 1;
			Destroy(gameObject);
		}
	}

	public void AddForce(Vector2 force) {
		rigidbody2D.AddForce(force);
	}
}
