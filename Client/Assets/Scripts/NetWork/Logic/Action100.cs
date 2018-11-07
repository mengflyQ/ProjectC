using UnityEngine;
using System.Collections;
using System;
using GameRanking.Pack;
using ZyGames.Framework.Common.Serialization;

public class Action100 : GameAction
{
	private bool isCustom = false;

	public Action100() : base(100)
	{
		
	}

	public override ActionResult GetResponseData()
	{
		Debug.Log("GetResponseData");
		return new ActionResult();
	}

	private ActionResult actionResult;

	// 服务器返回的内容，按照服务器Push的顺序返回相应值;
	protected override void DecodePackage (NetReader reader)
	{
		if (reader != null && reader.StatusCode == 0)
		{
			Debug.Log("1000 response OK " + reader.readString());
		}
	}

	protected override void SendParameter(NetWriter writer, ActionParam actionParam)
	{
		if (actionParam != null && actionParam.HasValue)
		{
			// 自定义对象参数格式;
			isCustom = true;
			var randData = actionParam.GetValue<RankData>();
			byte[] data = ProtoBufUtils.Serialize(randData);
		}
		else
		{
			isCustom = false;
			writer.writeString("hello_key", "来自客户端消息");
			writer.writeInt32("Score", 100);
		}	
	}
}