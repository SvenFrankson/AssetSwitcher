using UnityEngine;
using UnityEditor;
using System.Collections;

namespace CreativityZero.Skinning {


	static class SkinningSceneTool {

		static int chosenDisplayedSkin = 0;
		static int chosenBuildSkin = 0;

		public static void SkinningSceneGUI () {

			GUILayout.BeginVertical ("box", GUILayout.ExpandWidth (true));

			// Enter/Leave Skin Mode
			if (!SkinnableObject.isInSkinMode) {
				if (GUILayout.Button ("Enter Skinning Mode")) {
					Selection.activeObject = null;
					SkinnableObject.isInSkinMode = true;
					SkinnableObject.StaticUpdate ();
				}
			}
			else if (SkinnableObject.isInSkinMode) {
				if (GUILayout.Button ("Leave Skinning Mode")) {
					Selection.activeObject = null;
					SkinnableObject.isInSkinMode = false;
					SkinnableObject.StaticUpdate ();
					SkinnableObject.UseSkinForAll ("default");
				}
			}

			if (SkinnableObject.isInSkinMode) {

				// Pick displayed Skin
				GUILayout.BeginHorizontal ();
				GUILayout.Label ("Displayed Skin");
				if (SkinnableObject.allSkins != null) {
					GUI.changed = false;
					SkinningSceneTool.chosenDisplayedSkin = SkinnableObject.allSkins.IndexOf (SkinnableObject.currentSkin);
					SkinningSceneTool.chosenDisplayedSkin = EditorGUILayout.Popup (SkinningSceneTool.chosenDisplayedSkin, SkinnableObject.allSkins.ToArray ());
					if (GUI.changed) {
						GUI.changed = false;
						SkinnableObject.UseSkinForAll ( SkinnableObject.allSkins [SkinningSceneTool.chosenDisplayedSkin]);
					}
				}
				GUILayout.EndHorizontal ();

				// Build skin
				GUILayout.BeginHorizontal ();
				GUILayout.Label ("Skin");
				if (SkinnableObject.allSkins != null) {
					SkinningSceneTool.chosenBuildSkin = EditorGUILayout.Popup (SkinningSceneTool.chosenBuildSkin, SkinnableObject.allSkins.ToArray ());
					if (GUILayout.Button ("Build")) {
						SkinnableObjectEditor.BuildSkinBundles ( SkinnableObject.allSkins [SkinningSceneTool.chosenBuildSkin]);
					}
				}
				GUILayout.EndHorizontal ();
			}

			GUILayout.EndVertical ();
		}
	}
}
