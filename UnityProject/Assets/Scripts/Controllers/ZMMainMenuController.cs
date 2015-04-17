using UnityEngine;
using System.Collections;

public class ZMMainMenuController : MonoBehaviour {

	// Use this for initialization
	void Awake () {
		ZMGameInputManager.StartInputEvent += HandleStartInputEvent;
	}

	void HandleStartInputEvent (ZMPlayer.ZMPlayerInfo.PlayerTag playerTag)
	{
		Application.LoadLevel(2);
	}
}
