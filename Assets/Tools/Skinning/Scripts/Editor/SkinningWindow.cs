using UnityEngine;
using UnityEditor;
using System.Collections;

namespace CreativityZero.Skinning {

	public class SkinningWindow : EditorWindow {

		[MenuItem ("CreativityZero/Skinning")]
		static void Open () {
			EditorWindow.GetWindow<SkinningWindow> ();
		}

		static int chosenDisplayedSkin = 0;
		static int chosenBuildSkin = 0;

		public void OnGUI () {
			EditorGUILayout.LabelField ("CreativityZero. Skinning", EditorStyles.whiteLargeLabel);
			EditorGUILayout.Space ();

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
					SkinningWindow.chosenDisplayedSkin = SkinnableObject.allSkins.IndexOf (SkinnableObject.currentSkin);
					SkinningWindow.chosenDisplayedSkin = EditorGUILayout.Popup (SkinningWindow.chosenDisplayedSkin, SkinnableObject.allSkins.ToArray ());
					if (GUI.changed) {
						GUI.changed = false;
						SkinnableObject.UseSkinForAll ( SkinnableObject.allSkins [SkinningWindow.chosenDisplayedSkin]);
					}
				}
				GUILayout.EndHorizontal ();
				
				// Build skin
				GUILayout.BeginHorizontal ();
				GUILayout.Label ("Skin");
				if (SkinnableObject.allSkins != null) {
					SkinningWindow.chosenBuildSkin = EditorGUILayout.Popup (SkinningWindow.chosenBuildSkin, SkinnableObject.allSkins.ToArray ());
					if (GUILayout.Button ("Build")) {
						SkinnableObjectEditor.BuildSkinBundles ( SkinnableObject.allSkins [SkinningWindow.chosenBuildSkin]);
					}
				}
				GUILayout.EndHorizontal ();
			}
		}
	}
}