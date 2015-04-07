using UnityEngine;
using System.Collections;

namespace ZMPlayer {
	public class ZMPlayerClient : MonoBehaviour {
		public void ClientReady(ZMScoreController scoreController) {
			Debug.Log (gameObject.name + ": client ready!");

		}
	}
}