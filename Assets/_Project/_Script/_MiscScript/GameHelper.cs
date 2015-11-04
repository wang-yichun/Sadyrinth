using UnityEngine;
using System.Collections;

/// <summary>
/// 负责寻找下一关
/// </summary>
interface INextHelper
{
	string GetNextStageId (string cur_stage_id);

	bool IsLastStageInTheWorld (string cur_stage_id);

	bool IsLastWorldLastStage (string cur_stage_id);
}
