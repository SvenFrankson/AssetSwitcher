using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections;
using System.Collections.Generic;

namespace CreativityZero.Skinning {

	[InitializeOnLoad]
	public class SkinnableObjectEditor : Editor {

		static SkinnableObjectEditor () {
			// SceneView.onSceneGUIDelegate += SkinnableObjectUI.SkinnableObjectGUIDisplay;
			EditorApplication.playmodeStateChanged -= CheckForPlay;
			EditorApplication.playmodeStateChanged += CheckForPlay;
		}

		static public void CheckForPlay () {
			if (EditorApplication.isPlayingOrWillChangePlaymode) {
				if (SkinnableObject.currentSkin != "default") {
					SkinnableObject.UseSkinForAll ("default");
				}
			}
		}

		static public void SkinnableObjectGUIDisplay (SceneView scene) {
			Handles.BeginGUI ();

			GUI.changed = false;
			SkinnableObject.isInSkinMode = EditorGUILayout.Toggle ("Skin Mode", SkinnableObject.isInSkinMode);
			if (GUI.changed) {
				GUI.changed = false;
				SkinnableObject.StaticUpdate ();
				if (!SkinnableObject.isInSkinMode) {
					foreach (SkinnableObject so in SkinnableObject.instances) {
						so.hideFlags = HideFlags.HideInInspector;
					}
					SkinnableObject.UseSkinForAll ("default");
				}
				else {
					foreach (SkinnableObject so in SkinnableObject.instances) {
						so.hideFlags = HideFlags.None;
					}
				}
			}

			if (SkinnableObject.instances != null) {
				EditorGUILayout.IntField ("Instances Count", SkinnableObject.instances.Count, GUILayout.Width (300f));

				float width = 250f;

				if (SkinnableObject.isInSkinMode) {
					if (SkinnableObject.currentSkin == "default") {
						width = 300f;
					}
					else {
						width = 250f;
					}
					if (GUILayout.Button ("Default", GUILayout.Width (width))) {
						SkinnableObject.UseSkinForAll ("default");
					}
					foreach (string skin in SkinnableObject.allSkins) {
						if (SkinnableObject.currentSkin == skin) {
							width = 300f;
						}
						else {
							width = 250f;
						}
						if (GUILayout.Button (skin, GUILayout.Width (width))) {
							SkinnableObject.UseSkinForAll (skin);
						}
						if (GUILayout.Button ("Build " + skin, GUILayout.Width (width))) {
							SkinnableObjectEditor.BuildSkinBundles (skin);
						}
						EditorGUILayout.Space ();
					}
				}
			}

			Handles.EndGUI ();
		}

		static public void BuildSkinBundles (string skin) {
			List<string> explicitNames = new List<string> ();
			List<GameObject> skinPrefabs = new List<GameObject> ();
			foreach (SkinnableObject so in SkinnableObject.instances) {
				GameObject skinPrefab = so.GetSkinPrefab (skin);
				if (skinPrefab != null) {
					explicitNames.Add (so.name);
					skinPrefabs.Add (skinPrefab);
				}
			}

			Directory.CreateDirectory (Path.Combine (Application.dataPath, "../Skins/"));
			string path = Path.Combine (Application.dataPath, "../Skins/skin.unity3D");
			if (path.Length != 0) {
				BuildPipeline.BuildAssetBundleExplicitAssetNames (skinPrefabs.ToArray (), explicitNames.ToArray (), path, 
				                               BuildAssetBundleOptions.CollectDependencies | BuildAssetBundleOptions.CompleteAssets);
			}
		}
	}
}