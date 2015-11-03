using UnityEngine;
using System.Collections;

[System.Serializable]
public class PauseStatisticsLike
{

	public tk2dTextMesh StatisticsLabel;
	public tk2dTextMesh StatisticsValue;
	public tk2dTextMesh StatisticsScore;

	public StatisticsInfo Data;

	public void SetData (StatisticsInfo data)
	{
		Data = data;
		Data.CalcScore ();

		RefreshStatistics ();
	}

	public void RefreshStatistics ()
	{
		RefreshStatistics_Value ();
		RefreshStatistics_Score ();	
	}

	public void RefreshStatistics_Value ()
	{
		string pattern = "{0:0}\n{1:00:00}\n{2:0}";
		StatisticsValue.text = string.Format (pattern, Data.SadyGotten, Data.TimeUsed, Data.FuelRemain);
	}

	public void RefreshStatistics_Score ()
	{
		string pattern = "{0}\n{1}\n+{2}\n{3}";
		StatisticsScore.text = string.Format (pattern, Data.SadyGottenScore, Data.TimeUsedScore, Data.FuelRemainScore, Data.TotalScore);
	}
}
