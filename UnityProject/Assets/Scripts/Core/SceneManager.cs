using UnityEngine;
using System;

namespace Core
{
	public class SceneWrapper : IComparable
	{
		public int Index { get; private set; }
		public string Name { get; private set; }
		public UnityEngine.SceneManagement.Scene Scene { get; set; }

		public bool IsNull { get { return Index == -1; }  }

		public static SceneWrapper NullScene { get { return _nullScene; } }

		private static SceneWrapper _nullScene = new SceneWrapper();

		public SceneWrapper()
		{
			Index = -1;
			Name = "NullScene";
		}

		public SceneWrapper(UnityEngine.SceneManagement.Scene inScene, int index)
		{
			Init(inScene, index);
		}
			
		private void Init(UnityEngine.SceneManagement.Scene inScene, int index)
		{
			Scene = inScene;
			Index = index;
			Name = inScene.name;
		}

		public override int GetHashCode()
		{
			return Index.GetHashCode();
		}

		public override bool Equals(System.Object other)
		{
			var otherWrapper = (SceneWrapper) other;
			bool result = false;

			if (otherWrapper != null)
			{
				result = Index == otherWrapper.Index;
			}

			return result;
		}

		public static bool operator ==(SceneWrapper lhs, SceneWrapper rhs)
		{
			// Automatically equal if same reference (or both null).
			if (System.Object.ReferenceEquals(lhs, rhs)) { return true; }

			// If one is null, but not both, return false.
			if ((object) lhs == null || (object) rhs == null) { return false; }

			return lhs.Index == rhs.Index;
		}

		public static bool operator !=(SceneWrapper lhs, SceneWrapper rhs)
		{
			return !(lhs == rhs);
		}

		int IComparable.CompareTo(object other)
		{
			var otherWrapper = (SceneWrapper) other;
			int result = 0;

			if (otherWrapper == null) { result = 1; }
			else if (Index < otherWrapper.Index) { result = -1; }
			else if (Index > otherWrapper.Index) { result = 1; }

			return result;
		}
	}

	public static class SceneManager
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
