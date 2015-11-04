using UnityEngine;
using System.Collections;

public class WorldStage
{
	public int World;
	public int Stage;

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
			result = string.Format ("{0:00}-{1:00}", World, Stage);
			break;
		case Type.gde_stage_key:
			result = string.Format ("stage_{0:00}_{1:00}", World, Stage);
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
		ws.World = int.Parse (s [0]);
		ws.Stage = int.Parse (s [1]);
		return ws;
	}

	public static WorldStage CreateWithGDEStageKey (string stage_key)
	{
		WorldStage ws = new WorldStage ();
		string[] s = stage_key.Split ('_');
		ws.World = int.Parse (s [1]);
		ws.Stage = int.Parse (s [2]);
		return ws;
	}

	public static WorldStage CreateWithWorldIdxAndStageIdx (int world_idx, int stage_idx)
	{
		return new WorldStage () {
			World = world_idx + 1,
			Stage = stage_idx + 1
		};
	}
}
