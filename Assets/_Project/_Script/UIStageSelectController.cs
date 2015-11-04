using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class UIStageSelectController : RootCanvasBase
{
	public Transform ContentTransform;
	public GameObject StageSelectItemPrefab;

	public Dictionary<string, StageSelectItemController> StageSelectItemDic;

	public string StageIDSelected;

	public Button StartButton;

	public override void CanvasInEnd ()
	{
		base.CanvasInEnd ();

		PrepareStageInfoListByWorldID (0); // 暂时只有一个世界的

		RefreshStageInfo ();
//		StageIDSelected = DataHandler.LoadAutoSelectStageID ();
		StageIDSelected = DataController.GetInstance ().Common.auto_selected_stage_id;
		SelectStage (StageIDSelected);

		NotificationCenter.DefaultCenter.AddObserver (this, "stage_button_click");

//		EasyTouch.On_TouchStart += ET_CloseStageButtonDetailMenu;
	}

	public override void CanvasOutStart ()
	{
		base.CanvasOutStart ();
		NotificationCenter.DefaultCenter.RemoveObserver (this, "stage_button_click");

//		EasyTouch.On_TouchStart -= ET_CloseStageButtonDetailMenu;
	}

	void stage_button_click (NotificationCenter.Notification notification)
	{
		SelectStage ((string)notification.data ["stage_id"]);
	}

	public List<StageSelectItem> StageSelectItemList;

	void PrepareStageInfoListByWorldID (int world_id)
	{
		StageSelectItemList = new List<StageSelectItem> ();


		for (int i = 0; i < DataController.GetInstance ().WorldStageCount [world_id]; i++) {

//			string stage_id = string.Format ("{0:00}-{1:00}", world_id + 1, i + 1);
			string stage_id = WorldStage.CreateWithWorldIdxAndStageIdx (world_id, i).ToString ();

			StageSelectItemList.Add (new StageSelectItem () {
				stage_id = stage_id,
//				score = DataHandler.LoadScoreByStageID (stage_id)
				score = DataController.GetInstance ().GetStageData (stage_id).high_score
			});
		}
	}

	void ClearAllStageInfoItem ()
	{
		for (int i = 0; i < ContentTransform.childCount; i++) {
			Transform itemTransform = ContentTransform.GetChild (i);
			Destroy (itemTransform.gameObject);
		}
	}

	void RefreshStageInfo ()
	{
		ClearAllStageInfoItem ();

		StageSelectItemDic = new Dictionary<string, StageSelectItemController> ();

		foreach (StageSelectItem item in StageSelectItemList) {
			StageSelectItemController stageSelectItemController = Instantiate (StageSelectItemPrefab).GetComponent<StageSelectItemController> ();
			stageSelectItemController.transform.SetParent (ContentTransform);
			stageSelectItemController.transform.localScale = Vector3.one;
			stageSelectItemController.Refresh (item);

			StageSelectItemDic.Add (item.stage_id, stageSelectItemController);
		}
	}

	void SelectStage (string stage_id)
	{
		StageIDSelected = stage_id;

		if (StageIDSelected == null) {
			StartButton.interactable = false;
			StartButton.GetComponentInChildren<tk2dTextMesh> ().text = "Select A Stage";
		} else {
			StartButton.interactable = true;
			StartButton.GetComponentInChildren<tk2dTextMesh> ().text = string.Format ("Start: {0}", stage_id);
		}

//		DataHandler.SaveAutoSelectStageID (stage_id);

		DataController.GetInstance ().Common.auto_selected_stage_id = stage_id;
	}

	void ET_CloseStageButtonDetailMenu (Gesture gesture)
	{
		Debug.Log ("ETT_CloseStageButtonDetailMenu");

		var stageSelectItemControllerList = StageSelectItemDic.Where (kvp => {
			return kvp.Value.Data.detail_opened == true;
		}).Select (kvp => kvp.Value).ToList ();

		stageSelectItemControllerList.ForEach (_ => {
			_.CloseDetailMenu ();
		});

	}
}

