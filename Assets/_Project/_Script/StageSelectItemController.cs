using UnityEngine;
using System.Collections;

public class StageSelectItemController : MonoBehaviour
{

	public StageSelectItem Data;

	public tk2dTextMesh StageID;
	public tk2dTextMesh ScoreValue;

	public void Refresh (StageSelectItem item)
	{
		this.Data = item;
		StageID.text = Data.stage_id;
		ScoreValue.text = string.Format ("{0}", Data.score);
	}

	public void click ()
	{
		NotificationCenter.DefaultCenter.PostNotification (this, "stage_button_click", new Hashtable () {
			{ "stage_id", Data.stage_id }
		});
	}
}

[System.Serializable]
public class StageSelectItem
{
	public string stage_id;
	public int score;
}