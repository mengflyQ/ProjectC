using System;
using System.Collections.Generic;
using MathLib;
using GameServer.RoomServer;

namespace NPCFramework
{
    public class UseSkillBehaviour : BaseBehaviour
    {
        public override void Enter()
        {
            if (mContext.mSkillIDs.Count <= 0 && mContext.mCurkillAI == null)
            {
                mContext.mCurkillAI = mContext.CalcSkillAI();

                if (mContext.mCurkillAI != null)
                {
                    mContext.mSkillIDs.Clear();
                    mNPC.SetSkill(null);
                }
                else
                {
                    return;
                }
                if (mContext.mCurkillAI == null)
                    return;
            }
            if (mContext.mCurkillAI != null)
            {
                mOverFrame = mContext.mCurkillAI.overFrame;
            }
            mCurFrame = 0;
        }

        public override void Exit()
        {
            mOverFrame = 0;
            mCurFrame = 0;
        }

        public override void LogicTick()
        {
            if (mContext.CanOffFight())
            {
                mNPC.mBehaviourMachine.SetBehaviour(BehaviourType.OffFight, mContext);
                return;
            }
            if (mContext.mSkillIDs.Count <= 0)
	        {
		        if (mContext.mCurkillAI != null)
		        {
			        // Rand Skill;
                    mContext.RandSkill(mContext.mCurkillAI, out mContext.mSkillIDs);
		        }
		        else
		        {
			        mNoSkillToUse = true;
		        }
	        }
            // Use Skill;
            int nSkillSize = mContext.mSkillIDs.Count;
            if (nSkillSize > 0)
            {
                if (mNPC.GetSkill() == null)
                {
                    Character target = mNPC.GetTarget();
                    int skillID = mContext.mSkillIDs[0];
                    mContext.mSkillIDs.RemoveAt(0);

                    if (skillID > 0)
			        {
                        SkillHandle handle = new SkillHandle();
                        handle.skillID = skillID;
                        handle.skillTargetID = target == null ? 0 : target.gid;
                        handle.caster = mNPC;
                        handle.autoTargetPos = true;
				        SkillResult rst = SkillHandle.UseSkill(handle);
				        if (rst != SkillResult.Success/* && mReplaceSkillID != 0*/)
				        {
                            // handle.skillID = mReplaceSkillID;
                            rst = SkillHandle.UseSkill(handle);
				        }
			        }
                }
                if (mContext.mSkillIDs.Count == 0)
                {
                    mContext.mCurkillAI = null;
                }
            }

            if (mNPC.GetSkill() == null)
            {
                mNPC.mBehaviourMachine.SetBehaviour(BehaviourType.BetweenSkill, mContext);
            }
        }

        int mCurFrame = 0;
        int mOverFrame = 0;
        bool mNoSkillToUse = false;
    }
}
