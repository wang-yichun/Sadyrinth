using UnityEngine;
using System.Collections;

public class UIController : MonoBehaviour
{
	public RootCanvasBase MainMenu;
	public RootCanvasBase StageSelect;
	public RootCanvasBase InGame;
	public RootCanvasBase PauseMenu;
	public RootCanvasBase Win;


	public bool IsStageDebug;

	void Start ()
	{
		CloseAll ();

		#region StageDebug
		string debugStageId = null;
		GameObject stageDebugContainer = GameObject.Find ("StageDebugContainer");
		if (stageDebugContainer != null) {
			if (stageDebugContainer.transform.childCount == 1) {
				debugStageId = stageDebugContainer.transform.GetChild (0).name;
				IsStageDebug = true;
			}
		}
		#endregion

		if (IsStageDebug) {
			// 关卡测试流程
			InGame.Open ();
			GameController game = GameController.GetInstance ();
			game.ResetGame (debugStageId);
			game.StartGame ();
		} else {
			// 正常流程
			MainMenu.Open ();
		}
	}

	void CloseAll ()
	{
		MainMenu.gameObject.SetActive (false);
		StageSelect.gameObject.SetActive (false);
		InGame.gameObject.SetActive (false);
		PauseMenu.gameObject.SetActive (false);
		Win.gameObject.SetActive (false);
	}

	#region MainMenu

	public void MainMenu_ExitButton_OnClick ()
	{
		Application.Quit ();
	}

	public void MainMenu_PlayButton_OnClick ()
	{
		MainMenu.Close ();
		StageSelect.Open ();
	}

	#endregion

	#region StageSelect

	public void StageSelect_BackButton_OnClick ()
	{
		StageSelect.Close ();
		MainMenu.Open ();
	}

	public void StageSelect_StartButton_OnClick ()
	{
		string StageIDSelected = GetComponentInChildren<UIStageSelectController> ().StageIDSelected;

		StageSelect.Close ();
		InGame.Open ();

		GameController.GetInstance ().ResetGame (StageIDSelected);
		GameController.GetInstance ().StartGame ();
	}

	#endregion

	#region InGame

	public void InGame_PauseButton_OnClick ()
	{
		GameController game = GameController.GetInstance ();

		if (game.IsPause) {
			game.ResumeGame ();
			PauseMenu.Close ();
		} else {
			game.PauseGame ();
			PauseMenu.Open ();

			UIPauseMenuController pauseMenuController = PauseMenu as UIPauseMenuController;
			pauseMenuController.StatisticsLike.SetData (new StatisticsInfo (
				stage_id: game.LastStartStageID,
				sady_gotten: game.MaxSadyCount - game.RemainSadyCount,
				time_used: game.TimePassed,
				fuel_remain: game.Player.CurFuelValue
			));
		}
	}

	#endregion

	#region PauseMenu

	public void PauseMenu_ResumeButton_OnClick ()
	{
		PauseMenu.Close ();

		GameController game = GameController.GetInstance ();
		game.ResumeGame ();
	}

	public void PauseMenu_MainMenuButton_OnClick ()
	{
		PauseMenu.Close ();

		GameController game = GameController.GetInstance ();
		game.EndGame ();
		InGame.Close ();
		StageSelect.Open ();
	}

	public void PauseMenu_ResetButton_OnClick ()
	{
		PauseMenu.Close ();

		GameController game = GameController.GetInstance ();
		game.EndGame ();
		game.ResetGame (null);
		game.StartGame ();
	}

	#endregion

	#region Win

	public void Win_MainMenuButton_OnClick ()
	{
		GameController game = GameController.GetInstance ();
		game.EndGame ();

		Win.Close ();
		InGame.Close ();
		StageSelect.Open ();
	}

	public void Win_ReplayButton_OnClick ()
	{
		Win.Close ();

		GameController game = GameController.GetInstance ();
		game.EndGame ();
		game.ResetGame (null);
		game.StartGame ();
	}

	public void Win_NextStageButton_OnClick ()
	{
		Win.Close ();

		GameController game = GameController.GetInstance ();
		string nextStageID = GameHelper.GetNextStageId (game.LastStartStageID);
		game.EndGame ();
		game.ResetGame (nextStageID);
		game.StartGame ();
	}

	#endregion
}
