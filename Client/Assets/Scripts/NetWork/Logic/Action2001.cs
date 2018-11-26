using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class Action2001 : GameAction
{
    public Action2001()
        : base((int)ActionType.EnterLobby)
	{
		
	}

	public override ActionResult GetResponseData()
	{
        if (actionResult == null)
            throw new Exception("Error: actionResult is null!");
        return actionResult;
	}

    protected override void DecodePackage(NetReader reader)
    {
        if (reader != null && reader.StatusCode == 0)
        {
            actionResult = new ActionResult();

            actionResult["NickName"] = reader.readString();
            actionResult["Level"] = reader.getInt();
            actionResult["Exp"] = reader.getInt();
            actionResult["Money"] = reader.getInt();
            actionResult["VIPLevel"] = reader.getInt();
        }
    }

    protected override void SendParameter(NetWriter writer, ActionParam actionParam)
    {
        writer.writeInt32("UserID", GameController.mUserInfo.uid);
    }

    private ActionResult actionResult;
}
