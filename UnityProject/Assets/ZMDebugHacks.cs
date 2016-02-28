using UnityEngine;
using Core;

public class ZMDebugHacks : MonoBehaviour
{
	void Update()
	{
		if (Input.GetKeyDown(KeyCode.T))
		{
			MatchStateManager.EndMatch();
		}
	}
}
