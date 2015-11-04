using UnityEngine;
using UnityEditor;
using System;

public class SadyrinthOpenStage : ScriptableObject
{
	public const string STAGE_OPEN_STAGE = "Sadyrinth/Stage/Open Stage %`";

	[MenuItem (STAGE_OPEN_STAGE, true)]
	static bool ValidateSelectEnableODisable ()
	{
		object[] gobj = Selection.GetFiltered (typeof(object), SelectionMode.DeepAssets);

		if (gobj.Length == 1) {
			return true;
		}

		return false;
	}

	[MenuItem (STAGE_OPEN_STAGE)]
	static void stage_open_stage ()
	{
		object[] gobj = Selection.GetFiltered (typeof(object), SelectionMode.DeepAssets);

		if (gobj.Length == 1) {
			
			GameObject stagePrefab = gobj [0] as GameObject;
			Debug.Log ("Add to StageDebugContainer: " + stagePrefab.name);

			GameObject stageGameObject = Instantiate (stagePrefab, Vector3.zero, Quaternion.identity) as GameObject;
			stageGameObject.name = stageGameObject.name.Replace ("(Clone)", "");

			Transform stageDebugContainerTransform = GameObject.Find ("StageDebugContainer").transform;
			foreach (Transform childTransform in stageDebugContainerTransform) {
				DestroyImmediate (childTransform.gameObject);
			}

			stageGameObject.transform.SetParent (stageDebugContainerTransform);

			Ferr2D_Path[] paths = stageGameObject.GetComponentsInChildren<Ferr2D_Path> ();
			foreach (Ferr2D_Path path in paths) {
				path.ReCenter ();
			}
		}
	}
}
