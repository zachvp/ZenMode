using UnityEngine;

namespace Core
{
	public class SceneManager
	{
		public static int CurrentSceneIndex { get { return _currentSceneIndex; } }
		public static string CurrentSceneName { get { return _currentSceneName; } } 

		private static int _currentSceneIndex;
		private static string _currentSceneName;

		public static EventHandler OnLoadScene;

		public static void LoadScene(string name)
		{
			_currentSceneName = name;
			Notifier.SendEventNotification(OnLoadScene);

			UnityEngine.SceneManagement.SceneManager.LoadScene(name);
		}

		public static void LoadScene(int sceneIndex)
		{
			_currentSceneIndex = sceneIndex;
			Notifier.SendEventNotification(OnLoadScene);

			UnityEngine.SceneManagement.SceneManager.LoadScene(sceneIndex);
		}

		public static void LoadNextScene()
		{
			LoadScene(_currentSceneIndex + 1);
		}

		public static void ResetScene()
		{
			LoadScene(_currentSceneIndex);
		}

		public static void QuitGame()
		{
			Application.Quit();
		}
	}
}
