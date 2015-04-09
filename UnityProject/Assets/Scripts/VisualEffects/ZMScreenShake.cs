using UnityEngine;
using System.Collections;

public class ZMScreenShake : MonoBehaviour {

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
		transform.position = new Vector3(transform.position.x, Mathf.PingPong(Time.time, 16), transform.position.z);
	}
}
