using UnityEngine;
using System.Collections;
using GameDataEditor;

public class UIController : MonoBehaviour
{
	public RootCanvasBase MainMenu;
	public RootCanvasBase StageSelect;
	public RootCanvasBase InGame;
	public RootCanvasBase PauseMenu;
	public RootCanvasBase Win;
	public RootCanvasBase Lose;
	public RootCanvasBase Setting;
	public CanvasOKCancelBase SecretInput;

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
		Lose.gameObject.SetActive (false);
		Setting.gameObject.SetActive (false);

		SecretInput.gameObject.SetActive (false);
	}

	#region MainMenu

	public void MainMenu_ExitButton_OnClick ()
	{
		Application.Quit ();
	}

	public void MainMenu_SettingButton_OnClick ()
	{
		GameController.GetInstance ().PlaySound (GameController.GetInstance ().TapAudio);

		MainMenu.Close ();
		Setting.Open ();
	}

	public void MainMenu_PlayButton_OnClick ()
	{
		GameController.GetInstance ().PlaySound (GameController.GetInstance ().TapAudio);

		MainMenu.Close ();
		StageSelect.Open ();
	}

	#endregion

	#region StageSelect

	public void StageSelect_BackButton_OnClick ()
	{
		GameController.GetInstance ().PlaySound (GameController.GetInstance ().TapAudio);

		StageSelect.Close ();
		MainMenu.Open ();
	}

	public void StageSelect_StartButton_OnClick ()
	{
		GameController.GetInstance ().PlaySound (GameController.GetInstance ().TapAudio);

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
		GameController.GetInstance ().PlaySound (GameController.GetInstance ().TapAudio);

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
				mode : StatisticsInfo.StatisticsInfoMode.pause,
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
		GameController.GetInstance ().PlaySound (GameController.GetInstance ().TapAudio);

		PauseMenu.Close ();

		GameController game = GameController.GetInstance ();
		game.ResumeGame ();
	}

	public void PauseMenu_MainMenuButton_OnClick ()
	{
		GameController.GetInstance ().PlaySound (GameController.GetInstance ().TapAudio);

		PauseMenu.Close ();

		GameController game = GameController.GetInstance ();
		game.EndGame ();
		InGame.Close ();
		StageSelect.Open ();
	}

	public void PauseMenu_ResetButton_OnClick ()
	{
		GameController.GetInstance ().PlaySound (GameController.GetInstance ().TapAudio);

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
		GameController.GetInstance ().PlaySound (GameController.GetInstance ().TapAudio);

		GameController game = GameController.GetInstance ();
		game.EndGame ();

		Win.Close ();
		InGame.Close ();
		StageSelect.Open ();
	}

	public void Win_ReplayButton_OnClick ()
	{
		GameController.GetInstance ().PlaySound (GameController.GetInstance ().TapAudio);

		Win.Close ();

		GameController game = GameController.GetInstance ();
		game.EndGame ();
		game.ResetGame (null);
		game.StartGame ();
	}

	public void Win_NextStageButton_OnClick ()
	{
		GameController.GetInstance ().PlaySound (GameController.GetInstance ().TapAudio);

		Win.Close ();

		GameController game = GameController.GetInstance ();
		string nextStageID = DataController.GetInstance ().DefaultNextHelper.GetNextStageId (game.LastStartStageID);

		if (nextStageID == game.LastStartStageID) {
			game.EndGame ();

			InGame.Close ();
			StageSelect.Open ();
		} else {

			game.EndGame ();
			game.ResetGame (nextStageID);
			game.StartGame ();
		}
	}

	#endregion

	#region Lose

	public void Lose_MainMenuButton_OnClick ()
	{
		GameController.GetInstance ().PlaySound (GameController.GetInstance ().TapAudio);

		GameController game = GameController.GetInstance ();
		game.EndGame ();

		Lose.Close ();
		InGame.Close ();
		StageSelect.Open ();
	}

	public void Lose_TryAgainButton_OnClick ()
	{
		GameController.GetInstance ().PlaySound (GameController.GetInstance ().TapAudio);

		Lose.Close ();

		GameController game = GameController.GetInstance ();
		game.EndGame ();
		game.ResetGame (null);
		game.StartGame ();
	}

	#endregion

	#region setting

	public void Setting_MusicButton_OnClick ()
	{
		GameController.GetInstance ().PlaySound (GameController.GetInstance ().TapAudio);

		DataController.GetInstance ().Common.music = !DataController.GetInstance ().Common.music;
		(Setting as UISettingController).Refresh ();

		if (DataController.GetInstance ().Common.music == false) {
			GameController.GetInstance ().PlayMusic (null);
		} else {
			GameController.GetInstance ().PlayMusic (GameController.GetInstance ().MainThemeMusic);
		}
	}

	public void Setting_SoundButton_OnClick ()
	{
		DataController.GetInstance ().Common.sound = !DataController.GetInstance ().Common.sound;
		(Setting as UISettingController).Refresh ();

		GameController.GetInstance ().PlaySound (GameController.GetInstance ().TapAudio);
	}

	public void Setting_ClearButton_OnClick ()
	{
		GameController.GetInstance ().PlaySound (GameController.GetInstance ().TapAudio);

		SecretInput.Open (new CanvasOKCancelInfo () {
			ContentInfo = "This function is designed for desigeners.\nSo we need a secret code to continue.",
			CorrectSecretString = "ethan",
			On_OK = result => {
				ClearRecord ();
			},
			On_Cancel = () => {
			},
			parentCanvas = Setting
		});
	}

	public void ClearRecord ()
	{
		
		GDECommonData commonData = DataController.GetInstance ().Common;

		for (int world_id = 1; world_id <= commonData.world_count; world_id++) {
			int worldStageCount = DataController.GetInstance ().WorldStageCount [world_id - 1];
			for (int stage_id = 1; stage_id <= worldStageCount; stage_id++) {
				WorldStage sw = WorldStage.CreateWithWorldIdAndStageId (world_id, stage_id);

				GameDataEditor.GDEStageData stageData = DataController.GetInstance ().GetStageData (sw.ToString ());
				stageData.high_score = 0;
				stageData.remain_fuel = 0;

				if (world_id == 1 && stage_id == 1) {
					stageData.stage_lock = false;
				} else {
					stageData.stage_lock = true;
				}
			}
		}
	}

	public void Setting_UnlockButton_OnClick ()
	{
		GameController.GetInstance ().PlaySound (GameController.GetInstance ().TapAudio);

		SecretInput.Open (new CanvasOKCancelInfo () {
			ContentInfo = "This function is designed for desigeners.\nSo we need a secret code to continue.",
			CorrectSecretString = "ethan",
			On_OK = result => {
				UnlockStage ();
			},
			On_Cancel = () => {
			},
			parentCanvas = Setting	
		});
	}

	public void UnlockStage ()
	{
		GDECommonData commonData = DataController.GetInstance ().Common;

		for (int world_id = 1; world_id <= commonData.world_count; world_id++) {
			int worldStageCount = DataController.GetInstance ().WorldStageCount [world_id - 1];
			for (int stage_id = 1; stage_id <= worldStageCount; stage_id++) {
				WorldStage sw = WorldStage.CreateWithWorldIdAndStageId (world_id, stage_id);

				GDEStageData stageData = DataController.GetInstance ().GetStageData (sw.ToString ());

				stageData.stage_lock = false;

			}
		}
	}

	public void Setting_MainMenuButton_OnClick ()
	{
		GameController.GetInstance ().PlaySound (GameController.GetInstance ().TapAudio);

		Setting.Close ();
		MainMenu.Open ();
	}

	public void Setting_EnterCodeButton_OnClick ()
	{
		GameController.GetInstance ().PlaySound (GameController.GetInstance ().TapAudio);

		SecretInput.Open (new CanvasOKCancelInfo () {
			ContentInfo = "This function is designed for desigeners.\nSo we need a secret code to continue.",
			CorrectSecretString = null,
			On_OK = result => {
				HandleSecretCode (result);
			},
			On_Cancel = () => {
			},
			parentCanvas = Setting	
		});
	}

	public void HandleSecretCode (string secretCode)
	{
		switch (secretCode) {
		case "clearrecord":
			ClearRecord ();
			break;
		case "unlockstage":
			UnlockStage ();
			break;
		case "showremainfuel=true":
			DataController.GetInstance ().Common.show_remain_fuel = true;
			break;
		case "showremainfuel=false":
			DataController.GetInstance ().Common.show_remain_fuel = false;
			break;
		case "longtapstagerecordclear=true":
			DataController.GetInstance ().Common.long_tap_stage_record_clear = true;
			break;
		case "longtapstagerecordclear=false":
			DataController.GetInstance ().Common.long_tap_stage_record_clear = false;
			break;
		default:
			break;
		}
	}

	#endregion
}
