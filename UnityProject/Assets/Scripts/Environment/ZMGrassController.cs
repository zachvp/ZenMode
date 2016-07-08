using UnityEngine;
using System.Collections;

public class ZMGrassController : MonoBehaviour {

	public Sprite[] grassSpritesUp;
	public Sprite[] grassSpritesDown;
	public Vector3 origin;
	public ParticleSystem _cutEmitter;


	public void GrassEnter () {
		StartCoroutine (TranslateGrass (Vector3.zero, new Vector3(0.0f, -10.0f, 0.0f), 0.1f));
	}

	public void GrassExit () {
		StartCoroutine (TranslateGrass (new Vector3(0.0f, -10.0f, 0.0f), Vector3.zero, 0.1f));
	}

	public void CutGrass (ZMPlayerInfo playerInfo) {
		_cutEmitter.Play ();
		GetComponent<SpriteRenderer> ().enabled = false;
		Destroy (gameObject, 1.0f);

		// add to the stat tracker
		ZMStatTracker.GrassCuts.Add(playerInfo);
	}

	void Awake () {
		GetComponent<SpriteRenderer> ().sprite = grassSpritesUp [Random.Range (0, grassSpritesUp.Length)];
		origin = transform.position;
	}

	private IEnumerator TranslateGrass(Vector3 start, Vector3 end, float totalTime) {
		start += origin;
		end += origin;

		float t = 0;
		while (t < totalTime) {
			transform.localPosition = Vector3.Lerp(start, end, t / totalTime);
			yield return null;
			t += Time.deltaTime;
		} 
		transform.localPosition = end;
		yield break;
	}


}
