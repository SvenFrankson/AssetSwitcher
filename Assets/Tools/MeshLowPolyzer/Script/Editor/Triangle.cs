using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace CreativityZero.Utils.MeshModifier {

	public class Triangle {
		private List<Vertex> points;
		public List<Vertex> Points {
			get {
				return this.points;
			}
		}

		public Triangle (Vertex v1, Vertex v2, Vertex v3) {
			this.points = new List<Vertex> ();
			this.points.Add (v1);
			this.points.Add (v2);
			this.points.Add (v3);
		}

		public void Replace (Vertex oldV, Vertex newV) {
			for (int i = 0; i < this.points.Count; i++) {
				if (this.points[i] == oldV) {
					this.points[i] = newV;
				}
				newV.AddTriangle (this);
			}
		}
	}
}
