using System;
using System.Collections.Generic;
using UnityEngine;

public class StateGroup
{
    public StateGroup(excel_state_group excel, Character self, int srcUID)
    {
        mSelf = self;
        mStateMgr = mSelf.mStateMgr;
        mExcel = excel;
        mSrcUID = srcUID;
    }

    public void Enter()
    {
        mStartTime = Time.realtimeSinceStartup;

        for (int i = 0; i < mExcel.stateEffectIDs.Length; ++i)
        {
            int stateEffectID = mExcel.stateEffectIDs[i];
            excel_state_effect excel = excel_state_effect.Find(stateEffectID);
            if (excel == null)
                continue;
            StateItemType stateItemType = (StateItemType)excel.type;

            BaseStateItem stateItem = BaseStateItem.CreateStateItem(stateItemType);
            stateItem.excel = excel;
            stateItem.stateGroup = this;

            mStateMgr.AddStateItem(stateItem);
            mStateItems.Add(stateItem);
        }
    }

    public bool LogicTick()
    {
        float life = Time.realtimeSinceStartup - mStartTime;
        float duration = (float)mExcel.duration * 0.001f;

        if (duration > 0.0f && life > duration)
        {
            return false;
        }

        return true;
    }

    public void Exit()
    {
        for (int i = 0; i < mStateItems.Count; ++i)
        {
            BaseStateItem stateItem = mStateItems[i];
            if (stateItem == null)
                continue;
            mStateMgr.DelStateItem(stateItem);
        }
        mStateItems.Clear();
    }

    public Character mSelf;
    public excel_state_group mExcel = null;
    public int mSrcUID;

    private StateMgr mStateMgr = null;
    private float mStartTime;
    private List<BaseStateItem> mStateItems = new List<BaseStateItem>();
}