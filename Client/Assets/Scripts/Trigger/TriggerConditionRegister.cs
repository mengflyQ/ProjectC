using System;
using System.Collections.Generic;

public static class TriggerConditionRegister
{
    static bool AND(Trigger trigger, object bindObj, params object[] datas)
    {
        if (datas == null || datas.Length != 2)
            return false;
        TriggerConditionMethod condFunc1 = null;
        TriggerConditionMethod condFunc2 = null;

        int id1 = (int)datas[0];
        int id2 = (int)datas[1];
        for (int i = 0; i < trigger.mTriggerConditions.Count; ++i)
        {
            excel_trigger_condition excel = trigger.mTriggerConditions[i];
            if (excel.id == id1)
            {
                TriggerConditionType ct = (TriggerConditionType)excel.condition;
                conditions.TryGetValue(ct, out condFunc1);
            }
            if (excel.id == id2)
            {
                TriggerConditionType ct = (TriggerConditionType)excel.condition;
                conditions.TryGetValue(ct, out condFunc2);
            }
        }
        if (condFunc1 == null || condFunc2 == null)
            return false;
        bool b1 = condFunc1(trigger, bindObj, datas);
        bool b2 = condFunc1(trigger, bindObj, datas);
        return b1 && b2;
    }

    static bool OR(Trigger trigger, object bindObj, params object[] datas)
    {
        if (datas == null || datas.Length != 2)
            return false;
        TriggerConditionMethod condFunc1 = null;
        TriggerConditionMethod condFunc2 = null;

        int id1 = (int)datas[0];
        int id2 = (int)datas[1];
        for (int i = 0; i < trigger.mTriggerConditions.Count; ++i)
        {
            excel_trigger_condition excel = trigger.mTriggerConditions[i];
            if (excel.id == id1)
            {
                TriggerConditionType ct = (TriggerConditionType)excel.condition;
                conditions.TryGetValue(ct, out condFunc1);
            }
            if (excel.id == id2)
            {
                TriggerConditionType ct = (TriggerConditionType)excel.condition;
                conditions.TryGetValue(ct, out condFunc2);
            }
        }
        if (condFunc1 == null || condFunc2 == null)
            return false;
        bool b1 = condFunc1(trigger, bindObj, datas);
        bool b2 = condFunc1(trigger, bindObj, datas);
        return b1 || b2;
    }

    static bool NOT(Trigger trigger, object bindObj, params object[] datas)
    {
        if (datas == null || datas.Length != 1)
            return false;
        TriggerConditionMethod condFunc = null;

        int id = (int)datas[0];
        for (int i = 0; i < trigger.mTriggerConditions.Count; ++i)
        {
            excel_trigger_condition excel = trigger.mTriggerConditions[i];
            if (excel.id == id)
            {
                TriggerConditionType ct = (TriggerConditionType)excel.condition;
                conditions.TryGetValue(ct, out condFunc);
            }
        }
        if (condFunc == null)
            return false;
        bool b = condFunc(trigger, bindObj, datas);
        return !b;
    }

    static bool CharacterHasState(Trigger trigger, object bindObj, params object[] datas)
    {
        Character cha = bindObj as Character;
        if (cha == null)
            return false;
        if (cha.mStateMgr == null)
            return false;
        if (datas == null || datas.Length < 1)
            return false;
        int stateID = (int)datas[0];
        return cha.mStateMgr.HasState(stateID);
    }

    static bool CharacterNotHasState(Trigger trigger, object bindObj, params object[] datas)
    {
        Character cha = bindObj as Character;
        if (cha == null)
            return false;
        if (cha.mStateMgr == null)
            return false;
        if (datas == null || datas.Length < 1)
            return false;
        int stateID = (int)datas[0];
        return !cha.mStateMgr.HasState(stateID);
    }

    public static void Initialize()
    {
        conditions[TriggerConditionType.AND] = AND;
        conditions[TriggerConditionType.OR] = OR;
        conditions[TriggerConditionType.NOT] = NOT;
        conditions[TriggerConditionType.CharacterHasState] = CharacterHasState;
        conditions[TriggerConditionType.CharacterNotHasState] = CharacterNotHasState;
    }

    public delegate bool TriggerConditionMethod(Trigger trigger, object bindObj, params object[] datas);
    public static Dictionary<TriggerConditionType, TriggerConditionMethod> conditions = new Dictionary<TriggerConditionType, TriggerConditionMethod>();
}