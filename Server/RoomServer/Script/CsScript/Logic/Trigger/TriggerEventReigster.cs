using System;
using MathLib;
using System.Collections.Generic;
using GameServer.RoomServer;

public static class TriggerEventRegister
{
    static void SceneRefreshNPC(Trigger trigger, object bindObj, params object[] datas)
    {
        Scene scn = GetScene(bindObj);

        if (scn == null)
            return;

        int[] data = datas[0] as int[];
        for (int i = 0; i < data.Length; ++i)
        {
            int refreshID = (int)data[i];
            excel_refresh refreshExcel = excel_refresh.Find(refreshID);
            if (refreshExcel == null)
                continue;
            RefreshSystem.Instance.Refresh(refreshExcel, scn);
        }            
    }

    static void CharacterAddState(Trigger trigger, object bindObj, params object[] datas)
    {
        Character cha = bindObj as Character;
        if (cha == null)
            return;

        for (int i = 0; i < datas.Length; ++i)
        {
            int stateID = (int)datas[i];
            cha.mStateMgr.AddState(stateID, cha.gid);
        }
    }

    static void CharacterDelState(Trigger trigger, object bindObj, params object[] datas)
    {
        Character cha = bindObj as Character;
        if (cha == null)
            return;

        for (int i = 0; i < datas.Length; ++i)
        {
            int stateID = (int)datas[i];
            cha.mStateMgr.DelState(stateID);
        }
    }

    static void CharacterUseSkill(Trigger trigger, object bindObj, params object[] datas)
    {
        Character cha = bindObj as Character;
        if (cha == null)
            return;

        int skillID = (int)datas[0];
        Character target = cha.GetTarget();

        SkillHandle handle = new SkillHandle();
        handle.skillID = skillID;
        handle.autoTargetPos = true;
        handle.skillTargetID = target == null ? 0 : target.gid;

        SkillHandle.UseSkill(handle);
    }

    static Scene GetScene(object bindObj)
    {
        if (bindObj is Scene)
            return bindObj as Scene;
        if (bindObj is Character)
        {
            Character cha = bindObj as Character;
            return cha.mScene;
        }
        return null;
    }

    public static void Initialize()
    {
        events[TriggerEventType.SceneRefreshNPC] = SceneRefreshNPC;
        events[TriggerEventType.CharacterAddState] = CharacterAddState;
        events[TriggerEventType.CharacterDelState] = CharacterDelState;
        events[TriggerEventType.CharacterUseSkill] = CharacterUseSkill;
    }

    public delegate void TriggerEventMethod(Trigger trigger, object bindObj, params object[] datas);
    public static Dictionary<TriggerEventType, TriggerEventMethod> events = new Dictionary<TriggerEventType, TriggerEventMethod>();
}
