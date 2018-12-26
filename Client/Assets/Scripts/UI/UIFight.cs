using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIFight : MonoBehaviour
{
	void Start()
    {
		
	}
	
	void Update()
    {
		
	}

    public void DoSkill(int skillID)
    {
        Player player = GameController.mMainPlayer;
        if (player == null)
            return;
        Character target = player.GetTarget();
        ReqSkill reqSkill = new ReqSkill();
        reqSkill.skillID = skillID;
        reqSkill.position = Vector3Packat.FromVector3(player.Position);
        reqSkill.direction = Vector3Packat.FromVector3(player.Direction);
        reqSkill.targetID = target == null ? 0 : target.UserID;
        reqSkill.autoTargetPos = true;
        NetWork.SendPacket<ReqSkill>(CTS.CTS_SkillReq, reqSkill, null);
    }
}
