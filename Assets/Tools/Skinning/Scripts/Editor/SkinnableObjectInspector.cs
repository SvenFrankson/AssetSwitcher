using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections;
using System.Collections.Generic;

namespace CreativityZero.Skinning {

	[CustomEditor (typeof (SkinnableObject))]
	public class SkinnableObjectInspector : Editor {

		private string newSkinName = "";
		private SkinnableObject Target {
			get {
				return (SkinnableObject) target;
			}
		}

		public void OnEnable () {

			SkinnableObject.StaticUpdate ();
			foreach (string s in SkinnableObject.allSkins) {
				this.AddSkin (s);
			}

			if (Target.basePrefab == null) {
				Target.basePrefab = PrefabUtility.GetPrefabParent (Target.gameObject) as GameObject;
				if (Target.basePrefab == null) {
					Debug.LogWarning ("Onyx Skinning : Do not add SkinnableObject on a GameObject that is not an instance of a prefab. Remove this component, save associated GameObject as a prefab, and retry.");
				}
			}
		}

		public override void OnInspectorGUI () {

			if (SkinnableObject.isInSkinMode) {
				GUILayout.BeginVertical ("box");
				GUI.changed = false;
				for (int i = 0; i < Target.skins.Count; i++) {
					if (Target.skins [i] != "default") {
						Target.skinPrefabs[i] = EditorGUILayout.ObjectField (Target.skins[i], Target.skinPrefabs[i], typeof (GameObject), true) as GameObject;
						if (GUI.changed) {
							GUI.changed = false;
							if (Target.skinPrefabs [i] != null) {
								GameObject diffGameObject = GameObject.Instantiate (Target.skinPrefabs [i]) as GameObject;
								diffGameObject.transform.position = Target.transform.position + new Vector3 (0, 0, 2f);
								diffGameObject.transform.rotation = Target.transform.rotation;
								diffGameObject.transform.localScale = Target.transform.localScale;
								diffGameObject.name = Target.skinPrefabs [i].name + "_diff";
								GameObjectOperator.Substract (Target.gameObject, diffGameObject);

								Directory.CreateDirectory (Path.Combine (Application.dataPath, "Skinning/Prefab/"));
								Target.diffPrefabs [i] = PrefabUtility.CreatePrefab ("Assets/Skinning/Prefab/" + diffGameObject.name + ".prefab", diffGameObject);
								DestroyImmediate (diffGameObject);
							}
						}
						EditorGUILayout.Space ();
					}
				}
				GUILayout.EndVertical ();

				GUILayout.BeginHorizontal ();
				GUILayout.Label ("New Skin");
				this.newSkinName = GUILayout.TextField (this.newSkinName);
				if (GUILayout.Button ("Create", GUILayout.ExpandWidth (false))) {
					this.AddSkin (this.newSkinName);
				}
				GUILayout.EndHorizontal ();
			}
		}

		public void AddSkin (string skinName) {
			if (skinName != "") {
				if (!Target.skins.Contains (skinName)) {
					Target.skins.Add (skinName);
					Target.skinPrefabs.Add (null);
					Target.diffPrefabs.Add (null);
					this.newSkinName = "";
				}
			}
		}
	}
}