using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(Stage))]
public class StageModelCreator : Editor {
	public override void OnInspectorGUI(){
		DrawDefaultInspector();
		if(GUILayout.Button("Create Model")){
			Stage s = target as Stage;
			s.createModel();
		}
	}
}
