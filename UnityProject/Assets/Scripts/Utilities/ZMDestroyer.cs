﻿using UnityEngine;
using System.Collections;

public class ZMDestroyer : MonoBehaviour {

	public float seconds;

	// Use this for initialization
	void Start () {
		Destroy (gameObject, seconds);
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
