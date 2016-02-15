using UnityEngine;
using Core;

public class ZMNextScene : MonoBehaviour
{
	void Start ()
	{
		Cursor.visible = false;
		SceneManager.LoadNextScene();
	}
}
