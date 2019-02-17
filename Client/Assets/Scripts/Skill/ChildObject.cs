using System;
using System.Collections.Generic;
using UnityEngine;

public class ChildObject : MonoBehaviour
{
    public static ChildObject CreateChildObject(int id, SkillContext context, Transform parentObj)
    {
        ChildObject skillObject = CreateChildObject(id, context.mOwner, parentObj, context.TargetPos, context.SkillTarget);
        if (skillObject == null)
        {
            Debug.LogError("未找到ID为" + id + "的子物体，请确认技能物体表配置有该子物体");
            return null;
        }
        skillObject.mContext.mSkillID = context.mSkillID;

        return skillObject;
    }

    public static ChildObject CreateChildObject(int id, Character owner, Transform parentObj, Vector3 targetPos, Character target = null)
    {
        excel_child_object data = excel_child_object.Find(id);
        if (data == null)
        {
            Debug.LogError("未找到ID为" + id + "的子物体");
            return null;
        }
        GameObject go  = ResourceSystem.Load<GameObject>("Particles/Prefabs/" + data.path);
        if (go == null)
            return null;
        go = GameObject.Instantiate(go);
        ChildObject childObject = go.GetComponent<ChildObject>();
        if (childObject == null)
        {
            childObject = go.AddComponent<ChildObject>();
        }
        if (!childObject.gameObject.activeSelf)
        {
            childObject.gameObject.SetActive(true);
        }

        childObject.Reset();
        childObject.mVanish = false;
        childObject.mContext.Reset();
        childObject.mContext.mOwner = owner;
        childObject.mContext.mChildObject = childObject;
        childObject.mContext.SkillTargetID = target == null ? 0 : target.gid;
        childObject.mContext.TargetPos = targetPos;
        childObject.mData = data;
        childObject.mParentObj = null;

        childObject.Init(parentObj);

        return childObject;
    }

    protected void Init(Transform parentObj)
    {
        mLifeStartTime = Time.realtimeSinceStartup;
        mParentObj = parentObj;

        InitPos(parentObj);
        InitDir();

        mTick = 0;

        mEvents = new List<excel_skill_event>();
        for (int i = 0; i < mData.events.Length; ++i)
        {
            int evtID = mData.events[i];
            excel_skill_event evt = excel_skill_event.Find(evtID);
            if (evt == null)
                continue;
            mEvents.Add(evt);
        }

        DoEvent(SkillEventTriggerType.StageBegin);

        Vector3 v = Position;
        v.y += ((float)mData.yOffset * 0.001f);
        Position = v;
    }

    public void Release()
    {
        if (mVanish)
        {
            DoEvent(SkillEventTriggerType.ExeptEnd);
        }
        else
        {
            DoEvent(SkillEventTriggerType.NormalEnd);
        }
        DoEvent(SkillEventTriggerType.FinalEnd);
        Reset();
        Destroy(gameObject);
        // .Despawn(transform);
    }

    void InitPos(Transform parentObj)
    {
        ChildObjectMoveType mt = (ChildObjectMoveType)mData.moveType;
        switch (mt)
        {
            case ChildObjectMoveType.Bind:
                {
                    transform.parent = mContext.mOwner.transform;
                    break;
                }
            case ChildObjectMoveType.BindPos:
                {
                    if (mTarget != null)
                    {
                        transform.parent = mTarget.transform;
                    }
                    break;
                }
        }

        ChildObjectInitPosType posType = (ChildObjectInitPosType)mData.initPos;
        switch (posType)
        {
            case ChildObjectInitPosType.Src:
                {
                    mParentObj = mContext.mOwner.transform;
                    Position = mParentObj.position;
                    break;
                }
            case ChildObjectInitPosType.SrcHinge:
                {
                    Character cha = mContext.mOwner;
                    string hingeName = mData.initHinge;
                    Transform hinge = cha.HingePoints.GetHingeName(hingeName);
                    if (hinge == null)
                    {
                        mParentObj = cha.transform;
                    }
                    else
                    {
                        mParentObj = hinge;
                    }
                    Position = mParentObj.position;
                    break;
                }
            case ChildObjectInitPosType.Target:
                {
                    if (mTarget == null)
                        mParentObj = mContext.mOwner.transform;
                    else
                        mParentObj = mTarget.transform;
                    Position = mParentObj.position;
                    break;
                }
            case ChildObjectInitPosType.TargetHinge:
                {
                    if (mTarget == null)
                        mParentObj = mContext.mOwner.transform;
                    else
                    {
                        string hingeName = mData.initHinge;
                        Transform hinge = mTarget.HingePoints.GetHingeName(hingeName);
                        if (hinge == null)
                            mParentObj = mTarget.transform;
                        else
                            mParentObj = hinge;
                    }
                    Position = mParentObj.position;
                    break;
                }
            case ChildObjectInitPosType.TargetPos:
                {
                    Position = mContext.TargetPos;
                    break;
                }
            case ChildObjectInitPosType.CurSkillObject:
                {
                    Position = parentObj.position;
                    break;
                }
        }

        mNavLayer = NavigationSystem.GetLayer(Position);
    }

    void InitDir()
    {
        ChildObjectInitDirType dirType = (ChildObjectInitDirType)mData.initDir;
        switch (dirType)
        {
            case ChildObjectInitDirType.WorldZ:
                {
                    Direction = Vector3.forward;
                    break;
                }
            case ChildObjectInitDirType.TargetHingeDir:
                {
                    if (mParentObj != null)
                    {
                        Direction = mParentObj.forward;
                    }
                    else
                    {
                        Direction = mContext.mOwner.transform.forward;
                    }
                    break;
                }
            case ChildObjectInitDirType.TargetDir:
                {
                    if (mParentObj != null)
                    {
                        Direction = mParentObj.forward;
                        break;
                    }
                    else
                    {
                        Direction = mContext.mOwner.transform.forward;
                    }
                    break;
                }
            case ChildObjectInitDirType.SrcHingeDir:
                {
                    if (mParentObj != null)
                    {
                        Direction = mParentObj.forward;
                    }
                    else
                    {
                        Direction = mContext.mOwner.transform.forward;
                    }
                    break;
                }
            case ChildObjectInitDirType.SrcDir:
                {
                    Direction = mParentObj.forward;
                    break;
                }
            case ChildObjectInitDirType.CurSkillObjectDir:
                {
                    Direction = mParentObj.forward;
                    break;
                }
        }

    }

    void LateUpdate()
    {
        if (mArriveTargetPos)
            return;

        Vector3 targetPos = mContext.mOwner.Position;
        bool onLand = false;
        bool onParabola = false;


        ChildObjectMoveType mt = (ChildObjectMoveType)mData.moveType;
        switch (mt)
        {
            case ChildObjectMoveType.FlyToTarget:
                {
                    if (mTarget != null)
                    {
                        targetPos = mTarget.Position;
                    }
                    break;
                }
            case ChildObjectMoveType.FlyToTargetPos:
                {
                    targetPos = mContext.TargetPos;
                    break;
                }
            case ChildObjectMoveType.FlyToTarget_OnLand:
                {
                    if (mTarget != null)
                    {
                        targetPos = mTarget.Position;
                    }
                    onLand = true;
                    break;
                }
            case ChildObjectMoveType.FlyToTargetPos_OnLand:
                {
                    targetPos = mContext.TargetPos;
                    onLand = true;
                    break;
                }
            case ChildObjectMoveType.None:
                {
                    return;
                }
            case ChildObjectMoveType.Bind:
            case ChildObjectMoveType.BindPos:
                {
                    FollowInitPos(mt);
                    return;
                }
        }

        Vector3 dir = targetPos - Position;

        if (dir.magnitude <= size)
        {
            mArriveTargetPos = true;
            DoEvent(SkillEventTriggerType.Frame, mTick);
            DoLoopEvent();
            return;
        }

        dir.Normalize();
        if (onParabola)
        {
            float t = Time.deltaTime;
            dir = Vector3.Lerp(Direction, dir, 5 * t);
            dir.Normalize();
        }
        Direction = dir;
        Position += (dir * (float)mData.speed * 0.001f * Time.deltaTime);

        Vector3 curDir = targetPos - Position;
        curDir.Normalize();
        if (Vector3.Angle(curDir, dir) > 90.0f)
        {
            mArriveTargetPos = true;
            DoEvent(SkillEventTriggerType.Frame, mTick);
            DoLoopEvent();
            return;
        }

        if (onLand)
        {
            float height = Position.y;
            if (NavigationSystem.GetLayerHeight(Position, mNavLayer, out height))
            {
                Vector3 pos = Position;
                pos.y = height + ((float)mData.yOffset * 0.001f);
                Position = pos;
            }
        }
    }

    void FollowInitPos(ChildObjectMoveType moveType)
    {
        if (mParentObj != null)
        {
            Position = mParentObj.position;
            if (moveType == ChildObjectMoveType.Bind)
            {
                transform.rotation = mParentObj.rotation;
            }
        }
    }

    void FixedUpdate()
    {
        if (mVanish)
        {
            Release();
            return;
        }

        float life = Time.realtimeSinceStartup - mLifeStartTime;
        float duration = (float)mData.duration * 0.03333f;
        if (mData.duration > 0 && life >= duration)
        {
            mVanish = false;
            Release();
            return;
        }
        UpdateSkill();
        UpdateState();
    }

    #region State
    void UpdateState()
    {
        if (mArriveTargetPos && IsTrait(ChildObjectTrait.DestroyOnArrive))
        {
            mVanish = true;
            return;
        }
        if ((mTarget == null || mTarget.IsDead) && IsTrait(ChildObjectTrait.DestroyOnTargetDead))
        {
            mVanish = true;
            return;
        }
        if (mContext == null)
        {
            mVanish = true;
            return;
        }
        if ((mContext.mOwner == null || mContext.mOwner.IsDead) && IsTrait(ChildObjectTrait.DestroyOnSrcDead))
        {
            mVanish = true;
            return;
        }
    }

    public bool IsVanish()
    {
        return mVanish;
    }

    public void SetVanish()
    {
        mVanish = true;
    }
    #endregion // State

    #region Skill
    void UpdateSkill()
    {
        DoEvent(SkillEventTriggerType.Frame, mTick);
        DoLoopEvent();
        ++mTick;
    }

    public bool IsTrait(ChildObjectTrait trait)
    {
        if (mData == null)
            return false;
        int f1 = (int)mData.trait;
        int f2 = (1 << (int)trait);
        return (f1 & f2) != 0;
    }

    void DoEvent(SkillEventTriggerType triggerType, int param1 = -1, int param2 = -1)
    {
        if (mEvents == null)
            return;
        for (int i = 0; i < mEvents.Count; ++i)
        {
            excel_skill_event eventInfo = mEvents[i];
            if (!IsClientEvent(eventInfo))
                continue;
            if ((ushort)triggerType != eventInfo.triggerType)
                continue;
            if (param1 != -1 && param1 != eventInfo.triggerParam1)
                continue;
            if (param2 != -1 && param2 != eventInfo.triggerParam2)
                continue;

            SkillEventRegister.SkillEventMethod e = null;
            SkillEventType type = (SkillEventType)eventInfo.eventType;
            if (!SkillEventRegister.events.TryGetValue(type, out e))
                continue;
            e(null, this, mContext, eventInfo);
        }
    }

    public void DoLoopEvent()
    {
        if (mEvents == null)
            return;
        for (int i = 0; i < mEvents.Count; ++i)
        {
            excel_skill_event eventInfo = mEvents[i];
            if (eventInfo.triggerType != (int)SkillEventTriggerType.Loop)
                continue;
            if (!IsClientEvent(eventInfo))
                continue;
            int tick = mTick - eventInfo.triggerParam1;
            if (tick >= 0 && eventInfo.triggerParam2 > 0 && tick % eventInfo.triggerParam2 == 0)
            {
                SkillEventRegister.SkillEventMethod e = null;
                SkillEventType type = (SkillEventType)eventInfo.eventType;
                if (!SkillEventRegister.events.TryGetValue(type, out e))
                    continue;
                e(null, this, mContext, eventInfo);
            }
        }
    }

    bool IsClientEvent(excel_skill_event evt)
    {
        int exePort = (int)evt.trait;
        if ((exePort & (1 << (int)SkillEventTrait.Client)) != 0)
        {
            return true;
        }
        return false;
    }

    public void OnHitEvent(uint hitID)
    {
        DoEvent(SkillEventTriggerType.Hit, (int)hitID);
        if (IsTrait(ChildObjectTrait.DestroyOnHit))
        {
            mVanish = true;
        }
    }
    #endregion // Skill

    void Reset()
    {
        mData = null;
        mNavLayer = 0;
        mLifeStartTime = 0.0f;
        mArriveTargetPos = false;
        mEvents = null;
        mTick = 0;
    }

    public float size
    {
        get
        {
            if (mData == null)
                return 0.0f;
            return (float)mData.size * 0.001f;
        }
    }

    public Vector3 Direction
    {
        set
        {
            Vector3 forward = value;
            transform.forward = forward;
        }
        get
        {
            return transform.forward;
        }
    }

    public Vector3 Position
    {
        set
        {
            transform.position = value;
        }
        get
        {
            return transform.position;
        }
    }

    private Character mTarget
    {
        get
        {
            return mContext.SkillTarget;
        }
    }

    public int SkillID
    {
        get
        {
            return mContext.mSkillID;
        }
    }

    private excel_child_object mData = null;
    private uint mNavLayer = 0;
    private float mLifeStartTime = 0.0f;
    private SkillContext mContext = new SkillContext();
    private bool mVanish = false;
    private bool mArriveTargetPos = false;
    private Transform mParentObj = null;

    // Skill
    private List<excel_skill_event> mEvents = null;
    private int mTick = 0;
}