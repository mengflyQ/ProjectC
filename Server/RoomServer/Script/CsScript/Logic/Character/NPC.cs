using System;
using System.IO;
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

        mNpcAI = excel_npc_ai.Find(mRefreshList.npcAI);
        if (mNpcAI != null)
        {
            SetFlagMemory(FlagMemory.SearchTargetType, mNpcAI.searchTargetType);
            SetFlagMemory(FlagMemory.SearchTargetType, mNpcAI.searchTargetCondition);

            mSkillAIs = new excel_skill_ai[mNpcAI.skillAI.Length];
            for (int i = 0; i < mNpcAI.skillAI.Length; ++i)
            {
                mSkillAIs[i] = excel_skill_ai.Find(mNpcAI.skillAI[i]);
            }
        }

        // InitBehaviorTree();
        InitStateMachine();
        InitNPCAtb();
    }

    public void InitBehaviorTree()
    {
        if (string.IsNullOrEmpty(mRefreshList.behaviorTree))
        {
            return;
        }

        mBehaviorTree.LoadFromFile(@"../Data/BehaviorTree/" + mRefreshList.behaviorTree + ".json");
    }

    public override void Destroy()
    {
        base.Destroy();
        mScene.DelNPC(this);
    }

    public override void Update()
    {
        base.Update();

        // mBehaviorTree.Update();
        UpdateBehaviour();
    }

    public override void OnDead(DeadType type)
    {
        base.OnDead(type);

        if (mBehaviourMachine != null)
        {
            mBehaviourMachine.DoEvent(NPCFramework.BehaviourEvent.OnDead);
        }
    }

    public excel_refresh mRefreshList = null;

    public excel_npc_ai mNpcAI = null;

    public excel_skill_ai[] mSkillAIs = null;
}
