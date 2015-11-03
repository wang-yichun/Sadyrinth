using UnityEngine;
using System.Collections;

public class StatisticsInfo
{
	public StatisticsInfo ()
	{
	}

	public StatisticsInfo (string stage_id, int sady_gotten, float time_used, float fuel_remain)
	{
		SadyGotten = sady_gotten;
		StageId = stage_id;
		TimeUsed = time_used;
		FuelRemain = fuel_remain;
	}

	//	public enum StatisticsInfoMode
	//	{
	//		pause,
	//		win,
	//		lose
	//	}

	//	public StatisticsInfoMode mode;

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
		MechanismController mechanism;
		if (GameController.GetInstance ().StageRoot.name == StageId) {
			mechanism = GameController.GetInstance ().StageRoot.GetComponentInChildren<MechanismController> ();

		} else {
			Transform stagePrefab = Resources.Load<Transform> (string.Format ("stage/{0}", StageId));
			mechanism = stagePrefab.GetComponentInChildren<MechanismController> ();
		}

		StageIdScore = mechanism.BaseScore;

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