using UnityEngine;
using Core;

public class ZMGoBackController : MonoBehaviour
{
	public AudioClip _audioBack;
	public AudioClip[] _audioStart;

	void Awake()
	{
		ZMGameInputManager.AnyInputEvent += HandleGoBack;
	}

	void Start()
	{
		audio.PlayOneShot(_audioStart[Random.Range(0, _audioStart.Length)]);
	}

	private void HandleGoBack(int ID)
	{
		audio.PlayOneShot(_audioBack);
		Application.LoadLevel(ZMSceneIndexList.INDEX_MAIN_MENU);
	}
}
