using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace CreativityZero.Skinning {

	public class CompareComponent {

		static public bool Compare (MeshFilter A, MeshFilter B) {
			if (A.sharedMesh != B.sharedMesh) {
				return false;
			}

			return true;
		}

		static public bool Compare (MeshRenderer A, MeshRenderer B) {
			if (A.sharedMaterials.Length != B.sharedMaterials.Length) {
				return false;
			}
			for (int i = 0; i < A.sharedMaterials.Length; i++) {
				if (A.sharedMaterials[i].name != B.sharedMaterials[i].name) {
					return false;
				}
			}

			return true;
		}
		
		static public bool Compare (SkinnedMeshRenderer A, SkinnedMeshRenderer B) {
			if (A.sharedMesh != B.sharedMesh) {
				return false;
			}
			if (A.sharedMaterials.Length != B.sharedMaterials.Length) {
				return false;
			}
			for (int i = 0; i < A.sharedMaterials.Length; i++) {
				if (A.sharedMaterials[i].name != B.sharedMaterials[i].name) {
					return false;
				}
			}
			
			return true;
		}
	}
}
