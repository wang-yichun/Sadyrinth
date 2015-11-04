// MyEditorTool.cs
using UnityEngine;
using UnityEditor;
using System;

public class MyEditorTool : ScriptableObject
{
	//  设置菜单Tool 下的 MyTool 下的 Enable\Disable Multi GameObj 快捷键为  command 加shift 加 d  <MAC上的>
	public const string MENU_DISABLE_SELECTED_GAMEOBJ_RECURSION = "Sadyrinth/GameObject/Enable\\Disable Multi GameObject(RECURSION) &a";   //%#d 即代表 command 加shift 加 d快捷键
	public const string MENU_DISABLE_SELECTED_GAMEOBJ = "Sadyrinth/GameObject/Enable\\Disable Multi GameObject _a";   //%#d 即代表 command 加shift 加 d快捷键
	public const string MENU_POSITION_ZERO_GAMEOBJ = "Sadyrinth/GameObject/Clear GameObject Position &p";

	[MenuItem(MENU_DISABLE_SELECTED_GAMEOBJ_RECURSION,true)]
	[MenuItem(MENU_DISABLE_SELECTED_GAMEOBJ,true)]
	[MenuItem(MENU_POSITION_ZERO_GAMEOBJ,true)]
	static bool ValidateSelectEnableODisable ()
	{
		GameObject[] gobj = GetSelectedGameObject () as GameObject[];
		if (gobj == null) {
			return false;
		} 
		if (gobj.Length == 0) {
			return false;
		}
		return true;
		
	}
	
	[MenuItem(MENU_DISABLE_SELECTED_GAMEOBJ_RECURSION)]
	static void SelectEnableODisableRecursion ()
	{
		GameObject[] gobj = GetSelectedGameObject () as GameObject[];
		bool enable = !gobj [0].active;
		foreach (GameObject go in gobj) {
			EnableODisableChildNote (go.transform, enable);
		}
	}

	[MenuItem(MENU_DISABLE_SELECTED_GAMEOBJ)]
	static void SelectEnableODisable ()
	{
		GameObject[] gobj = GetSelectedGameObject () as GameObject[];
		bool enable = !gobj [0].active;
		foreach (GameObject go in gobj) {
			go.SetActive(!go.activeInHierarchy);
		}
	}

	//激活或者关闭选中的物体及其子物体
	public static void EnableODisableChildNote (Transform parent, bool enable)
	{
		parent.gameObject.active = enable;
		for (int i = 0; i < parent.childCount; i++) {
			Transform child = parent.GetChild (i);
			if (child.childCount != 0) {
				EnableODisableChildNote (child, enable);
			} else {
				child.gameObject.active = enable;
			}
		}
	}
	
	[MenuItem(MENU_POSITION_ZERO_GAMEOBJ)]
	static void SelectPositionZero ()
	{
		GameObject[] gobj = GetSelectedGameObject () as GameObject[];
		foreach (GameObject go in gobj) {
			go.transform.position = Vector3.zero;
		}
	}

	// 返回选中的物体
	static GameObject[] GetSelectedGameObject ()
	{
		return Selection.gameObjects;
	}
}