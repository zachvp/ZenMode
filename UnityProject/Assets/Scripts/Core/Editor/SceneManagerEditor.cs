using UnityEngine;

namespace Core
{
	// TODO: Initialize all scenes in some data structure.
	public static class SceneManagerEditor
	{
		private static SceneWrapper[] _scenes;

		public static int SceneCount { get { return UnityEditor.SceneManagement.EditorSceneManager.sceneCount; } }

		public static SceneWrapper CurrentScene
		{
			get
			{
				var activeUnityScene = UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene();

				return FindWrapperForUnityScene(activeUnityScene);
			}
		}

		public static void Init()
		{
			_scenes = new SceneWrapper[SceneCount];

			for (int i = 0; i < _scenes.Length; ++i)
			{
				var unityScene = UnityEditor.SceneManagement.EditorSceneManager.GetSceneAt(i);

				_scenes[i] = new SceneWrapper(unityScene, i);
			}
		}

		public static SceneWrapper GetSceneAtIndex(int index)
		{
			SceneWrapper result;

			// Initialize our internal list of SceneWrappers if we haven't yet.
			if (SceneManagerEditor.IsEmpty())
			{
				SceneManagerEditor.Init();
			}

			if (index < 0 || index >= _scenes.Length)
			{
				Debug.LogWarningFormat("SceneManagerEditor: Unable to retrieve scene for index {0}. Returning NullScene", index);
				result = SceneWrapper.NullScene;
			}
			else
			{
				result = _scenes[index];
			}

			return result;
		}

		// Necessary because a Unity scene's build index isn't the index according to GetSceneAt(int);
		private static SceneWrapper FindWrapperForUnityScene(UnityEngine.SceneManagement.Scene scene)
		{
			SceneWrapper result = SceneWrapper.NullScene;

			// Initialize our internal list of SceneWrappers if we haven't yet.
			if (SceneManagerEditor.IsEmpty())
			{
				SceneManagerEditor.Init();
			}

			// We need to go through the unity scenes to find the given one.
			for (int i = 0; i < SceneManagerEditor.SceneCount; ++i)
			{
				var unityScene = UnityEditor.SceneManagement.EditorSceneManager.GetSceneAt(i);

				if (unityScene == scene)
				{
					var wrapper = _scenes[i];
					var message = string.Format("Inconsistent path for unity scene {0} and SceneWrapper {1}",
						unityScene.path, wrapper.Scene.path);

					Debug.AssertFormat(unityScene.path == wrapper.Scene.path, message);

					result = wrapper;
					break;
				}
			}

			return result;
		}

		public static bool IsEmpty()
		{
			return _scenes == null || _scenes.Length == 0;
		}
	}
}