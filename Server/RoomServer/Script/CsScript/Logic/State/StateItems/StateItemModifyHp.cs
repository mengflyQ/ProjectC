using System;
using GameServer.RoomServer;
using System.Collections.Generic;

public class StateItemModifyHp : BaseStateItem
{
    protected enum UseAtbType
    {
        Target,
        Src
    }

    public override void Enter()
    {
        if (excel.dataIntArray.Length != 6)
            return;
        int atbID = excel.dataIntArray[2];
        if (atbID > 0)
        {
            float v = (float)excel.dataIntArray[3] * 0.0001f;
            UseAtbType uat = (UseAtbType)excel.dataIntArray[4];
            if (uat == UseAtbType.Target)
            {
                fHpModif = (float)stateGroup.mSelf.GetAtb((AtbType)atbID) * v;
            }
            else
            {
                Scene scn = stateGroup.mSelf.mScene;
                if (scn == null)
                    return;
                Character cha = scn.FindCharacter(stateGroup.mSrcUID);
                if (cha == null)
                    return;

                fHpModif = (float)cha.GetAtb((AtbType)atbID) * v;
            }
        }
        mChgHpPeriod = (float)excel.dataIntArray[5] * 0.001f;

        if (mChgHpPeriod > 0)
        {
            mStartTime = Time.ElapsedSeconds;
            mNextChgHpTime = mStartTime;
            mNextChgHpTime += mChgHpPeriod;
        }
        else
        {
            ChgHp();
        }
    }

    public override void LogicTick()
    {
        float life = Time.ElapsedSeconds - mStartTime;
        if (mChgHpPeriod > 0 && life > mNextChgHpTime)
        {
            ChgHp();
            mNextChgHpTime += mChgHpPeriod;
        }
    }

    private void ChgHp()
    {
        int hp = stateGroup.mSelf.GetAtb(AtbType.HP);
        bool hpPct = (excel.dataIntArray[0] != 0);
        float fHp = (float)excel.dataIntArray[1];
        int chg = 0;

        if (hpPct)
        {
            float v = (float)stateGroup.mSelf.GetAtb(AtbType.MaxHP) * (fHp / 10000) + fHpModif;
            chg = (int)MathLib.Mathf.Floor(v);

            stateGroup.mSelf.SetAtb(AtbType.HP, hp + chg);
        }
        else
        {
            chg = (int)MathLib.Mathf.Floor(fHp + fHpModif);

            stateGroup.mSelf.SetAtb(AtbType.HP, hp + chg);
        }
    }

    float fHpModif;
    float mChgHpPeriod;
    float mNextChgHpTime;
    float mStartTime;
}