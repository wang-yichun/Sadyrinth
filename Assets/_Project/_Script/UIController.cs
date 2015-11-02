using UnityEngine;
using System.Collections;

public class UIController : MonoBehaviour
{
	public RootCanvasBase MainMenu;
	public RootCanvasBase StageSelect;
	public RootCanvasBase InGame;
	public RootCanvasBase PauseMenu;

	void Start ()
	{
		CloseAll ();
		MainMenu.Open ();
	}

	void CloseAll ()
	{
		MainMenu.gameObject.SetActive (false);
		StageSelect.gameObject.SetActive (false);
		InGame.gameObject.SetActive (false);
		PauseMenu.gameObject.SetActive (false);
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
		if (game.isPause) {
			game.ResumeGame ();
			PauseMenu.Close ();
		} else {
			game.PauseGame ();
			PauseMenu.Open ();
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
}
