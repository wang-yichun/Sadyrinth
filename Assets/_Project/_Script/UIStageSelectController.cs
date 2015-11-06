using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using GameDataEditor;

using UnityEngine.Advertisements;

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

		GameController.GetInstance ().PlayMusic (GameController.GetInstance ().MainThemeMusic);

		PrepareStageInfoListByWorldID (0); // 暂时只有一个世界的

		RefreshStageInfo ();
		StageIDSelected = DataController.GetInstance ().Common.auto_selected_stage_id;
		SelectStage (StageIDSelected);

		NotificationCenter.DefaultCenter.AddObserver (this, "stage_button_click");

		EasyTouch.On_TouchUp += ET_CloseStageButtonDetailMenu;

		IgnoreCloseMenu = 0;

		ShowAd ();
	}

	public override void CanvasOutStart ()
	{
		base.CanvasOutStart ();
		NotificationCenter.DefaultCenter.RemoveObserver (this, "stage_button_click");

		EasyTouch.On_TouchUp -= ET_CloseStageButtonDetailMenu;
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
			
			string stage_id = WorldStage.CreateWithWorldIdxAndStageIdx (world_id, i).ToString ();

			GDEStageData stageData = DataController.GetInstance ().GetStageData (stage_id);

			StageSelectItemList.Add (new StageSelectItem () {
				stage_id = stage_id,
				stage_lock = stageData.stage_lock,
				score = stageData.high_score,
				remain_fuel = stageData.remain_fuel
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

		RectTransform rt = ContentTransform.GetComponent<RectTransform> ();
		rt.sizeDelta = new Vector2 (rt.sizeDelta.x, GetContentHeightByStageCount (StageSelectItemList.Count));
	}

	float GetContentHeightByStageCount (int count)
	{
		GridLayoutGroup glg = ContentTransform.GetComponent<GridLayoutGroup> ();

		int col_count = glg.constraintCount;
		int row_count = Mathf.CeilToInt (count / col_count);
		return row_count * glg.cellSize.y;
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

		Transform startButtonLabelTransform = StartButton.GetComponentInChildren<tk2dTextMesh> ().transform;

		startButtonLabelTransform.DOKill ();
		Sequence sequence = DOTween.Sequence ();
		sequence.Append (startButtonLabelTransform.DOScale (1.2f, .1f).SetEase (Ease.OutSine));
		sequence.Append (startButtonLabelTransform.DOScale (1f, .1f).SetEase (Ease.InSine));

		DataController.GetInstance ().Common.auto_selected_stage_id = stage_id;
	}

	public int IgnoreCloseMenu;

	void ET_CloseStageButtonDetailMenu (Gesture gesture)
	{
		if (IgnoreCloseMenu == 0) {
			var stageSelectItemControllerList = StageSelectItemDic.Where (kvp => {
				return kvp.Value.Data.detail_opened == true;
			}).Select (kvp => kvp.Value).ToList ();

			stageSelectItemControllerList.ForEach (_ => {
				_.CloseDetailMenu ();
			});
		} else {
			IgnoreCloseMenu--;
		}
	}

	#region Ad test

	public void ShowAd ()
	{

		#if UNITY_ANDROID || UNITY_IPHONE

		if (Advertisement.IsReady ()) {
			Advertisement.Show ();
		}

		#endif
	}



	#endregion
}

