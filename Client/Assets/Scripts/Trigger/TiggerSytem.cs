using System;
using System.Collections.Generic;


public class TiggerSytem
{
    public void Initialize()
    {
    }

    public void DoTrigger(TriggerBindType bindType, object bindObj, TriggerType triggerType, params object[] triggerParams)
    {
        switch (bindType)
        {
            case TriggerBindType.NPC:
            case TriggerBindType.Player:
                {
                    Character cha = bindObj as Character;
                    for (int i = 0; i < cha.mTriggers.Count; ++i)
                    {
                        Trigger trigger = cha.mTriggers[i];

                        if (trigger == null)
                            continue;

                        if (!trigger.CheckTriggerType(triggerParams))
                            continue;

                        bool condition = trigger.CheckCondition(bindObj);

                        if (!condition)
                            continue;

                        trigger.DoEvent(bindObj);
                    }
                }
                break;
            case TriggerBindType.Scene:
                {
                    Scene scn = bindObj as Scene;
                    for (int i = 0; i < scn.mTriggers.Count; ++i)
                    {
                        Trigger trigger = scn.mTriggers[i];

                        if (trigger == null)
                            continue;

                        if (!trigger.CheckTriggerType(triggerParams))
                            continue;

                        bool condition = trigger.CheckCondition(bindObj);

                        if (!condition)
                            continue;

                        trigger.DoEvent(bindObj);
                    }
                }
                break;
        }
    }

    static TiggerSytem mInstance = null;
    public static TiggerSytem Instance
    {
        get
        {
            if (mInstance == null)
            {
                mInstance = new TiggerSytem();
                mInstance.Initialize();
            }
            return mInstance;
        }
    }
}
