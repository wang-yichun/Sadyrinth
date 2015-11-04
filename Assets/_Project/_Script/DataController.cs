using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using GameDataEditor;

public class DataController : MonoBehaviour
{

	public TextAsset DataAsset;

	public GDECommonData Common;
	public Dictionary<string, GDEStageData> StageDic;
	public Dictionary<int, int> WorldStageCount;

	public static DataController MyDataController;

	public static DataController GetInstance ()
	{
		return MyDataController;
	}

	// Use this for initialization
	void Start ()
	{
		GDEDataManager.Init (DataAsset);

		GDEDataManager.DataDictionary.TryGetCustom (GDEItemKeys.Common_common, out Common);

		Debug.Log (Common.version);

		StageDic = new Dictionary<string, GDEStageData> ();
		WorldStageCount = new Dictionary<int, int> ();

		foreach (GDEStageData stage in Common.stage) {
			StageDic.Add (stage.Key, stage);

			int world = WorldStage.CreateWithGDEStageKey (stage.Key).World;
			if (WorldStageCount.ContainsKey (world)) {
				WorldStageCount [world]++;
			} else {
				WorldStageCount.Add (world, 1);
			}
		}
	}

	public GDEStageData GetStageData (string stage_id)
	{
		return StageDic [StageIDToStageDataKey (stage_id)];
	}

	public static string StageIDToStageDataKey (string stage_id)
	{
		string[] s = stage_id.Split ('-');
		return string.Format ("stage_{0}_{1}", s [0], s [1]);
	}
}

public struct WorldStage
{
	public int World;
	public int Stage;

	enum Type
	{
		stage_id,
		gde_stage_key
	}

	public string ToString (Type type)
	{
		string result;
		switch (type) {
		case Type.stage_id:
			result = string.Format ("{0:0}-{1:0}", World, Stage);
			break;
		case Type.gde_stage_key:
			result = string.Format ("stage_{0:0}_{1:0}", World, Stage);
			break;
		default:
			break;
		}
		return result;
	}

	public override string ToString ()
	{
		return this.ToString (Type.stage_id);
	}

	public static WorldStage CreateWithStageId (string stage_id)
	{
		WorldStage ws = new WorldStage ();
		string[] s = stage_id.Split ("-");
		ws.World = int.Parse (s [0]);
		ws.Stage = int.Parse (s [1]);
		return ws;
	}

	public static WorldStage CreateWithGDEStageKey (string stage_key)
	{
		WorldStage ws = new WorldStage ();
		string[] s = stage_key.Split ('_');
		ws.World = int.Parse (s [1]);
		ws.Stage = int.Parse (s [2]);
		return ws;
	}

	public static WorldStage CreateWithWorldIdxAndStageIdx (int world_idx, int stage_idx)
	{
		return new WorldStage () {
			World = world_idx + 1,
			Stage = stage_idx + 1
		};
	}
}
