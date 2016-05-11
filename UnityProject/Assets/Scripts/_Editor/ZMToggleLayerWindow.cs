using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using Core;

using System.Collections.Generic;

public class ZMToggleLayerWindow : EditorWindow
{
	UnityEngine.SceneManagement.Scene _previousScene;

	[MenuItem ("Window/ToggleLayers")]
	static void Init()
	{
		var window = EditorWindow.GetWindow<ZMToggleLayerWindow>() as ZMToggleLayerWindow;

		// TODO: Should happen on editor scene load...
		LayerManager.Init();
		ListenToEvents();

		window.Show();
	}

	private static void ListenToEvents()
	{
		LayerManager.OnLayerChangeActive += HandleLayerChangeActive;
	}

	[ExecuteInEditMode]
	private static void HandleLayerChangeActive(Layer layer)
	{
		LayerManager.SetLayerObjectsActive(layer, layer.isActive);
	}

	void OnGUI()
	{
		GUILayout.Label("Toggle Layers", EditorStyles.boldLabel);

		EditorGUILayout.LabelField("Which layers should be active?");

		for (int i = 0; i < LayerManager.LayerCount; ++i)
		{
			var layer = LayerManager.Layers[i];

			layer.isActive = GUILayout.Toggle(layer.isActive, layer.name);
		}
	}

	void OnSelectionChange()
	{
		var currentScene = UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene();

		if (_previousScene != currentScene)
		{
			LayerManager.Clear();
		}

		_previousScene = currentScene;
	}
}
