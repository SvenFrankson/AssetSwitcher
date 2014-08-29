using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace CreativityZero.Skinning {
	
	public class GameObjectOperator {

		static public void CancelAdd (GameObject targetGameObject, GameObject baseGameObject, GameObject diffGameObject) {

			MeshFilter targetMeshFilter = targetGameObject.GetComponent<MeshFilter> ();
			
			if (targetMeshFilter != null) {
				MeshFilter baseMeshFilter = baseGameObject.GetComponent<MeshFilter> ();

				if (baseMeshFilter != null) {
					targetMeshFilter.sharedMesh = baseMeshFilter.sharedMesh;
				}
			}
			
			MeshRenderer targetMeshRenderer = targetGameObject.GetComponent<MeshRenderer> ();
			
			if (targetMeshRenderer != null) {
				MeshRenderer baseMeshRenderer = baseGameObject.GetComponent<MeshRenderer> ();
				
				if (baseMeshRenderer != null) {
					targetMeshRenderer.sharedMaterials = baseMeshRenderer.sharedMaterials;
				}
			}
			
			SkinnedMeshRenderer targetSkinnedMeshRenderer = targetGameObject.GetComponent<SkinnedMeshRenderer> ();
			
			if (targetSkinnedMeshRenderer != null) {
				SkinnedMeshRenderer baseSkinnedMeshRenderer = baseGameObject.GetComponent<SkinnedMeshRenderer> ();
				
				if (baseSkinnedMeshRenderer != null) {
					targetSkinnedMeshRenderer.sharedMesh = baseSkinnedMeshRenderer.sharedMesh;
					targetSkinnedMeshRenderer.sharedMaterials = baseSkinnedMeshRenderer.sharedMaterials;
				}
			}
			
			List<GameObject> targetChildren = new List<GameObject> ();
			List<GameObject> baseChildren = new List<GameObject> ();
			List<GameObject> diffChildren = new List<GameObject> ();
			
			for (int i = 0; i < targetGameObject.transform.childCount; i++) {
				targetChildren.Add (targetGameObject.transform.GetChild (i).gameObject);
			}
			for (int i = 0; i < baseGameObject.transform.childCount; i++) {
				baseChildren.Add (baseGameObject.transform.GetChild (i).gameObject);
			}
			for (int i = 0; i < diffGameObject.transform.childCount; i++) {
				diffChildren.Add (diffGameObject.transform.GetChild (i).gameObject);
			}
			
			foreach (GameObject targetChild in targetChildren) {
				bool inBase = false;
				GameObject baseChildFound = null;
				foreach (GameObject baseChild in baseChildren) {
					if (baseChild.name == targetChild.name) {
						inBase = true;
						baseChildFound = baseChild;
					}
				}
				bool inDiff = false;
				GameObject diffChildFound = null;
				foreach (GameObject diffChild in diffChildren) {
					if (diffChild.name == targetChild.name) {
						inDiff = true;
						diffChildFound = diffChild;
					}
				}

				if (!inBase && inDiff) {
					GameObject.DestroyImmediate (targetChild);
				}
				else if (inBase && inDiff) {
					CancelAdd (targetChild, baseChildFound, diffChildFound);
				}
			}
		}

		/// <summary>
		/// Add the specified baseGO and addGO.
		/// </summary>
		/// <param name="baseGO">Base G.</param>
		/// <param name="addGO">Add G.</param>
		static public void Add (GameObject baseGO, GameObject addGO) {

			MeshFilter addMeshFilter = addGO.GetComponent<MeshFilter> ();

			if (addMeshFilter != null) {
				MeshFilter baseMeshFilter = baseGO.GetComponent<MeshFilter> ();

				if (baseMeshFilter == null) {
					baseMeshFilter = baseGO.AddComponent<MeshFilter> ();
				}

				baseMeshFilter.sharedMesh = addMeshFilter.sharedMesh;
			}

			MeshRenderer addMeshRenderer = addGO.GetComponent<MeshRenderer> ();
			
			if (addMeshRenderer != null) {
				MeshRenderer baseMeshRenderer = baseGO.GetComponent<MeshRenderer> ();
				
				if (baseMeshRenderer == null) {
					baseMeshRenderer = baseGO.AddComponent<MeshRenderer> ();
				}
				
				baseMeshRenderer.sharedMaterials = addMeshRenderer.sharedMaterials;
			}
			
			SkinnedMeshRenderer addSkinnedMeshRenderer = addGO.GetComponent<SkinnedMeshRenderer> ();
			
			if (addSkinnedMeshRenderer != null) {
				SkinnedMeshRenderer baseSkinnedMeshRenderer = baseGO.GetComponent<SkinnedMeshRenderer> ();
				
				if (baseSkinnedMeshRenderer == null) {
					baseSkinnedMeshRenderer = baseGO.AddComponent<SkinnedMeshRenderer> ();
				}
				
				baseSkinnedMeshRenderer.sharedMesh = addSkinnedMeshRenderer.sharedMesh;
				baseSkinnedMeshRenderer.sharedMaterials = addSkinnedMeshRenderer.sharedMaterials;
			}

			List<GameObject> baseChildren = new List<GameObject> ();
			List<GameObject> addChildren = new List<GameObject> ();
			
			for (int i = 0; i < baseGO.transform.childCount; i++) {
				baseChildren.Add (baseGO.transform.GetChild (i).gameObject);
			}
			
			for (int i = 0; i < addGO.transform.childCount; i++) {
				addChildren.Add (addGO.transform.GetChild (i).gameObject);
			}
			
			foreach (GameObject gAdd in addChildren) {
				bool fused = false;
				foreach (GameObject gBase in baseChildren) {
					if (gAdd.name == gBase.name) {
						Add (gBase, gAdd);
						fused = true;
					}
				}

				if (!fused) {
					GameObject addInstance = GameObject.Instantiate (gAdd) as GameObject;
					addInstance.transform.parent = baseGO.transform;
					addInstance.name = gAdd.name;
					addInstance.transform.localPosition = gAdd.transform.localPosition;
					addInstance.transform.localRotation = gAdd.transform.localRotation;
					addInstance.transform.localScale = gAdd.transform.localScale;
				}
			}
		}

		/// <summary>
		/// Substract the specified baseGameObject and diffGameObject.
		/// </summary>
		/// <param name="baseGameObject">Base game object.</param>
		/// <param name="diffGameObject">Diff game object.</param>
		/// - BaseGameObject est le prefab d'origine. Ce serait le Happy classique.
		/// - DiffGameObject est une copie du prefab d'évènement. Ce serait le Happy avec chapeau amusant.
		/// - Substract compare composant par composant ceux de DiffGameObject et BaseGameObject,
		/// et supprime de DiffGameObject ceux qui sont présent dans le meme état dans BaseGameObject.
		/// - Une fois l'opération effectuée, elle est réitérée sur les paires d'enfants de BaseGameObject et DiffGameObject 
		/// qui ont le meme nom.
		static public void Substract (GameObject baseGO, GameObject diffGO) {

			Component[] diffComponents = diffGO.GetComponents<Component> ();

			foreach (Component c in diffComponents) {
				if (c.GetType () != typeof (Transform)) {
					if (c.GetType () != typeof (MeshFilter)) {
						if (c.GetType () != typeof (MeshRenderer)) {
							if (c.GetType () != typeof (SkinnedMeshRenderer)) {
								GameObject.DestroyImmediate (c);
							}
						}
					}
				}
			}

			MeshFilter diffMeshFilter = diffGO.GetComponent<MeshFilter> ();
			MeshFilter baseMeshFilter = baseGO.GetComponent<MeshFilter> ();

			if ((diffMeshFilter != null) && (baseMeshFilter != null)) {
				// On compare les MeshFilter, et on détruit si besoin.
				if (CompareComponent.Compare (diffMeshFilter, baseMeshFilter)) {
					GameObject.DestroyImmediate (diffMeshFilter);
				}
			}
			
			MeshRenderer diffMeshRenderer = diffGO.GetComponent<MeshRenderer> ();
			MeshRenderer baseMeshRenderer = baseGO.GetComponent<MeshRenderer> ();

			if ((diffMeshRenderer != null) && (baseMeshRenderer != null)) {
				// On compare les MeshRenderer, et on détruit si besoin.
				if (CompareComponent.Compare (diffMeshRenderer, baseMeshRenderer)) {
					GameObject.DestroyImmediate (diffMeshRenderer);
				}
			}

			SkinnedMeshRenderer diffSkinnedMeshRenderer = diffGO.GetComponent<SkinnedMeshRenderer> ();
			SkinnedMeshRenderer baseSkinnedMeshRenderer = baseGO.GetComponent<SkinnedMeshRenderer> ();

			if ((diffSkinnedMeshRenderer != null) && (baseSkinnedMeshRenderer != null)) {
				// On compare les SkinnedMeshRenderer, et on détruit si besoin.
				if (CompareComponent.Compare (diffSkinnedMeshRenderer, baseSkinnedMeshRenderer)) {
					GameObject.DestroyImmediate (diffSkinnedMeshRenderer);
				}
			}

			List<GameObject> baseChildren = new List<GameObject> ();
			List<GameObject> skinnedChildren = new List<GameObject> ();

			for (int i = 0; i < baseGO.transform.childCount; i++) {
				baseChildren.Add (baseGO.transform.GetChild (i).gameObject);
			}
			
			for (int i = 0; i < diffGO.transform.childCount; i++) {
				skinnedChildren.Add (diffGO.transform.GetChild (i).gameObject);
			}

			foreach (GameObject gSkinned in skinnedChildren) {
				foreach (GameObject gBase in baseChildren) {
					if (gSkinned.name == gBase.name) {
						Substract (gBase, gSkinned);
					}
				}
			}
		}
	}
}