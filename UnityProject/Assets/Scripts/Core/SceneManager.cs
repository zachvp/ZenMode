using UnityEngine;

namespace Core
{
	public class SceneManager
	{
		public static int CurrentSceneIndex { get { return _currentSceneIndex; } }
		public static string CurrentSceneName { get { return _currentSceneName; } } 

		private static int _currentSceneIndex { get { return GetCurrentSceneIndex(); } }
		private static string _currentSceneName { get { return GetCurrentSceneName(); } }

		public static EventHandler OnLoadScene;

		public static void LoadScene(string name)
		{
			LoadSceneInternal();
			UnityEngine.SceneManagement.SceneManager.LoadScene(name);
		}

		public static void LoadScene(int sceneIndex)
		{
			LoadSceneInternal();
			UnityEngine.SceneManagement.SceneManager.LoadScene(sceneIndex);
		}

		private static void LoadSceneInternal()
		{
			Notifier.SendEventNotification(OnLoadScene);
			MatchStateManager.Clear();	
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

		private static int GetCurrentSceneIndex()
		{
			var scene = UnityEngine.SceneManagement.SceneManager.GetActiveScene();

			return scene.buildIndex;
		}

		private static string GetCurrentSceneName()
		{
			var scene = UnityEngine.SceneManagement.SceneManager.GetActiveScene();

			return scene.name;
		}
	}
}
