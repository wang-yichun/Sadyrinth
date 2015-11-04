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
	public int LastWorldIdx;

	public static DataController MyDataController;

	public static DataController GetInstance ()
	{
		return MyDataController;
	}

	void Awake ()
	{
		MyDataController = this;
		DefaultNextHelper = new NextHelper (this);
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

			int world = WorldStage.CreateWithGDEStageKey (stage.Key).WorldId;
			int world_id = world - 1;
			if (WorldStageCount.ContainsKey (world_id)) {
				WorldStageCount [world_id]++;
			} else {
				WorldStageCount.Add (world_id, 1);
			}

			if (LastWorldIdx < world_id) {
				LastWorldIdx = world_id;
			}
		}
	}

	public GDEStageData GetStageData (string stage_id)
	{
		var stage_gde_key = WorldStage.CreateWithStageId (stage_id).ToString (WorldStage.Type.gde_stage_key);
		return StageDic [stage_gde_key];
	}

	#region NextHelper

	public class NextHelper: INextHelper
	{

		private DataController OuterDataController;

		public NextHelper (DataController dataController)
		{
			OuterDataController = dataController;
		}

		#region INextHelper implementation

		public string GetNextStageId (string cur_stage_id)
		{
			WorldStage ws = WorldStage.CreateWithStageId (cur_stage_id);

			string result_stage_id;
			if (IsLastWorldLastStage (cur_stage_id)) {
				result_stage_id = cur_stage_id;	
			} else if (IsLastStageInTheWorld (cur_stage_id)) {
				ws.WorldId++;
				ws.StageId = 0;
				result_stage_id = ws.ToString ();
			} else {
				ws.StageId++;
				result_stage_id = ws.ToString ();
			}

			return result_stage_id;
		}

		public bool IsLastStageInTheWorld (string cur_stage_id)
		{
			return WorldStage.CreateWithStageId (cur_stage_id).WorldId == OuterDataController.LastWorldIdx;
		}

		public bool IsLastWorldLastStage (string cur_stage_id)
		{
			WorldStage ws = WorldStage.CreateWithStageId (cur_stage_id);
			return ws.WorldId == OuterDataController.LastWorldIdx + 1 && ws.StageId == OuterDataController.WorldStageCount [ws.WorldId - 1];
		}

		#endregion
		
	}

	public NextHelper DefaultNextHelper;

	#endregion
}