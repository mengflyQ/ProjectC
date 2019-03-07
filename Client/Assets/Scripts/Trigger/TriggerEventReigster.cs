using System;
using System.Collections.Generic;

public static class TriggerEventRegister
{

    public static void Initialize()
    {
    }

    public delegate void TriggerEventMethod(Trigger trigger, object bindObj, params object[] datas);
    public static Dictionary<TriggerEventType, TriggerEventMethod> events = new Dictionary<TriggerEventType, TriggerEventMethod>();
}
