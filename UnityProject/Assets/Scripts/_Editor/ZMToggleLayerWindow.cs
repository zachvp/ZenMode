using UnityEditor;
using UnityEngine;
using Core;

using System.Collections.Generic;

// Additional features:
// 		Add object to layer when added to scene.
//		Remove object from layer when removed from scene.

// TODO: Move change scene logic to the SceneManagerEditor. Easiest way i can think of rn is extend the main unity
// 		 editor window and listen to OnHierarchy change, checking for a scene difference.
public class ZMToggleLayerWindow : EditorWindow
{
	private static SceneWrapper _previousScene;

	[MenuItem ("Window/Toggle Layers")]
	static void InitWindow()
	{
		var window = EditorWindow.GetWindow<ZMToggleLayerWindow>() as ZMToggleLayerWindow;

		window.Show();
	}

	private static void DiffInit()
	{
		// Only call Init() if the scene has changed.
		if (_previousScene != SceneManagerEditor.CurrentScene)
		{
//			Init();
			// Add the new scene.
			LayerManager.AddActiveScene();

			ZMToggleLayerWindow._previousScene = SceneManagerEditor.CurrentScene;
			Debug.Log("update previous scene");
		}
	}

	private static void Init()
	{
		Debug.Log("Init");

		// TODO: Should happen on editor scene load...
		LayerManager.Init();

		ZMToggleLayerWindow.ListenToEvents();

		// Init the previous scene to a nonexistent one.
		ZMToggleLayerWindow._previousScene = null;
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

		if (GUILayout.Button("Force refresh"))
		{
			Init();
		}
	}

	void OnFocus()
	{
		Debug.LogFormat("OnFocus");
		ZMToggleLayerWindow.DiffInit();
	}

	static void OnHierarchyChange()
	{
		Debug.Log("OnHierarchyChange");
		ZMToggleLayerWindow.DiffInit();
	}

	private static void ListenToEvents()
	{
		LayerManager.OnLayerChangeActive += HandleLayerChangeActive;
	}

	[ExecuteInEditMode]
	private static void HandleLayerChangeActive(LayerEventArgs args)
	{
		LayerManager.SetLayerObjectsActive(args.layer, args.layer.isActive);
	}
}
