using UnityEngine;
using UnityEditor;
using System.Collections;

public class SynchroImporterWindow : EditorWindow {

	private GameObject targetFBX;

	static private Object LoadAssetNamed (string name, System.Type type) {
		string[] allPaths = AssetDatabase.GetAllAssetPaths ();
		foreach (string path in allPaths) {
			if (path.Contains (name)) {
				return AssetDatabase.LoadAssetAtPath (path, type);
			}
		}

		return null;
	}

	private GameObject fixedFBX = null;
	private GameObject FixedFBX {
		get {
			if (fixedFBX == null) {
				fixedFBX = LoadAssetNamed ("FixedFBX1.prefab", typeof (GameObject)) as GameObject;
				GameObject quad = fixedFBX.transform.Find ("Quad").gameObject;
				quad.GetComponent <MeshRenderer> ().sharedMaterials = new Material[] {LoadAssetNamed ("scale.mat", typeof(Material)) as Material};
				quad.GetComponent <MeshRenderer> ().sharedMaterials[0].SetTexture (0, LoadAssetNamed ("scale.png", typeof(Texture2D)) as Texture2D);
			}

			return fixedFBX;
		}
	}

	private GameObject targetWithFix = null;

	private Editor targetFBXEditor;

	private Material whiteMaterial = null;
	private Material WhiteMaterial {
		get {
			if (whiteMaterial == null) {
				whiteMaterial = LoadAssetNamed ("OnyxSynchroImporterTransparentWhite", typeof (Material)) as Material;
			}

			return whiteMaterial;
		}
	}

	private string targetAssetPath = "";
	
	static public void OpenWindow (string assetPath) {
		SynchroImporterWindow win = EditorWindow.GetWindow<SynchroImporterWindow> ("Onyx Synchro Importer", true) as SynchroImporterWindow;
		win.minSize = new Vector2 (400f, 600f);
		win.targetAssetPath = assetPath;
		win.BuildTargetWithFix ();
	}

	public void BuildTargetWithFix () {
		this.targetFBX = AssetDatabase.LoadAssetAtPath (targetAssetPath, typeof (GameObject)) as GameObject;
		Selection.activeObject = this.targetFBX;
		DestroyImmediate (this.targetFBXEditor);

		if ((this.FixedFBX != null) && (this.targetFBX != null)) {

			GameObject targetWithFixInstance = new GameObject ("Tmp");
			targetWithFixInstance.transform.position = Vector3.zero;
			targetWithFixInstance.transform.rotation = Quaternion.identity;
			targetWithFixInstance.transform.localScale = Vector3.one;

			GameObject fixInstance = GameObject.Instantiate (this.FixedFBX) as GameObject;
			fixInstance.transform.parent = targetWithFixInstance.transform;
			fixInstance.transform.localPosition = Vector3.zero;
			fixInstance.transform.localRotation = Quaternion.identity;
			fixInstance.transform.localScale = Vector3.one;
			
			GameObject targetInstance = GameObject.Instantiate (this.targetFBX) as GameObject;
			targetInstance.transform.parent = targetWithFixInstance.transform;
			targetInstance.transform.localPosition = Vector3.zero;
			targetInstance.transform.localRotation = Quaternion.identity;
			targetInstance.transform.localScale = Vector3.one;
			SetMaterialToWhite (targetInstance);

			this.targetWithFix = PrefabUtility.CreatePrefab ("Assets/tmpPreview.prefab", targetWithFixInstance) as GameObject;

			DestroyImmediate (targetWithFixInstance);
			
			this.targetFBXEditor = Editor.CreateEditor (targetWithFix);
		}
		else {
			this.Close ();
		}
	}

	public void SetMaterialToWhite (GameObject target) {
		MeshRenderer[] mrs = target.GetComponentsInChildren<MeshRenderer> ();
		SkinnedMeshRenderer[] smrs = target.GetComponentsInChildren<SkinnedMeshRenderer> ();

		foreach (MeshRenderer mr in mrs) {
			Material[] newMaterials = new Material[mr.sharedMaterials.Length];
			for (int i = 0; i < mr.sharedMaterials.Length; i++) {
				newMaterials [i] = WhiteMaterial;
			}
			mr.sharedMaterials = newMaterials;
		}

		foreach (SkinnedMeshRenderer smr in smrs) {
			Material[] newMaterials = new Material[smr.sharedMaterials.Length];
			for (int i = 0; i < smr.sharedMaterials.Length; i++) {
				newMaterials [i] = WhiteMaterial;
			}
			smr.sharedMaterials = newMaterials;
		}
	}

	public void OnGUI () {
		if (this.targetFBXEditor) {
			EditorGUILayout.LabelField ("Onyx Synchro Importer", EditorStyles.boldLabel);
			EditorGUILayout.Space ();
			targetFBXEditor.OnPreviewGUI (GUILayoutUtility.GetRect (330f, 330f), GUIStyle.none);
			if (GUILayout.Button ("Close", GUILayout.Height (40f))) {
				this.Close ();
			}
		}
	}

	public void OnDestroy () {
		AssetDatabase.DeleteAsset (AssetDatabase.GetAssetPath (this.targetWithFix));
		DestroyImmediate (this.targetFBXEditor);
	}
}
