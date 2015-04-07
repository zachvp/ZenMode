using UnityEditor;
using UnityEngine;
using System.Collections;

public class PackageIcons
{
	[MenuItem("Assets/PackageIcons")]
	static void DoPackageIcons()
	{
		// Build the resource file from the active selection.
		string [] assetNames = new string[Selection.objects.Length];
		for (int i = 0; i < Selection.objects.Length; ++i)
		{
			assetNames[i] = System.IO.Path.GetFileName(Selection.objects[i].name);
		}
		BuildPipeline.BuildAssetBundleExplicitAssetNames(Selection.objects, assetNames, "Assets/P4Connect/Editor/Icons.pack", BuildAssetBundleOptions.UncompressedAssetBundle);
	}
}
