using System;
using UnityEngine;
using System.Collections.Generic;

public class Trigger
{
    public Trigger(excel_trigger_list triggerExcel)
    {
        mTriggerList = triggerExcel;
        mFirstCondition = excel_trigger_condition.Find(triggerExcel.firstCondition);
        for (int i = 0; i < triggerExcel.conditions.Length; ++i)
        {
            excel_trigger_condition condition = excel_trigger_condition.Find(triggerExcel.conditions[i]);
            mTriggerConditions.Add(condition);
        }
        for (int i = 0; i < triggerExcel.events.Length; ++i)
        {
            excel_trigger_event e = excel_trigger_event.Find(triggerExcel.events[i]);
            mTriggerEvents.Add(e);
        }
    }

    public bool CheckTriggerType(params object[] triggerParams)
    {
        if (mTriggerList == null)
            return false;
        if (triggerParams == null || triggerParams.Length != mTriggerList.triggerParams.Length)
            return false;
        excel_trigger_list excel = mTriggerList;
        for (int i = 0; i < excel.triggerParams.Length; ++i)
        {
            if (excel.triggerParams[i] != (int)triggerParams[i])
                return false;
        }
        return true;
    }

    public bool CheckCondition(object bindObj)
    {
        if (mFirstCondition == null)
            return true;
        TriggerConditionType conditionType = (TriggerConditionType)mFirstCondition.condition;
        TriggerConditionRegister.TriggerConditionMethod func = null;
        if (TriggerConditionRegister.conditions.TryGetValue(conditionType, out func))
        {
            Debug.LogError("未找到条件判断方法，条件ID：" + conditionType.ToString());
            return false;
        }
        return func(this, bindObj, mFirstCondition.condParams);
    }

    public void DoEvent(object bindObj)
    {
        for (int i = 0; i < mTriggerList.events.Length; ++i)
        {
            int eventID = mTriggerList.events[i];
            excel_trigger_event e = excel_trigger_event.Find(eventID);
            if (e == null)
                continue;
            TriggerEventType et = (TriggerEventType)e.eventType;
            TriggerEventRegister.TriggerEventMethod method = null;
            if (!TriggerEventRegister.events.TryGetValue(et, out method))
            {
                continue;
            }
            method(this, bindObj, e.eventParams);
        }
    }

    public TriggerBindType BindType
    {
        get
        {
            return (TriggerBindType)mTriggerList.bindType;
        }
    }

    public TriggerType TriggerType
    {
        get
        {
            return (TriggerType)mTriggerList.triggerType;
        }
    }

    public excel_trigger_list mTriggerList;
    public excel_trigger_condition mFirstCondition;
    public List<excel_trigger_condition> mTriggerConditions = new List<excel_trigger_condition>();
    public List<excel_trigger_event> mTriggerEvents = new List<excel_trigger_event>();
}