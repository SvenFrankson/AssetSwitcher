using System.IO;
using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System;

namespace CreativityZero.Utils.MeshModifier {
	
	public class BoneWeightCluster {
		private Dictionary<int, float> maxiBoneWeights;
		
		public BoneWeightCluster () {
			this.maxiBoneWeights = new Dictionary<int, float> ();
		}
		
		public void AddBoneWeight (BoneWeight bweight) {
			if (!this.maxiBoneWeights.ContainsKey (bweight.boneIndex0)) {
				this.maxiBoneWeights.Add (bweight.boneIndex0, bweight.weight0);
			}
			else {
				this.maxiBoneWeights[bweight.boneIndex0] += bweight.weight0;
			}
			
			if (!this.maxiBoneWeights.ContainsKey (bweight.boneIndex1)) {
				this.maxiBoneWeights.Add (bweight.boneIndex1, bweight.weight1);
			}
			else {
				this.maxiBoneWeights[bweight.boneIndex1] += bweight.weight1;
			}
			
			if (!this.maxiBoneWeights.ContainsKey (bweight.boneIndex2)) {
				this.maxiBoneWeights.Add (bweight.boneIndex2, bweight.weight2);
			}
			else {
				this.maxiBoneWeights[bweight.boneIndex2] += bweight.weight2;
			}
			
			if (!this.maxiBoneWeights.ContainsKey (bweight.boneIndex3)) {
				this.maxiBoneWeights.Add (bweight.boneIndex3, bweight.weight3);
			}
			else {
				this.maxiBoneWeights[bweight.boneIndex3] += bweight.weight3;
			}
		}
		
		private KeyValuePair<int, float> PopTop () {
			KeyValuePair<int, float> topPair = new KeyValuePair<int, float> (0, 0f);
			
			foreach (KeyValuePair<int, float> kvp in this.maxiBoneWeights) {
				if (kvp.Value > topPair.Value) {
					topPair = kvp;
				}
			}
			
			this.maxiBoneWeights.Remove (topPair.Key);
			
			return topPair;
		}
		
		public BoneWeight ComputeBoneWeight () {
			BoneWeight mediumBoneWeight = new BoneWeight ();
			float sum = 0f;
			
			KeyValuePair<int, float> top = this.PopTop ();
			mediumBoneWeight.boneIndex0 = top.Key;
			mediumBoneWeight.weight0 = top.Value;
			sum += top.Value;
			
			top = this.PopTop ();
			mediumBoneWeight.boneIndex1 = top.Key;
			mediumBoneWeight.weight1 = top.Value;
			sum += top.Value;
			
			top = this.PopTop ();
			mediumBoneWeight.boneIndex2 = top.Key;
			mediumBoneWeight.weight2 = top.Value;
			sum += top.Value;
			
			top = this.PopTop ();
			mediumBoneWeight.boneIndex3 = top.Key;
			mediumBoneWeight.weight3 = top.Value;
			sum += top.Value;
			
			mediumBoneWeight.weight0 = mediumBoneWeight.weight0 / sum;
			mediumBoneWeight.weight1 = mediumBoneWeight.weight1 / sum;
			mediumBoneWeight.weight2 = mediumBoneWeight.weight2 / sum;
			mediumBoneWeight.weight3 = mediumBoneWeight.weight3 / sum;
			
			return mediumBoneWeight;
		}
	}
}
