using UnityEngine;
using UnityEngine.UI;

using System.Collections;

public class StageSelectItemController : MonoBehaviour
{

	public StageSelectItem Data;

	public tk2dTextMesh StageID;
	public tk2dTextMesh ScoreLabel;
	public tk2dTextMesh ScoreValue;
	public Text RemainFuel;

	public Button SelfButton;
	public Button ClearButton;

	public void Refresh (StageSelectItem item)
	{
		this.Data = item;

		if (this.Data.stage_lock) {
			StageID.text = Data.stage_id;
			ScoreLabel.text = "Locked";
			ScoreValue.text = "--";
			SelfButton.interactable = false;
			RemainFuel.text = "";
		} else {
			ScoreLabel.text = "Score";
			StageID.text = Data.stage_id;
			ScoreValue.text = string.Format ("{0}", Data.score);
			RemainFuel.text = string.Format ("Remain Fuel:{0:F2}", Data.remain_fuel);
			SelfButton.interactable = true;
		}

		if (DataController.GetInstance ().Common.show_remain_fuel) {
			RemainFuel.enabled = true;
		} else {
			RemainFuel.enabled = false;
		}

		ClearButton.gameObject.SetActive (false);

		if (DataController.GetInstance ().Common.long_tap_stage_record_clear) {
			GetComponent<EasyTouchTrigger> ().enabled = true;
		} else {
			GetComponent<EasyTouchTrigger> ().enabled = false;
		}
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
		(GameController.GetInstance ().UIController.StageSelect as UIStageSelectController).IgnoreCloseMenu++;
	}

	public void ClearButton_OnClicked ()
	{
		Data.score = 0;
		GameDataEditor.GDEStageData stageData = DataController.GetInstance ().GetStageData (Data.stage_id);
		stageData.high_score = 0;
		stageData.remain_fuel = 0;
		Refresh (Data);
	}
}

[System.Serializable]
public class StageSelectItem
{
	public StageSelectItem ()
	{
		detail_opened = false;
	}

	public string stage_id;
	public bool stage_lock;
	public int score;
	public float remain_fuel;

	public bool detail_opened;
}