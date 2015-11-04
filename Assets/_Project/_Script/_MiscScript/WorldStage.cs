using UnityEngine;
using System.Collections;

public class WorldStage
{
	public int WorldId;
	public int StageId;

	private WorldStage ()
	{
	}

	public enum Type
	{
		stage_id,
		gde_stage_key
	}

	public string ToString (Type type)
	{
		string result = null;
		switch (type) {
		case Type.stage_id:
			result = string.Format ("{0:00}-{1:00}", WorldId, StageId);
			break;
		case Type.gde_stage_key:
			result = string.Format ("stage_{0:00}_{1:00}", WorldId, StageId);
			break;
		default:
			break;
		}
		return result;
	}

	public override string ToString ()
	{
		return this.ToString (Type.stage_id);
	}

	public static WorldStage CreateWithStageId (string stage_id)
	{
		WorldStage ws = new WorldStage ();
		string[] s = stage_id.Split ('-');
		ws.WorldId = int.Parse (s [0]);
		ws.StageId = int.Parse (s [1]);
		return ws;
	}

	public static WorldStage CreateWithGDEStageKey (string stage_key)
	{
		WorldStage ws = new WorldStage ();
		string[] s = stage_key.Split ('_');
		ws.WorldId = int.Parse (s [1]);
		ws.StageId = int.Parse (s [2]);
		return ws;
	}

	public static WorldStage CreateWithWorldIdxAndStageIdx (int world_idx, int stage_idx)
	{
		return new WorldStage () {
			WorldId = world_idx + 1,
			StageId = stage_idx + 1
		};
	}
}
