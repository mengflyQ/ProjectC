using System;
using System.Collections.Generic;
using GameServer.Model;
using ZyGames.Framework.Game.Contract;

public partial class NPC : Character
{
    public NPC() : base()
    {
        Type = CharacterType.NPC;
    }

    public override void Initialize()
    {
        base.Initialize();

        InitStateMachine();
    }

    public override void Update()
    {
        base.Update();

        UpdateBehaviour();
    }

    public excel_refresh mRefreshList = null;
}
