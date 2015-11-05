using UnityEngine;
using System.Collections;
using GameDataEditor;

public class StatisticsInfo
{
	public StatisticsInfo ()
	{
	}

	public StatisticsInfo (string stage_id, StatisticsInfoMode mode, int sady_gotten, float time_used, float fuel_remain)
	{
		StageId = stage_id;
		Mode = mode;
		SadyGotten = sady_gotten;
		TimeUsed = time_used;
		FuelRemain = fuel_remain;
	}

	public enum StatisticsInfoMode
	{
		pause,
		win,
		lose
	}

	public StatisticsInfoMode Mode;

	public string StageId;
	public int SadyGotten;
	public float TimeUsed;
	public float FuelRemain;

	public int StageIdScore;
	public int SadyGottenScore;
	public int TimeUsedScore;
	public int FuelRemainScore;

	public int TotalScore;

	public void CalcScore ()
	{
		#region 无用的部分
		/*
		 *  没有使用基础分数去计算成绩
		 */
		GDEStageData stageData = DataController.GetInstance ().GetStageData (StageId);
		StageIdScore = stageData.base_score;
		#endregion

		SadyGottenScore = CalcSadyGottenScore (SadyGotten);

		TimeUsedScore = CalcTimeUsedScore (TimeUsed);

		FuelRemainScore = CalcFuelRemainScore (FuelRemain);

		TotalScore = SadyGottenScore + TimeUsedScore + FuelRemainScore;

		if (TotalScore < 0) {
			TotalScore = 0;
		}
	}

	public static int CalcSadyGottenScore (int SadyGotten)
	{
		return SadyGotten * 10000;
	}

	public static int CalcTimeUsedScore (float TimeUsed)
	{
		return (int)(-TimeUsed * 5);
	}

	public static int CalcFuelRemainScore (float FuelRemain)
	{
		return (int)(FuelRemain * 10f);
	}
}