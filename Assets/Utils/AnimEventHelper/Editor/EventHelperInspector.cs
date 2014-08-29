using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

namespace CreativityZero.Utils {

	[CustomEditor(typeof(EventHelper))]
	public class EventHelperInspector : Editor {

		private Texture imgPlus;
		private Texture imgMoins;
		private UnityEngine.Object lastAddedObject = null;

		void OnEnable () {
			this.imgPlus = AssetDatabase.LoadAssetAtPath ("Assets/Utils/Editor/plus.jpg", typeof(Texture)) as Texture;
			this.imgMoins = AssetDatabase.LoadAssetAtPath ("Assets/Utils/Editor/moins.jpg", typeof(Texture)) as Texture;

			if (this.imgPlus == null) {
				Debug.Log ("PLUS has failed");
			}
			if (this.imgMoins == null) {
				Debug.Log ("MOINS has failed");
			}
		}

		public override void OnInspectorGUI () {
			EventHelper myTarget = (EventHelper) target;
			UnityEngine.Object[] myTargetObjs = myTarget.objs;
			string[] myTargetArgs = myTarget.args;
			string[] myTargetMyTypes = myTarget.myTypes;

			EditorGUILayout.Space ();
			for (int i = 0; i < myTargetObjs.Length; i++) {
				EditorGUILayout.BeginHorizontal ();
				if (GUILayout.Button ("X", GUILayout.Width(30f))) {
					myTarget.RemoveEvent (i);
				}
				EditorGUILayout.ObjectField ("Event " + i, myTargetObjs[i], typeof(UnityEngine.Object), true);
				EditorGUILayout.EndHorizontal ();

				if (myTargetMyTypes[i] == "GameObjectWithAnimator") {
					EditorGUILayout.BeginHorizontal ();
					GUILayout.Label ("", GUILayout.Width(30f));
					myTargetArgs[i] = EditorGUILayout.TextField ("Trigger name", myTargetArgs[i]);
					EditorGUILayout.EndHorizontal ();
				}
				EditorGUILayout.Space ();
			}

			EditorGUILayout.BeginHorizontal ();
			if (GUILayout.Button ("+", GUILayout.Width(30f))) {
				if (lastAddedObject != null) {
					myTarget.AddEvent (lastAddedObject, "");
					lastAddedObject = null;
				}
			}
			lastAddedObject = EditorGUILayout.ObjectField ("New Event", lastAddedObject, typeof(UnityEngine.Object), true) as UnityEngine.Object;
			EditorGUILayout.EndHorizontal ();
		}

		private Editor gameObjectEditor;

		new void OnPreviewGUI(Rect r, GUIStyle background) {
			GameObject gameObject = this.target as GameObject;
			if (gameObject != null) {
				if (gameObjectEditor == null)
					gameObjectEditor = Editor.CreateEditor(gameObject);
				
				gameObjectEditor.OnPreviewGUI(r, background);
			}
		}
	}
}
