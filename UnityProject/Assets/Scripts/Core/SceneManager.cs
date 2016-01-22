using UnityEngine;

namespace Core
{
	public class SceneManager : MonoBehaviour
	{
		public static EventHandler OnLoadScene;

		public static void LoadScene(string name)
		{
			Notifier.SendEventNotification(OnLoadScene);

			Application.LoadLevel(name);
		}

		public static void LoadScene(int sceneIndex)
		{
			Notifier.SendEventNotification(OnLoadScene);
			
			Application.LoadLevel(sceneIndex);
		}

		public static void ResetScene()
		{
			LoadScene(Application.loadedLevel);
		}

		public static void QuitGame()
		{
			Application.Quit();
		}
	}
}
