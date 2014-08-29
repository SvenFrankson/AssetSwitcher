using UnityEngine;

using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;

namespace CreativityZero.Skinning {
	
	public class SkinnableObject : MonoBehaviour {

		static private bool loadFromURL = true;

		static public bool isInSkinMode = false;
		static public List<AssetBundle> assetBundle = null;
		
		static public List<SkinnableObject> instances;
		static public List<string> allSkins = new List<string> ();
		static public string currentSkin = "default";
		
		public List<string> skins = new List<string> ();
		public GameObject basePrefab = null;
		public List<GameObject> skinPrefabs = new List<GameObject> ();
		public List<GameObject> diffPrefabs = new List<GameObject> ();

		public bool skinningDone { private set; get; }
		
		static public void StaticUpdate () {
			SkinnableObject.instances = new List<SkinnableObject> (GameObject.FindObjectsOfType<SkinnableObject> ());
			
			if (!SkinnableObject.allSkins.Contains ("default")) {
				SkinnableObject.allSkins.Add ("default");
			}

			foreach (SkinnableObject so in SkinnableObject.instances) {
				foreach (string s in so.skins) {
					if (!SkinnableObject.allSkins.Contains (s)) {
						SkinnableObject.allSkins.Add (s);
					}
				}
			}
			
			SkinnableObject.CheckForSameNames ();
		}
		
		static public void CheckForSameNames () {
			for (int i = 0; i < SkinnableObject.instances.Count; i++) {
				for (int j = i + 1; j < SkinnableObject.instances.Count; j++) {
					if (SkinnableObject.instances [i].name == SkinnableObject.instances [j].name) {
						Debug.LogWarning ("OnyxSkinning : Two Skinnable GameObjects should not have the same name, it may cause Skinning to fail at runtime (please note it will perfectly work in edit mode, but fail at runtime)");
					}
				}
			}
		}
		
		static public void UseSkinForAll (string skin) {
			if (SkinnableObject.currentSkin != "default") {
				foreach (SkinnableObject so in SkinnableObject.instances) {
					try {
						so.DropSkin (SkinnableObject.currentSkin);
					} catch (Exception e) { Debug.Log (e); }
				}
			}
			
			SkinnableObject.currentSkin = skin;
			
			if (SkinnableObject.currentSkin != "default") {
				foreach (SkinnableObject so in SkinnableObject.instances) {
					try {
						so.UseSkin (skin);
					} catch (Exception e) { Debug.Log (e); }
				}
			}
		}

		public SkinnableObject () {
			this.skinningDone = false;
		}
		
		public void Awake () {
			if (!SkinnableObject.loadFromURL) {
				SkinnableObject.assetBundle = new List<AssetBundle> ();
				string [] assetBundlesPath = Directory.GetFiles (Path.Combine (Application.dataPath, "../Skins/"));
				
				foreach (string path in assetBundlesPath) {
					assetBundle.Add (AssetBundle.CreateFromFile (path));
				}

				foreach (AssetBundle assBundled in SkinnableObject.assetBundle) {
					if (assBundled != null) {
						GameObject skinGameObject = assBundled.Load (this.name, typeof (GameObject)) as GameObject;
						if (skinGameObject != null) {
							GameObjectOperator.Add (this.gameObject, skinGameObject);
							this.skinningDone = true;
							return;
						}
					}
				}
				this.skinningDone = true;
			}
			else if (SkinnableObject.loadFromURL) {
				StartCoroutine ("CoroutinedAwake");
			}
		}

		public int dled = 0;

		public void OnGUI () {
			GUILayout.TextField ("Downloading skin... " + dled + " %");
			GUILayout.TextField ("Skinning done = " + this.skinningDone);
		}

		IEnumerator CoroutinedAwake () {
			WWW www = new WWW ("https://dl.dropboxusercontent.com/u/3130492/skin.unity3D");
			while (!www.isDone) {
				this.dled = Mathf.FloorToInt(www.progress * 100);
				yield return false;
			}

			SkinnableObject.assetBundle = new List<AssetBundle> ();
			SkinnableObject.assetBundle.Add (www.assetBundle);

			foreach (AssetBundle assBundled in SkinnableObject.assetBundle) {
				if (assBundled != null) {
					GameObject skinGameObject = assBundled.Load (this.name, typeof (GameObject)) as GameObject;
					if (skinGameObject != null) {
						GameObjectOperator.Add (this.gameObject, skinGameObject);
						this.skinningDone = true;
						
						// Debug assetBundle content
						UnityEngine.Object[] allInAssetBundle = assBundled.LoadAll ();
						foreach (UnityEngine.Object o in allInAssetBundle) {
							Debug.Log (o.name);
						}
						// End Debug assetBundle content
						
						yield break;
					}
				}
			}
			this.skinningDone = true;
			yield break;
		}
		
		public GameObject GetSkinPrefab (string skin) {
			int index = skins.IndexOf (skin);
			if (index != -1) {
				return this.diffPrefabs [index];
			}
			return null;
		}
		
		public void UseSkin (string skin) {
			int index = skins.IndexOf (skin);
			if (index != -1) {
				if (this.diffPrefabs [index] != null) {
					GameObjectOperator.Add (this.gameObject, this.diffPrefabs [index]);
				}
			}
		}
		
		public void DropSkin (string skin) {
			int index = skins.IndexOf (skin);
			if (index != -1) {
				if (this.diffPrefabs [index] != null) {
					GameObjectOperator.CancelAdd (this.gameObject, this.basePrefab, this.diffPrefabs [index]);
				}
			}
		}
		
		public void Reset () {
			this.skins = new List<string> ();
			this.basePrefab = null;
			this.skinPrefabs = new List<GameObject> ();
			this.diffPrefabs = new List<GameObject> ();
		}
	}
}
