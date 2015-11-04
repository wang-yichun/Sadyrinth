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

	void Awake ()
	{
		MyDataController = this;
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
			int world_id = world - 1;
			if (WorldStageCount.ContainsKey (world_id)) {
				WorldStageCount [world_id]++;
			} else {
				WorldStageCount.Add (world_id, 1);
			}
		}
	}

	public GDEStageData GetStageData (string stage_id)
	{
		var stage_gde_key = WorldStage.CreateWithStageId (stage_id).ToString (WorldStage.Type.gde_stage_key);
		return StageDic [stage_gde_key];
	}
}