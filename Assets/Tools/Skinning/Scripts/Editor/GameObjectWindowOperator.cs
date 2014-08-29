using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

namespace CreativityZero.Skinning {
	
	public class GameObjectWindowOperator : EditorWindow {

		public GameObject baseGameObject;
		public GameObject skinnedGameObject;
		public GameObject additionalGameObject;
		public AssetBundle skinBundle;

		[MenuItem ("Window/GameObjectOperator")]
		static void Open () {
			EditorWindow.GetWindow<GameObjectWindowOperator> ();
		}

		public void OnGUI () {
			GUI.skin = Resources.Load<GUISkin> ("HappySkin");

			this.baseGameObject = EditorGUILayout.ObjectField ("Base", this.baseGameObject, typeof(GameObject), true) as GameObject;
			this.skinnedGameObject = EditorGUILayout.ObjectField ("Skinned", this.skinnedGameObject, typeof(GameObject), true) as GameObject;

			if (GUILayout.Button ("Substract")) {
				GameObject diffGameObject = GameObject.Instantiate (skinnedGameObject, Vector3.zero, Quaternion.identity) as GameObject;
				diffGameObject.name = "DiffSkinBase";
				GameObjectOperator.Substract (this.baseGameObject, diffGameObject);
			}

			this.baseGameObject = EditorGUILayout.ObjectField ("Base", this.baseGameObject, typeof(GameObject), true) as GameObject;
			this.additionalGameObject = EditorGUILayout.ObjectField ("Skin", this.additionalGameObject, typeof(GameObject), true) as GameObject;
			
			if (GUILayout.Button ("Add")) {
				GameObject addGameObject = GameObject.Instantiate (this.baseGameObject, Vector3.zero + Vector3.forward * 2f, Quaternion.identity) as GameObject;
				addGameObject.name = "AddSkinBase";
				GameObjectOperator.Add (addGameObject, this.additionalGameObject);
			}

			if (GUILayout.Button ("Load Base Asset Bundle")) {
				this.LoadBaseAssetBundle ();
			}

			EditorGUILayout.ObjectField ("Skin Asset Bundle", this.skinBundle, typeof(AssetBundle), false);
			if (GUILayout.Button ("Load Skin Asset Bundle")) {
				this.LoadSkinAssetBundle ();
			}

			if (GUILayout.Button ("Merge All With Skin")) {
				MergeAllByAdding ();
			}
			
			if (GUILayout.Button ("Log Skin Asset Bundle")) {
				LogSkinAssetBundle ();
			}

			if (GUILayout.Button ("Log Skin Asset Bundle Material")) {
				LogSkinAssetBundleMaterialGUID ();
			}

			this.skinnedGameObject = EditorGUILayout.ObjectField ("Skinned", this.skinnedGameObject, typeof(GameObject), true) as GameObject;
			this.baseGameObject = EditorGUILayout.ObjectField ("Base", this.baseGameObject, typeof(GameObject), true) as GameObject;
			this.additionalGameObject = EditorGUILayout.ObjectField ("Skin", this.additionalGameObject, typeof(GameObject), true) as GameObject;

			if (GUILayout.Button ("Cancel Add")) {
				GameObjectOperator.CancelAdd (this.skinnedGameObject, this.baseGameObject, this.additionalGameObject);
			}
		}

		/// <summary>
		/// Finds all root game objects.
		/// </summary>
		/// <returns>The all root game objects.</returns>
		public List<GameObject> FindAllRootGameObjects () {
			List<GameObject> rootGameObjects = new List<GameObject> ();
			GameObject[] allGameObjects = GameObject.FindObjectsOfType<GameObject> ();
			foreach (GameObject go in allGameObjects) {
				if (go.transform.parent == null) {
					rootGameObjects.Add (go);
				}
			}

			return rootGameObjects;
		}

		/// <summary>
		/// Finds all skin game objects.
		/// </summary>
		/// <returns>The all skin game objects.</returns>
		public List<GameObject> FindAllSkinGameObjects () {
			Object[] skinObjects = this.skinBundle.LoadAll (typeof(GameObject));
			List<GameObject> skinGameObjects = new List<GameObject> ();
			foreach (Object o in skinObjects) {
				skinGameObjects.Add (o as GameObject);
			}

			return skinGameObjects;
		}

		/// <summary>
		/// Merges all by adding.
		/// </summary>
		public void MergeAllByAdding () {
			List<GameObject> baseGameObjects = FindAllRootGameObjects ();
			List<GameObject> skinGameObjects = FindAllSkinGameObjects ();

			foreach (GameObject goBase in baseGameObjects) {
				foreach (GameObject goSkin in skinGameObjects) {
					MergeByAdding (goBase, goSkin);
				}
			}
		}

		/// <summary>
		/// Merges the by adding.
		/// </summary>
		/// <param name="baseGO">Base G.</param>
		/// <param name="addGO">Add G.</param>
		public void MergeByAdding (GameObject baseGO, GameObject addGO) {
			if (baseGO.name != addGO.name) {
				return;
			}

			GameObjectOperator.Add (baseGO, addGO);
		}

		public void LoadBaseAssetBundle () {
			string baseAssetBundlePath = EditorUtility.OpenFilePanel ("Open Resource", "", "unity3d");
			AssetBundle baseAssetBundle = AssetBundle.CreateFromFile (baseAssetBundlePath);
			Object[] gos = baseAssetBundle.LoadAll (typeof(GameObject));
			this.baseGameObject = Instantiate ((GameObject)gos [0]) as GameObject;
		}
		
		public void LoadSkinAssetBundle () {
			string skinAssetBundlePath = EditorUtility.OpenFilePanel ("Open Resource", "", "unity3d");
			Debug.Log (skinAssetBundlePath);
			if (skinAssetBundlePath != "") {
				this.skinBundle = AssetBundle.CreateFromFile (skinAssetBundlePath);
			}
		}

		public void LogSkinAssetBundle () {

			GameObject g = this.skinBundle.Load ("9694224_skin") as GameObject;
			Debug.Log ("Name = " + g.name);
		}

		public void LogSkinAssetBundleMaterialGUID () {
			Object[] gos = skinBundle.LoadAll (typeof(Material));
			Material mat = gos[0] as Material;
			Debug.Log ("Name = " + mat.name + ", InstanceID = " + mat.GetInstanceID ());
		}
	}
}