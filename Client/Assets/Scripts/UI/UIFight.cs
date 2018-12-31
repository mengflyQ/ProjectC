using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIFight : MonoBehaviour
{
	void Start()
    {
        MessageSystem.Instance.MsgRegister(MessageType.OnSetChaClass, OnSetChaClass);
	}

    private void OnDestroy()
    {
        
    }

    void Update()
    {
		
	}

    void OnSetChaClass(params object[] param)
    {
        excel_cha_class chaClass = param[0] as excel_cha_class;
        if (chaClass == null)
            return;
        if (skillBtns == null)
            return;
        for (int i = 0; i < chaClass.skillIDs.Length && i < skillBtns.Length; ++i)
        {
            Button btn = skillBtns[i];
            int skillID = chaClass.skillIDs[i];

            btn.onClick.AddListener(() =>
            {
                DoSkill(skillID);
            });
        }
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
        reqSkill.targetPos = Vector3Packat.FromVector3(Vector3.zero);
        NetWork.SendPacket<ReqSkill>(CTS.CTS_SkillReq, reqSkill, null);
    }

    public Button[] skillBtns;
}
