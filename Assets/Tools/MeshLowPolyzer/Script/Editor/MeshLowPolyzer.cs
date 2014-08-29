using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEditor;
using System.Collections;
using System;
using CreativityZero.Utils;

namespace CreativityZero.Utils.MeshModifier {
	
	[InitializeOnLoad]
	public class MeshLowPolyzer : EditorWindow {

		public GameObject target = null;
		public GameObject checkChangeTarget = null;
		public EasyModifiableMesh easyMesh = new EasyModifiableMesh ();

		public int nbDecimations;

		public int coefGranularity = 3;
		
		public List<MeshFilter> meshFilters;
		public List<Mesh> meshFiltersMeshes;
		public List<SkinnedMeshRenderer> skinnedMeshRenderers;
		public List<Mesh> skinnedMeshRenderersMeshes;

		static MeshLowPolyzer () {

		}

		[MenuItem("Window/MeshLowPolyzer")]
		static void ShowPreferences()
		{
			EditorWindow window = GetWindow<MeshLowPolyzer>();
			window.minSize = new Vector2(400, 300);
			window.Show();
		}
		
		void OnGUI()
		{			
			this.target = EditorGUILayout.ObjectField ("Cible", this.target, typeof(GameObject), true) as GameObject;
			if (this.checkChangeTarget != this.target) {
				this.checkChangeTarget = this.target;
				if (this.target != null) {
					this.CaptureOriginalState ();
				}
			}

			this.coefGranularity = EditorGUILayout.IntField ("LOD Coef", this.coefGranularity);

			GUILayout.BeginHorizontal ();
			if (GUILayout.Button ("Decimate")) {
				if (this.target != null) {
					this.ResetToOriginalState ();
					this.Decimate ();
				}
			}
			if (GUILayout.Button ("Save")) {
				if (this.target != null) {
					this.SaveMeshInAssetDatabase ();
					this.CaptureOriginalState ();
				}
			}
			if (GUILayout.Button ("Cancel")) {
				if (this.target != null) {
					this.ResetToOriginalState ();
				}
			}
			if (GUILayout.Button ("Close")) {
				if (this.target != null) {
					this.Close ();
				}
			}
			GUILayout.EndHorizontal ();
		}

		private void Decimate () {

			foreach (MeshFilter mf in this.meshFilters) {
				float granu = this.ComputeGranularity (mf.sharedMesh);
				mf.sharedMesh = this.easyMesh.DecimateMesh (mf.sharedMesh, granu * (float) this.coefGranularity / 10f);
			}
			
			foreach (SkinnedMeshRenderer smr in this.skinnedMeshRenderers) {
				float granu = this.ComputeGranularity (smr.sharedMesh);
				smr.sharedMesh = this.easyMesh.DecimateMesh (smr.sharedMesh, granu * (float) this.coefGranularity / 10f);
			}
		}

		private float ComputeGranularity (Mesh m) {
			float granularity = 0;
			
			for (int i = 0; i < m.triangles.Length / 3; i++) {
				granularity += (m.vertices[m.triangles[3 * i]] - m.vertices[m.triangles[3 * i + 1]]).magnitude;
				granularity += (m.vertices[m.triangles[3 * i + 1]] - m.vertices[m.triangles[3 * i + 2]]).magnitude;
				granularity += (m.vertices[m.triangles[3 * i + 2]] - m.vertices[m.triangles[3 * i]]).magnitude;
			}
			
			granularity /= (float) m.triangles.Length;
			
			return granularity;
		}

		private void CaptureOriginalState () {
			this.meshFilters = new List<MeshFilter> ();
			this.meshFiltersMeshes = new List<Mesh> ();
			this.skinnedMeshRenderers = new List<SkinnedMeshRenderer> ();
			this.skinnedMeshRenderersMeshes = new List<Mesh> ();

			MeshFilter[] mfs = this.target.GetComponentsInChildren<MeshFilter> ();
			SkinnedMeshRenderer[] smrs = this.target.GetComponentsInChildren<SkinnedMeshRenderer> ();

			foreach (MeshFilter mf in mfs) {
				this.meshFilters.Add (mf);
				this.meshFiltersMeshes.Add (mf.sharedMesh);
			}

			foreach (SkinnedMeshRenderer smr in smrs) {
				this.skinnedMeshRenderers.Add (smr);
				this.skinnedMeshRenderersMeshes.Add (smr.sharedMesh);
			}
		}

		private void ResetToOriginalState () {
			if ((this.meshFiltersMeshes == null) || (this.skinnedMeshRenderersMeshes == null)) {
				this.CaptureOriginalState ();
			}

			for (int i = 0; i < this.meshFilters.Count; i++) {
				this.meshFilters [i].sharedMesh = this.meshFiltersMeshes [i];
			}
			for (int i = 0; i < this.skinnedMeshRenderers.Count; i++) {
				this.skinnedMeshRenderers [i].sharedMesh = this.skinnedMeshRenderersMeshes [i];
			}
		}

		private void SaveMeshInAssetDatabase () {
			
			for (int i = 0; i < this.meshFilters.Count; i++) {
				AssetDatabase.CreateAsset (this.meshFilters [i].sharedMesh, "Assets/Tools/MeshDecimator/Mesh/" + this.target.name + "_" + this.meshFiltersMeshes [i].name + ".asset");
			}

			for (int i = 0; i < this.skinnedMeshRenderers.Count; i++) {
				AssetDatabase.CreateAsset (this.skinnedMeshRenderers [i].sharedMesh, "Assets/Tools/MeshDecimator/Mesh/" + this.target.name + "_" + this.skinnedMeshRenderersMeshes [i].name + ".asset");
			}
		}
	}
}
