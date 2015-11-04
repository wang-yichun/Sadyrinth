#if UNITY_EDITOR

using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor (typeof(StageDebugContainerController))]
public class StageDebugContainerEditor : Editor
{
	public override void OnInspectorGUI ()
	{
		base.DrawDefaultInspector ();

		serializedObject.Update ();

		EditorGUILayout.HelpBox ("Show environment need this.", MessageType.None);

		if (GUILayout.Button ("Ferr2D Terrain - Center Position")) {
			CenterPosition ();
		}

		serializedObject.ApplyModifiedProperties ();
	}

	public void CenterPosition ()
	{
		StageDebugContainerController containerController = target as StageDebugContainerController;

		Ferr2D_Path[] paths = containerController.GetComponentsInChildren<Ferr2D_Path> ();
		foreach (Ferr2D_Path path in paths) {
			path.ReCenter ();
		}
	}
}

#endif