using UnityEngine;
using System.Collections;
using GameDataEditor;

public class GameController : MonoBehaviour
{
	// 手动绑定
	public MainCameraController MainCamera;
	public ControlPadController Pad;
	public UIController UIController;

	// 动态绑定
	public Transform StageRoot;
	public PlayerController Player;
	public Transform CollectableContainerTransform;

	public int MaxSadyCount;
	public int RemainSadyCount;

	private static GameController MyGameController;

	public static GameController GetInstance ()
	{
		return MyGameController;
	}

	#region EasyTouch Subscribe

	public void SubscribeEasyTouchEvents ()
	{
		EasyTouch.On_SwipeStart += EasyTouch_On_SwipeStart;
		EasyTouch.On_Swipe += EasyTouch_On_Swipe;
		EasyTouch.On_SwipeEnd += EasyTouch_On_SwipeEnd;

		EasyTouch.On_TouchStart += EasyTouch_On_TouchStart;
		EasyTouch.On_TouchUp += EasyTouch_On_TouchUp;
	}

	public void UnsubscribeEasyTouchEvents ()
	{
		EasyTouch.On_SwipeStart -= EasyTouch_On_SwipeStart;
		EasyTouch.On_Swipe -= EasyTouch_On_Swipe;
		EasyTouch.On_SwipeEnd -= EasyTouch_On_SwipeEnd;

		EasyTouch.On_TouchStart -= EasyTouch_On_TouchStart;
		EasyTouch.On_TouchUp -= EasyTouch_On_TouchUp;
	}

	void EasyTouch_On_TouchStart (Gesture gesture)
	{
		if (IsPause == false) {
			Pad.Open ();
			Pad.SetPadPosition (gesture.GetTouchToWorldPoint (0f));
		}
	}

	void EasyTouch_On_TouchUp (Gesture gesture)
	{
		Pad.Close ();
	}

	#endregion

	#region EasyTouch Control


	void EasyTouch_On_SwipeStart (Gesture gesture)
	{
		OnSwipeStartPosition = gesture.startPosition;
		Pad.Open ();
		Pad.SetPadPosition (gesture.GetTouchToWorldPoint (0f), true);
	}

	Vector2 OnSwipeStartPosition;

	void EasyTouch_On_Swipe (Gesture gesture)
	{
		var inputVec = gesture.position - OnSwipeStartPosition;
		Player.InputVector (inputVec);
	}

	void EasyTouch_On_SwipeEnd (Gesture gesture)
	{
		Pad.Close ();
		Player.InputVector (Vector2.zero);
	}

	#endregion

	void InitUIData ()
	{
		// 燃料
		NotificationCenter.DefaultCenter.PostNotification (this, "set_fuel_to",
			new Hashtable () { 
				{ "value", 0f }
			}
		);

		// Sady
		MaxSadyCount = CollectableContainerTransform.childCount;
		RemainSadyCount = MaxSadyCount;

		NotificationCenter.DefaultCenter.PostNotification (this, "set_sady_to",
			new Hashtable () { 
				{ "value", GameController.GetInstance ().MaxSadyCount - GameController.GetInstance ().RemainSadyCount },
				{ "total_value", GameController.GetInstance ().MaxSadyCount }
			}
		);
	}

	void Awake ()
	{
		MyGameController = this;
	}

	void Start ()
	{
	}

	void OnDisable ()
	{
		UnsubscribeEasyTouchEvents ();
	}

	void OnDestroy ()
	{
		UnsubscribeEasyTouchEvents ();
	}

	#region 游戏总控

	public string LastStartStageID;
	public bool IsPause;
	public float TimePassed;

	void FixedUpdate ()
	{
		TimePassed += Time.fixedDeltaTime;
	}

	public void ResetGame (string stage_id)
	{
		if (stage_id == null) {
			stage_id = LastStartStageID;
		}
			
		LoadStage (stage_id);
		LastStartStageID = stage_id;

		NotificationCenter.DefaultCenter.PostNotification (this, "set_stage_id_to", new Hashtable () {
			{ "value", stage_id }
		});

		InitUIData ();
		Player.InitPlayer ();

		TimePassed = 0f;
		PauseGame ();
	}

	public void StartGame ()
	{
		SubscribeEasyTouchEvents ();
		AddObserver_Player ();
		ResumeGame ();

		NotificationCenter.DefaultCenter.PostNotification (this, "start_game_reset");

		UIController.InGame.SetAllButtonInteractable (true);
	}

	public void EndGame ()
	{
		PauseGame ();
		UnsubscribeEasyTouchEvents ();
		RemoveObserver_Player ();
		UnloadStage ();
	}

	public void PauseGame ()
	{
		Time.timeScale = 0f;
		IsPause = true;
		Pad.Close ();
	}

	public void ResumeGame ()
	{
		Time.timeScale = 1;
		IsPause = false;
	}

	#endregion

	#region 加载/卸载 关卡

	void UnloadStage ()
	{
		if (StageRoot != null) {
			DestroyImmediate (StageRoot.gameObject);
			StageRoot = null;
		}
	}

	void LoadStage (string stage_id)
	{
		UnloadStage ();

		GameObject stageDebugContainer = GameObject.Find ("StageDebugContainer");
		if (stageDebugContainer != null) {
			if (stageDebugContainer.transform.childCount == 1) {
				StageRoot = stageDebugContainer.transform.GetChild (0);
			}
		}

		if (StageRoot == null) {
			Transform stagePrefab = Resources.Load<Transform> (string.Format ("stage/{0}", stage_id));
			StageRoot = Instantiate (stagePrefab);
		}

		Player = StageRoot.Find ("Player").GetComponent<PlayerController> ();
		CollectableContainerTransform = StageRoot.Find ("CollectableContainer");

		MainCamera.Player = Player;

//		public Transform StageRoot;
//		public PlayerController Player;
//		public Transform CollectableContainerTransform;
	}

	#endregion

	#region 降落区检测 / 油料耗尽检测

	void AddObserver_Player ()
	{
		NotificationCenter.DefaultCenter.AddObserver (this, "player_landing_success");
		NotificationCenter.DefaultCenter.AddObserver (this, "player_no_fuel");
	}

	void RemoveObserver_Player ()
	{
		NotificationCenter.DefaultCenter.RemoveObserver (this, "player_landing_success");
		NotificationCenter.DefaultCenter.RemoveObserver (this, "player_no_fuel");
	}

	void player_landing_success (NotificationCenter.Notification notification)
	{
		if (this.IsPause == false) {
			if (this.RemainSadyCount == 0) {
				PauseGame ();
				UIController.Win.Open ();

				UIController.InGame.SetAllButtonInteractable (false);

				UIWinController winController = UIController.Win as UIWinController;

				winController.StatisticsLike.SetData (new StatisticsInfo (
					stage_id: this.LastStartStageID,
					mode : StatisticsInfo.StatisticsInfoMode.win,
					sady_gotten: this.MaxSadyCount - this.RemainSadyCount,
					time_used: this.TimePassed,
					fuel_remain: this.Player.CurFuelValue
				));

				int current_score = winController.StatisticsLike.Data.TotalScore;

				GDEStageData stageData = DataController.GetInstance ().GetStageData (this.LastStartStageID);
				if (current_score > stageData.high_score) {
					stageData.high_score = current_score;
					stageData.remain_fuel = winController.StatisticsLike.Data.FuelRemain;
				}

				string nextStageId = DataController.GetInstance ().DefaultNextHelper.GetNextStageId (this.LastStartStageID);
				if (nextStageId != this.LastStartStageID) {
					GDEStageData nextStageData = DataController.GetInstance ().GetStageData (nextStageId);
					nextStageData.stage_lock = false;

					DataController.GetInstance ().Common.auto_selected_stage_id = nextStageId;
				}
			}
		}
	}

	void player_no_fuel (NotificationCenter.Notification notification)
	{
		Debug.Log ("player_no_fuel");
		if (this.IsPause == false) {
			PauseGame ();
			UIController.Lose.Open ();

			UIController.InGame.SetAllButtonInteractable (false);

			UILoseController loseController = UIController.Lose as UILoseController;

			loseController.StatisticsLike.SetData (new StatisticsInfo (
				stage_id: this.LastStartStageID,
				mode : StatisticsInfo.StatisticsInfoMode.lose,
				sady_gotten: this.MaxSadyCount - this.RemainSadyCount,
				time_used: this.TimePassed,
				fuel_remain: this.Player.CurFuelValue
			));
		}
	}

	#endregion
}
