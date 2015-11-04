﻿using UnityEngine;
using UnityEngine.UI;

using System.Collections;

public class StageSelectItemController : MonoBehaviour
{

	public StageSelectItem Data;

	public tk2dTextMesh StageID;
	public tk2dTextMesh ScoreValue;

	public Button ClearButton;

	public void Refresh (StageSelectItem item)
	{
		this.Data = item;
		StageID.text = Data.stage_id;
		ScoreValue.text = string.Format ("{0}", Data.score);


		ClearButton.gameObject.SetActive (false);
	}

	public void OpenDetailMenu ()
	{
		ClearButton.gameObject.SetActive (true);
		Data.detail_opened = true;
	}

	public void CloseDetailMenu ()
	{
		ClearButton.gameObject.SetActive (false);
		Data.detail_opened = false;
	}


	public void SelfButton_OnClicked ()
	{
		NotificationCenter.DefaultCenter.PostNotification (this, "stage_button_click", new Hashtable () {
			{ "stage_id", Data.stage_id }
		});
	}

	public void ETT_OpenStageButtonDetailMenu ()
	{
		OpenDetailMenu ();
	}

	public void ClearButton_OnClicked ()
	{
		Data.score = 0;
		GameDataEditor.GDEStageData stageData = DataController.GetInstance ().GetStageData (Data.stage_id);
		stageData.high_score = 0;

		Refresh (Data);
	}
}

[System.Serializable]
public class StageSelectItem
{
	public StageSelectItem() {
		detail_opened = false;
	}

	public string stage_id;
	public int score;

	public bool detail_opened;
}