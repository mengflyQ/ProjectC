using System;
using System.Collections.Generic;

public class StateMgr
{
    public StateMgr(Character cha)
    {
        mOwner = cha;
    }

    private excel_state_group CheckMutex(excel_state_group excel, int srcUID)
    {
        if (excel == null)
            return null;

        excel_state_group mutexExcel = null;
        for (int i = 0; i < mStateList.Count; ++i)
        {
            StateGroup s = mStateList[i];
            // 如果互斥检查范围是全部，则检查全部状态是否呼出
            // 如果互斥检查范围是同一来源，则只检查同一来源的状态是否互斥
            if (excel.mutexScope == (int)StateMutexScope.All
                || (excel.mutexScope == (int)StateMutexScope.SameSrc && srcUID == s.mSrcUID))
            {
                int mutexID = excel.mutexID;
                // 同ID或同互斥组的状态都算互斥;
                if ((mutexID > 0 && mutexID == s.mExcel.mutexID)
                    || (mutexID == 0 && excel.id == s.mExcel.id))
                {
                    mutexExcel = s.mExcel;
                    break;
                }
            }
        }

        if (mutexExcel != null)
        {
            
            if (excel.mutexPriority > mutexExcel.mutexPriority)
            {
                // 优先级高，直接删除并替换原来的状态;
                DelState(mutexExcel.id);
            }
            else if (excel.mutexPriority == mutexExcel.mutexPriority)
            {
                // 同优先级，状态叠加升级一层（如果没有填升级ID则直接替换掉原来的状态）
                int nextID = mutexExcel.mutexNextID;
                if (nextID > 0)
                {
                    excel = excel_state_group.Find(nextID);
                }
                DelState(mutexExcel.id);
            }
            else if (excel.mutexPriority < mutexExcel.mutexPriority)
            {
                // 低优先级，添加失败;
                excel = null;
            }
        }

        return excel;
    }

    public void AddState(int id, int srcUID)
    {
        excel_state_group excel = excel_state_group.Find(id);
        excel = CheckMutex(excel, srcUID);
        if (excel == null)
            return;

        StateGroup state = new StateGroup(excel, mOwner, srcUID);

        mStates[excel.id] = state;
        mStateList.Add(state);

        state.Enter();
    }

    public void DelState(int id)
    {
        StateGroup state = null;
        if (!mStates.TryGetValue(id, out state))
        {
            return;
        }
        mRemoveList.Add(state);
    }

    void DelState(StateGroup state)
    {
        if (state == null)
        {
            return;
        }
        mRemoveList.Add(state);
    }

    public void LogicTick()
    {
        for (int i = 0; i < mStateList.Count; ++i)
        {
            StateGroup state = mStateList[i];
            if (!state.LogicTick())
            {
                DelState(state);
            }
        }

        if (mRemoveList.Count > 0)
        {
            for (int i = 0; i < mRemoveList.Count; ++i)
            {
                StateGroup state = mRemoveList[i];
                if (state == null)
                    continue;
                mStates.Remove(state.mExcel.id);
                mStateList.Remove(state);
            }
            mRemoveList.Clear();
        }

        for (int i = 0; i < mStateItems.Length; ++i)
        {
            List<BaseStateItem> items = mStateItems[i];
            if (items == null)
                continue;
            for (int j = 0; j < items.Count; ++j)
            {
                BaseStateItem item = items[j];
                if (item == null)
                    continue;
                item.LogicTick();
            }
        }
    }

    public void AddStateItem(BaseStateItem stateItem)
    {
        StateItemType type = stateItem.stateItemType;
        List<BaseStateItem> items = mStateItems[(int)StateItemType.Count];
        if (items == null)
        {
            items = new List<BaseStateItem>();
            mStateItems[(int)type] = items;
        }

        stateItem.Enter();

        items.Add(stateItem);

        stateItem.OnOverlay();
    }

    public void DelStateItem(BaseStateItem stateItem)
    {
        StateItemType type = stateItem.stateItemType;
        List<BaseStateItem> items = mStateItems[(int)StateItemType.Count];
        if (items == null)
        {
            return;
        }
        if (items.Count == 0)
            return;
        BaseStateItem lastItem = items[items.Count - 1];
        items.Remove(stateItem);

        lastItem.Exit();

        if (lastItem == stateItem && items.Count > 0)
        {
            lastItem = items[items.Count - 1];
            lastItem.OnOverlay();
        }
    }

    public static void AddState(Character cha, int id, int srcUID)
    {
        cha.mStateMgr.AddState(id, srcUID);
    }

    public Character mOwner;
    public Dictionary<int, StateGroup> mStates = new Dictionary<int, StateGroup>();
    public List<StateGroup> mStateList = new List<StateGroup>();
    public List<BaseStateItem>[] mStateItems = new List<BaseStateItem>[(int)StateItemType.Count];

    private List<StateGroup> mRemoveList = new List<StateGroup>();
}