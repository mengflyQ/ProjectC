using System;
using UnityEngine;
using System.Collections.Generic;

public class StateItemModifyHp : BaseStateItem
{
    public enum UseAtbType
    {
        [EnumDescription("状态目标")]
        Target,
        [EnumDescription("状态来源")]
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
                Scene scn = SceneSystem.Instance.mCurrentScene;
                if (scn == null)
                    return;
                Character cha = scn.GetCharacter(stateGroup.mSrcUID);
                if (cha == null)
                    return;

                fHpModif = (float)cha.GetAtb((AtbType)atbID) * v;
            }
        }
        mChgHpPeriod = (float)excel.dataIntArray[5] * 0.001f;

        if (mChgHpPeriod > 0)
        {
            mStartTime = Time.realtimeSinceStartup;
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
        float life = Time.realtimeSinceStartup - mStartTime;
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
            chg = (int)Mathf.Floor(v);

            stateGroup.mSelf.SetAtb(AtbType.HP, hp + chg);
        }
        else
        {
            chg = (int)Mathf.Floor(fHp + fHpModif);

            stateGroup.mSelf.SetAtb(AtbType.HP, hp + chg);
        }
    }

    float fHpModif;
    float mChgHpPeriod;
    float mNextChgHpTime;
    float mStartTime;
}