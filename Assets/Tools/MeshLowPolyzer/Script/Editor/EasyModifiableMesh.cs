using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace CreativityZero.Utils.MeshModifier {
	
	public class EasyModifiableMesh {
		private List<Vertex> vertices;
		private List<Triangle> triangles;
		private Dictionary<string, Dictionary<int, Vertex>> clusters;
		private Dictionary<string, Vertex> splitEdges;

		public EasyModifiableMesh () {

		}

		#region BUILDER_PARTS
		// Contains functions for partial building of EasyModifiableMesh

		/// <summary>
		/// Fills the vertices list.
		/// </summary>
		/// <param name="mesh">Mesh.</param>
		private void FillVerticesList (Mesh mesh) {
			this.vertices = new List<Vertex> ();

			if (mesh.normals.Length != mesh.vertexCount) {
				Debug.Log ("No Normals");
				mesh.normals = new Vector3[mesh.vertexCount];
			}
			if (mesh.uv.Length != mesh.vertexCount) {
				Debug.Log ("No UV");
				mesh.uv = new Vector2[mesh.vertexCount];
			}
			if (mesh.uv1.Length != mesh.vertexCount) {
				Debug.Log ("No UV1");
				mesh.uv1 = new Vector2[mesh.vertexCount];
			}
			if (mesh.uv2.Length != mesh.vertexCount) {
				Debug.Log ("No UV2");
				mesh.uv2 = new Vector2[mesh.vertexCount];
			}
			if (mesh.boneWeights.Length != mesh.vertexCount) {
				Debug.Log ("No BoneWeights");
				mesh.boneWeights = new BoneWeight[mesh.vertexCount];
			}

			for (int i = 0; i < mesh.vertexCount; i++) {
				this.vertices.Add (new Vertex (i, mesh.vertices[i], mesh.normals[i], mesh.uv[i], mesh.uv1[i], mesh.uv2[i], mesh.boneWeights[i]));
			}
		}

		/// <summary>
		/// Fills the triangles list.
		/// </summary>
		/// <param name="mesh">Mesh.</param>
		private void FillTrianglesList (Mesh mesh) {
			this.triangles = new List<Triangle> ();

			for (int j = 0; j < mesh.triangles.Length / 3; j++) {
				Vertex p1 = this.vertices[mesh.triangles[3*j]];
				Vertex p2 = this.vertices[mesh.triangles[3*j + 1]];
				Vertex p3 = this.vertices[mesh.triangles[3*j + 2]];
				
				Triangle t = new Triangle (p1, p2, p3);
				
				this.triangles.Add (t);
				
				p1.AddTriangle (t);
				p2.AddTriangle (t);
				p3.AddTriangle (t);
			}
		}

		/// <summary>
		/// Computes the seams.
		/// </summary>
		private void ComputeSeams () {
			for (int k = 0; k < this.vertices.Count; k++) {
				for (int l = k + 1; l < this.vertices.Count; l++) {
					if (this.vertices[k].IsSeamWith (this.vertices[l])) {
						this.vertices[k].AddSeam (this.vertices[l]);
					}
				}
			}
		}

		#endregion

		#region SUBDIVISION
		// Contains function concerning mesh subdivision

		/// <summary>
		/// Splits the edge. You should previously make sure v1 and v2 are connected by an edge.
		/// </summary>
		/// <returns>The edge.</returns>
		/// <param name="v1">V1.</param>
		/// <param name="v2">V2.</param>
		private Vertex SplitEdge (Vertex v1, Vertex v2) {
			if (!this.splitEdges.ContainsKey (v1.ID + "|" + v2.ID)) {
				Vertex splitV = v1.SplitVertex (this.vertices.Count, v2);

				this.splitEdges.Add ((v1.ID + "|" + v2.ID), splitV);
				this.splitEdges.Add ((v2.ID + "|" + v1.ID), splitV);

				this.vertices.Add (splitV);

				return splitV;
			}
			else {
				return this.splitEdges[v1.ID + "|" + v2.ID];
			}
		}

		public void SubdivideNoSeamFactoring () {
			List<Triangle> trianglesBase = new List<Triangle> (this.triangles);
			int iteration = 0;
			foreach (Triangle t in trianglesBase) {
				iteration ++;
				List<Vertex> pts = t.Points;
				Vertex p1 = pts[0];
				Vertex p2 = pts[1];
				Vertex p3 = pts[2];
	
				Vertex p12 = this.SplitEdge (p1, p2);
				if (iteration == 400) {
					Debug.Log ("## ## ## ## ## ## ## ## ## ## ## ## ## ##");
					Debug.Log ("## p1");
					p1.ShowBWeight ();
					Debug.Log ("## p2");
					p2.ShowBWeight ();
					Debug.Log ("## p12");
					p12.ShowBWeight ();
				}
				Vertex p23 = this.SplitEdge (p2, p3);
				Vertex p31 = this.SplitEdge (p3, p1);

				p1.RemoveTriangleNoSeamFactoring (t);
				p2.RemoveTriangleNoSeamFactoring (t);
				p3.RemoveTriangleNoSeamFactoring (t);
				this.triangles.Remove (t);

				Triangle t1 = new Triangle (p1, p12, p31);
				this.triangles.Add (t1);
				p1.AddTriangle (t1);
				p12.AddTriangle (t1);
				p31.AddTriangle (t1);

				Triangle t2 = new Triangle (p2, p23, p12);
				this.triangles.Add (t2);
				p2.AddTriangle (t2);
				p23.AddTriangle (t2);
				p12.AddTriangle (t2);

				Triangle t3 = new Triangle (p3, p31, p23);
				this.triangles.Add (t3);
				p3.AddTriangle (t3);
				p31.AddTriangle (t3);
				p23.AddTriangle (t3);

				Triangle t4 = new Triangle (p12, p23, p31);
				this.triangles.Add (t4);
				p12.AddTriangle (t4);
				p23.AddTriangle (t4);
				p31.AddTriangle (t4);
			}
		}
		#endregion

		#region MESH_BUILD
		// Contains functions about mesh rebuild
		private Mesh BuildMesh (Matrix4x4[] bindposes) {
			Mesh newMesh = new Mesh ();
			newMesh.bindposes = bindposes;

			Vector3[] newVertices = new Vector3[this.vertices.Count];
			Vector3[] newNormals = new Vector3[this.vertices.Count];
			Vector2[] newUV = new Vector2[this.vertices.Count];
			Vector2[] newUV1 = new Vector2[this.vertices.Count];
			Vector2[] newUV2 = new Vector2[this.vertices.Count];
			BoneWeight[] newBoneWeights = new BoneWeight[this.vertices.Count];
			Dictionary<Vertex, int> invertedIndex = new Dictionary<Vertex, int> ();
			int i = 0;
			foreach (Vertex v in this.vertices) {
				newVertices[i] = v.Position;
				newNormals[i] = v.Normal;
				newUV[i] = v.Uv;
				newUV1[i] = v.Uv1;
				newUV2[i] = v.Uv2;
				newBoneWeights[i] = v.BWeight;
				invertedIndex.Add(v, i);
				i ++;
			}
			
			int[] newTriangles = new int[this.triangles.Count * 3];
			i = 0;
			foreach (Triangle t in this.triangles) {
				newTriangles[i] = invertedIndex[t.Points[0]];
				i ++;
				newTriangles[i] = invertedIndex[t.Points[1]];
				i ++;
				newTriangles[i] = invertedIndex[t.Points[2]];
				i ++;
			}
			
			newMesh.vertices = newVertices;
			newMesh.triangles = newTriangles;
			newMesh.normals = newNormals;
			newMesh.uv = newUV;
			newMesh.uv1 = newUV1;
			newMesh.uv2 = newUV2;
			newMesh.boneWeights = newBoneWeights;

			return newMesh;
		}

		private Mesh BuildMeshUnityNormalsComputation (Matrix4x4[] bindposes) {
			Mesh newMesh = this.BuildMesh (bindposes);

			newMesh.RecalculateNormals ();

			return newMesh;
		}
		#endregion

		#region MESH_MANIPULATION

		public Mesh DecimateMesh (Mesh mesh, float granu) {

			this.FillVerticesList (mesh);
			this.FillTrianglesList (mesh);
			this.FillClusterList (mesh, granu);
			this.GroupAllClusters ();
			this.FuseAllClusters ();
			//this.OrthogonalSmooth ();
			
			return this.BuildMeshUnityNormalsComputation (mesh.bindposes);
		}

		public Mesh SubdivideAndSmoothMesh (Mesh mesh) {
			this.splitEdges = new Dictionary<string, Vertex> ();
			
			this.FillVerticesList (mesh);
			this.FillTrianglesList (mesh);
			this.SubdivideNoSeamFactoring ();
			this.ComputeSeams ();

			this.LaplacianSmooth ();
			
			return this.BuildMesh (mesh.bindposes);
		}

		#endregion

		#region MESH_SMOOTH

		public Mesh SmoothMesh (Mesh mesh) {
			this.FillVerticesList (mesh);
			this.FillTrianglesList (mesh);
			this.ComputeSeams ();
			
			this.LaplacianSmooth ();
			
			return this.BuildMesh (mesh.bindposes);
		}

		private void LaplacianSmooth () {
			List<Vector3> newVectors = new List<Vector3> ();
			
			foreach (Vertex v in this.vertices) {
				if (v.ExtendedNeighboors.Count != 0) {
					Vector3 newVVector = new Vector3 (0f, 0f, 0f);
					foreach (Vertex nV in v.ExtendedNeighboors) {
						newVVector += nV.Position;
					}
					newVVector = newVVector / ((float) v.ExtendedNeighboors.Count);
					newVectors.Add (newVVector);
				}
				else {
					newVectors.Add (v.Position);
				}
			}
			
			for (int i = 0; i < newVectors.Count; i++) {
				this.vertices[i].Position = newVectors[i];
			}
		}

		private void NearlyLaplacianSmooth () {
			List<Vector3> newVectors = new List<Vector3> ();
			
			foreach (Vertex v in this.vertices) {
				if (v.ExtendedNeighboors.Count != 0) {
					Vector3 newVVector = new Vector3 (0f, 0f, 0f);
					foreach (Vertex nV in v.ExtendedNeighboors) {
						newVVector += nV.Position;
					}
					newVVector = newVVector / ((float) v.ExtendedNeighboors.Count);
					newVVector = (newVVector + v.Position) / 2f;
					newVectors.Add (newVVector);
				}
				else {
					newVectors.Add (v.Position);
				}
			}
			
			for (int i = 0; i < newVectors.Count; i++) {
				this.vertices[i].Position = newVectors[i];
			}
		}

		private void OrthogonalSmooth () {
			Debug.Log ("OrthogonalSmooth");
			List<Vector3> newVectors = new List<Vector3> ();
			
			foreach (Vertex v in this.vertices) {
				if (v.ExtendedNeighboors.Count != 0) {
					Vector3 newVVector = Vector3.zero;
					Vector3 localNorm = Vector3.zero;
					localNorm += v.Normal;
					foreach (Vertex sV in v.Seams) {
						localNorm += sV.Normal;
					}
					localNorm.Normalize ();
					foreach (Vertex nV in v.ExtendedNeighboors) {
						newVVector += Vector3.Dot(nV.Position - v.Position, localNorm) * localNorm;
					}
					newVVector = newVVector / ((float) v.ExtendedNeighboors.Count);
					newVVector = v.Position + newVVector;
					newVectors.Add (newVVector);
				}
				else {
					newVectors.Add (v.Position);
				}
			}
			
			for (int i = 0; i < newVectors.Count; i++) {
				this.vertices[i].Position = newVectors[i];
			}
		}

		#endregion

		#region MESH_CLUSTER

		private void FillClusterList (Mesh mesh, float granularity) {
			this.clusters = new Dictionary<string, Dictionary<int, Vertex>> ();
			
			string clusterKey;
			
			for (int i = 0; i < mesh.vertexCount; i++) {
				clusterKey = Mathf.FloorToInt(mesh.vertices[i].x / granularity) + "|" + Mathf.FloorToInt(mesh.vertices[i].y / granularity) + "|" + Mathf.FloorToInt(mesh.vertices[i].z / granularity);
				
				if (!this.clusters.ContainsKey(clusterKey)) {
					this.clusters.Add(clusterKey, new Dictionary<int, Vertex> ());
				}
				
				this.clusters[clusterKey].Add(i, this.vertices[i]);
			}
		}

		private void GroupCluster (Dictionary<int, Vertex> cluster) {
			Vector3 medianVector = Vector3.zero;
			BoneWeightCluster maxiBoneWeight = new BoneWeightCluster ();
			foreach (Vertex v in cluster.Values) {
				medianVector += v.Position;
				maxiBoneWeight.AddBoneWeight (v.BWeight);
			}
			
			medianVector /= cluster.Count;
			BoneWeight mediumBweight = maxiBoneWeight.ComputeBoneWeight ();
			
			foreach (Vertex v in cluster.Values) {
				v.Position = medianVector;
				v.BWeight = mediumBweight;
			}
		}
		
		private void GroupAllClusters () {
			foreach (Dictionary<int, Vertex> cluster in this.clusters.Values) {
				this.GroupCluster (cluster);
			}
		}

		private void FuseCluster (Dictionary<int, Vertex> cluster) {
			List<Vertex> vertices = new List<Vertex> (cluster.Values);
			
			for (int i = 0; i < vertices.Count; i++) {
				for (int j = i + 1; j < vertices.Count; j++) {
					if (vertices[i].Neighboors.Contains (vertices[j])) {
						vertices[i].Fuse (vertices[j], this.vertices, this.triangles);
					}
				}
			}
		}
		
		private void FuseAllClusters () {
			foreach (Dictionary<int, Vertex> cluster in this.clusters.Values) {
				this.FuseCluster (cluster);
			}
		}

		#endregion
	}
}
