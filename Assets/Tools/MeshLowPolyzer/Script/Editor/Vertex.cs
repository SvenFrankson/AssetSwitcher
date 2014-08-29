using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace CreativityZero.Utils.MeshModifier {

	public class Vertex {
		private int idInsert;
		public int ID {
			get {
				return this.idInsert;
			}
		}

		private Vector3 position;
		public Vector3 Position {
			set {
				this.position = value;
			}
			get {
				return this.position;
			}
		}

		private Vector3 normal;
		public Vector3 Normal {
			get {
				return this.normal;
			}
		}

		private Vector2 uv;
		public Vector2 Uv {
			get {
				return this.uv;
			}
		}

		private Vector2 uv1;
		public Vector2 Uv1 {
			get {
				return this.uv1;
			}
		}

		private Vector2 uv2;
		public Vector2 Uv2 {
			get {
				return this.uv2;
			}
		}

		private BoneWeight bWeight;
		public BoneWeight BWeight {
			get {
				return this.bWeight;
			}
			set {
				this.bWeight = value;
			}
		}

		private List<Vertex> seams;
		public List<Vertex> Seams {
			get {
				return this.seams;
			}
		}

		private List<Vertex> neighboors;
		private void neighboorsAdd (Vertex v) {
			if (v != this) {
				if (!this.neighboors.Contains (v)) {
					this.neighboors.Add (v);
					this.extendedNeighboors.Add (v);
				}
			}
		}
		public List<Vertex> Neighboors {
			get {
				return this.neighboors;
			}
		}

		private List<Vertex> extendedNeighboors;
		private void extendedNeighboorsAdd (Vertex v) {
			if (v != this) {
				if (!this.extendedNeighboors.Contains (v)) {
					this.extendedNeighboors.Add (v);
				}
			}
		}
		public List<Vertex> ExtendedNeighboors {
			get {
				return this.extendedNeighboors;
			}
		}

		private List<Triangle> triangles;
		private void trianglesAdd (Triangle t) {
			if (!this.triangles.Contains (t)) {
				this.triangles.Add (t);
				this.extendedTriangles.Add (t);
			}
		}
		public List<Triangle> Triangles {
			get {
				return this.triangles;
			}
		}

		private List<Triangle> extendedTriangles;
		private void extendedTrianglesAdd (Triangle t) {
			if (!this.extendedTriangles.Contains (t)) {
				this.extendedTriangles.Add (t);
			}
		}
		public List<Triangle> ExtendedTriangles {
			get {
				return this.extendedTriangles;
			}
		}

		public Vertex (int iidInsert, Vector3 iPosition, Vector3 iNormal, Vector2 iUv, Vector2 iUv1, Vector2 iUv2, BoneWeight iBWeight) {
			this.idInsert = iidInsert;
			this.position = iPosition;
			this.normal = iNormal;
			this.uv = iUv;
			this.uv1 = iUv1;
			this.uv2 = iUv2;
			this.bWeight = iBWeight;

			this.seams = new List<Vertex> ();
			this.neighboors = new List<Vertex> ();
			this.extendedNeighboors = new List<Vertex> ();
			this.triangles = new List<Triangle> ();
			this.extendedTriangles = new List<Triangle> ();
		}

		public void Fuse (Vertex v, List<Vertex> vertexList, List<Triangle> triangleList) {
			foreach (Triangle tri in this.triangles) {
				if (tri.Points.Contains (v)) {
					triangleList.Remove (tri);
				}
				else {
					tri.Replace (this, v);
				}
			}
			
			v.uv = (this.uv + v.Uv) / 2f;
			v.uv1 = (this.uv1 + v.Uv1) / 2f;
			v.uv2 = (this.uv2 + v.Uv2) / 2f;
			
			vertexList.Remove (this);
		}

		#region TRIANGLE_FACTORING
		// Contains all functions related with triangles detection and comprehension.
		// Should be invoked while parsing mesh.triangles.

		/// <summary>
		/// Adds the triangle. A vertex has a triangle if it's one corner of the triangle.
		/// </summary>
		/// <param name="t">T.</param>
		public void AddTriangle (Triangle t) {
			if (!this.triangles.Contains (t)) {
				this.trianglesAdd (t);

				foreach (Vertex v in t.Points) {
					this.neighboorsAdd (v);
				}
			}
		}

		public void RemoveTriangleNoSeamFactoring (Triangle t) {			
			if (this.triangles.Contains (t)) {
				this.triangles.Remove (t);

				foreach (Vertex v in t.Points) {
					this.neighboors.Remove (v);
					this.extendedNeighboors.Remove (v);
				}
			}
		}
		#endregion

		#region SEAM_FACTORING
		// Contains all functions related with seams detection and comprehension.
		// Seams vertices are required to apply uv textures on mesh.
		// Should be invoked while parsing mesh.vertices X mesh.vertices.

		public bool IsSeamWith (Vertex v) {
			return (this.position == v.Position);
		}

		/// <summary>
		/// Adds the seam. Two vertices are "seam" if they share the same position. Function will also invoke v.AddSeam (this)
		/// </summary>
		/// <param name="v">V.</param>
		public void AddSeam (Vertex v) {
			if (!this.seams.Contains (v)) {
				this.seams.Add (v);
				this.FuseNeighboors (v);
				this.FuseTriangles (v);

				v.AddSeam (this);
			}
		}

		/// <summary>
		/// Fuses the neighboors. Two vertices are "neighboors" if they share an edge, and "extendedNeighboors" if they or one of their seam share an edge.
		/// </summary>
		/// <param name="v">V.</param>
		public void FuseNeighboors (Vertex v) {
			foreach (Vertex n in v.ExtendedNeighboors) {
				this.extendedNeighboorsAdd (n);
			}
		}

		/// <summary>
		/// Fuses the triangles. A vertex has a triangle if it's one corner of the triangle. A vertex has an extended triangle if it's or on of its seam is one corner of the triangle.
		/// </summary>
		/// <param name="v">V.</param>
		public void FuseTriangles (Vertex v) {
			foreach (Triangle t in v.ExtendedTriangles) {
				this.extendedTrianglesAdd (t);
			}
		}
		#endregion

		#region SUBDIVISION_FACTORING
		// Contains all functions related with vertices division.

		private BoneWeight BWeightMedium (BoneWeight b1, BoneWeight b2) {
			BoneWeightCluster mBWeight = new BoneWeightCluster ();
			mBWeight.AddBoneWeight (b1);
			mBWeight.AddBoneWeight (b2);

			return mBWeight.ComputeBoneWeight ();
		}

		/// <summary>
		/// Splits this and v in a new vertex.
		/// </summary>
		/// <returns>The new vertex.</returns>
		/// <param name="v">V.</param>
		public Vertex SplitVertex (int iidInsert, Vertex v) {
			if (!this.neighboors.Contains (v)) {
				return null;
			}

			Vertex sV = new Vertex (iidInsert,
									(this.Position + v.Position) / 2f,
			                        (this.Normal + v.Normal).normalized,
			                        (this.Uv + v.Uv) / 2f,
			                        (this.Uv1 + v.Uv1) / 2f,
			                        (this.Uv2 + v.Uv2) / 2f,
			                        this.BWeightMedium(this.BWeight, v.BWeight));

			return sV;
		}
		#endregion

		#region DEBUG
		public void ShowBWeight () {
			Debug.Log (this.bWeight.boneIndex0 + " | " + this.bWeight.weight0);
			Debug.Log (this.bWeight.boneIndex1 + " | " + this.bWeight.weight1);
			Debug.Log (this.bWeight.boneIndex2 + " | " + this.bWeight.weight2);
			Debug.Log (this.bWeight.boneIndex3 + " | " + this.bWeight.weight3);
		}
		#endregion
	}
}
