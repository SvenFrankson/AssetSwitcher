using UnityEngine;
using UnityEditor;
using System.Collections;

public class SynchroImporterPostProcessor : AssetPostprocessor {

	public void OnPostprocessModel (GameObject g) {
		if (g.name != "fixed") {
			SynchroImporterWindow.OpenWindow (assetPath);
		}
	}
}
