using UnityEngine;
using System.Collections;

public class GameController : MonoBehaviour
{
	// 手动绑定
	public MainCameraController MainCamera;
	public ControlPadController Pad;

	// 动态绑定
	public Transform StageRoot;
	public PlayerController Player;
	public Transform CollectableContainerTransform;

	public int MaxSadyCount;
	public int CurSadyCount;

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
		Pad.Open ();
		Pad.SetPadPosition (gesture.GetTouchToWorldPoint (0f));
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
		CurSadyCount = MaxSadyCount;

		NotificationCenter.DefaultCenter.PostNotification (this, "set_sady_to",
			new Hashtable () { 
				{ "value", GameController.GetInstance ().CurSadyCount },
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

	string LastStartStageID;

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
	}

	public void StartGame ()
	{
		SubscribeEasyTouchEvents ();
	}

	public void EndGame ()
	{
		UnsubscribeEasyTouchEvents ();
	}

	#endregion

	void UnloadStage ()
	{
		if (StageRoot != null) {
			Destroy (StageRoot);
			StageRoot = null;
		}
	}

	void LoadStage (string stage_id)
	{
		UnloadStage ();

		Transform StageRoot = null;

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
}
