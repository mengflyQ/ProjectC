using System;
using MathLib;
using GameServer.RoomServer;

public class BTSavePosition : BTAction
{
    public BTSavePosition(Character self)
        : base(self)
    {

    }

    public override void Load(LitJson.JsonData json)
    {
        base.Load(json);

        Variable.LoadVariable(json["SPos"], self, mPosVar, out mPosVar);
    }

    protected override BTStatus Update()
    {
        mPosVar.Value = self.Position;

        return BTStatus.Success;
    }

    Variable mPosVar;
}