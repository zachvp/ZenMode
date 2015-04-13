﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ZMFadeIn : MonoBehaviour {
	public Image fadedImage;
	public float interval = 0.2f;

	// Use this for initialization
	void Start () {
		Color newcolor = fadedImage.color;
		newcolor.a = 0;

		fadedImage.color = newcolor;
	}
	
	// Update is called once per frame
	void Update () {
		Color newcolor = fadedImage.color;
		newcolor.a += interval;
		fadedImage.color += newcolor * Time.deltaTime;

		if (fadedImage.color.a >= 255) {
			Application.LoadLevel(1);
		}
	}
}
