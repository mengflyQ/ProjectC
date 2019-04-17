using System;
using MathLib;
using GameServer.RoomServer;

public class BTCastSkill : BTAction
{
    public BTCastSkill(Character self)
        : base(self)
    {

    }

    public override void Load(LitJson.JsonData json)
    {
        base.Load(json);

        mSkillID = json["SkillID"].AsInt;
        mSpecifyTarget = json["STarget"].AsBool;
        mSpecifyTargetPos = json["SPos"].AsBool;
        if (mSpecifyTarget)
        {
            LitJson.JsonData targetJson = json["Target"];
            Variable.LoadVariable(targetJson, self, mSkillTarget, out mSkillTarget);
        }
        if (mSpecifyTargetPos)
        {
            LitJson.JsonData posJson = json["Pos"];
            Variable.LoadVariable(posJson, self, mSkillTargetPos, out mSkillTargetPos);
        }
    }

    protected override BTStatus Update()
    {
        SkillHandle handle = new SkillHandle();
        handle.skillID = mSkillID;
        handle.caster = self;
        if (mSpecifyTarget)
        {
            VariableCharacter varTarget = mSkillTarget as VariableCharacter;
            handle.skillTargetID = varTarget.gid;
        }
        if (mSpecifyTargetPos)
        {
            handle.autoTargetPos = true;
            VariableVector3 varTargetPos = mSkillTargetPos as VariableVector3;
            handle.targetPos = varTargetPos.value;
        }
        else
        {
            handle.autoTargetPos = false;
        }

        SkillResult rst = SkillHandle.UseSkill(handle);
        if (rst == SkillResult.Success)
            return BTStatus.Success;
        return BTStatus.Failure;
    }

    int mSkillID;
    bool mSpecifyTarget;
    Variable mSkillTarget;
    bool mSpecifyTargetPos;
    Variable mSkillTargetPos;
}
