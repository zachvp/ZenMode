﻿using UnityEngine;

public class ZMAddForce : MonoBehaviour {
	public Vector2 force;
	public float torque;

	private ParticleSystem _familyParticleSystem;
	private float _sprayRate;

	private const float DISSAPATE_RATE = 20;
	private const float MIN_SPRAY_RATE = 4;

	private static float BaseEmissionRate = 0;

	void Start () {
		rigidbody2D.AddForce(force);
		rigidbody2D.AddTorque(torque);

		_familyParticleSystem = GetComponentInChildren<ParticleSystem>();

		if (BaseEmissionRate == 0) {
			BaseEmissionRate = _familyParticleSystem.emissionRate;;
		}

		_sprayRate = BaseEmissionRate;
	}

	void Update() {
		if (_sprayRate > MIN_SPRAY_RATE) {
			_sprayRate -= DISSAPATE_RATE * Time.deltaTime;
			_familyParticleSystem.emissionRate = _sprayRate;
		}
	}
}