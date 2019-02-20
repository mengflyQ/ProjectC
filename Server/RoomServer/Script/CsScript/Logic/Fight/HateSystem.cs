using System;
using System.Collections.Generic;
using GameServer.RoomServer;
using MathLib;

public class HateNode
{
    public Character mTarget;
    public int value;
    public float mTime;
    public bool mAttacked = false;
}

public class HateData
{
    public Character mSelf;
    public int mHateLinkID = 0;
    public List<HateNode> mHateList = new List<HateNode>();
    public Dictionary<Character, HateNode> mHateMap = new Dictionary<Character, HateNode>();

    public HateData(Character c)
    {
        mSelf = c;
    }

    public HateNode AddHateTarget(Character target)
    {
        HateNode hateNode = null;
        if (!mHateMap.TryGetValue(target, out hateNode))
        {
            hateNode = new HateNode();
            hateNode.mTarget = target;
            hateNode.mTime = Time.ElapsedSeconds;
            hateNode.value = 1;
            hateNode.mAttacked = false;
            mHateMap.Add(target, hateNode);
            mHateList.Add(hateNode);

            Character t = mSelf.GetTarget();
            if (t == null || !CampSystem.IsEnemy(t, mSelf))
            {
                mSelf.SetTarget(target);
            }
            HateSystem.UpdateFightState(mSelf);
        }
        return hateNode;
    }

    public HateNode GetHateNode(Character target)
    {
        HateNode hateNode = null;
        if (!mHateMap.TryGetValue(target, out hateNode))
        {
            return null;
        }
        return hateNode;
    }

    public void RemoveHateTarget(Character target)
    {
        mHateMap.Remove(target);
        for (int i = 0; i < mHateList.Count; ++i)
        {
            HateNode hateNode = mHateList[i];
            if (hateNode.mTarget == target)
            {
                mHateList.RemoveAt(i);
                HateSystem.UpdateFightState(mSelf);
                break;
            }
        }
    }
}

public class ScnHateLinks
{
    public Dictionary<int, List<Character>> mHateLinks = new Dictionary<int, List<Character>>();
}

public class HateSystem : BaseSystem
{
    public override void Initialize()
    {
        mScnHateLinks.Clear();
    }

    ScnHateLinks GetScnHateLinks(Character src)
    {
        if (src == null || src.mScene == null)
        {
            return null;
        }
        int scnGID = src.mScene.ScnUID;
        ScnHateLinks scnHateLinks = null;
        if (!mScnHateLinks.TryGetValue(scnGID, out scnHateLinks))
        {
            return null;
        }
        return scnHateLinks;
    }

    public void SetHateLink(Character src, int hateLink)
    {
        RemoveHateLink(src);

        if (hateLink > 0)
        {
            ScnHateLinks scnHateLinks = GetScnHateLinks(src);
            if (scnHateLinks == null)
            {
                scnHateLinks = new ScnHateLinks();
                mScnHateLinks.Add(src.mScene.ScnUID, scnHateLinks);
            }
            List<Character> hateLinkList = null;
            if (!scnHateLinks.mHateLinks.TryGetValue(hateLink, out hateLinkList))
            {
                hateLinkList = new List<Character>();
                scnHateLinks.mHateLinks.Add(hateLink, hateLinkList);
            }

            hateLinkList.Add(src);

            Character target = src.GetTarget();
            if (target == null)
            {
                target = GetHateLinkTarget(src);
                if (target != null)
                {
                    src.SetTarget(target);
                }
            }
        }

        src.mHateData.mHateLinkID = hateLink;
    }

    public void RemoveHateLink(Character src)
    {
        int hateLink = src.mHateData.mHateLinkID;
        if (hateLink > 0)
        {
            int scnGID = src.mScene.ScnUID;
            ScnHateLinks scnHateLinks = null;
            if (mScnHateLinks.TryGetValue(scnGID, out scnHateLinks))
            {
                List<Character> hateLinkList = null;
                if (scnHateLinks.mHateLinks.TryGetValue(hateLink, out hateLinkList))
                {
                    hateLinkList.Remove(src);
                }
            }
        }
    }

    public Character UpdateHateLinkTarget(Character src, Character target)
    {
        if (target == null)
        {
            if (!src.IsDead)
            {
                target = GetHateLinkTarget(src);
            }
        }

        if (CampSystem.IsEnemy(src, target))
        {
            int hateLinkID = src.mHateData.mHateLinkID;
            if (hateLinkID > 0)
            {
                ScnHateLinks scnHateLinks = GetScnHateLinks(src);
                if (scnHateLinks == null)
                {
                    return target;
                }
                List<Character> hateLinkList = null;
                if (!scnHateLinks.mHateLinks.TryGetValue(hateLinkID, out hateLinkList))
                {
                    return target;
                }
                for (int i = 0; i < hateLinkList.Count; ++i)
                {
                    Character c = hateLinkList[i];
                    if (c != src)
                    {
                        Character t = c.GetTarget();
                        if (t == null)
                        {
                            c.SetTarget(target, hateLink: false);
                            AddHate(c, target);
                        }
                    }
                }
            }
        }

        return target;
    }

    Character GetHateLinkTarget(Character src)
    {
        if (src.mHateData == null)
        {
            return null;
        }
        int hateLinkID = src.mHateData.mHateLinkID;
        if (hateLinkID <= 0)
        {
            return null;
        }
        Character srcTarget = src.GetTarget();

        ScnHateLinks scnHateLinks = GetScnHateLinks(src);
        if (scnHateLinks == null)
        {
            return null;
        }
        List<Character> hateLinkList = null;
        if (!scnHateLinks.mHateLinks.TryGetValue(hateLinkID, out hateLinkList))
        {
            return null;
        }
        for (int i = 0; i < hateLinkList.Count; ++i)
        {
            Character c = hateLinkList[i];
            if (c != src)
            {
                Character target = c.GetTarget();
                if (target != null && target != srcTarget)
                {
                    return target;
                }
            }
        }
        return null;
    }

    public void AddHate(Character src, Character target, int hate = 0)
    {
        if (src == null)
            return;
        if (target == null || !target.IsMaySelected(src) || !src.IsMaySelected(target))
            return;
        // 将被攻击者加入攻击者仇恨列表;
        HateData srcHate = src.mHateData;
        if (srcHate == null)
        {
            Debug.LogError("未找到来源仇恨数据");
        }
        srcHate.AddHateTarget(target);

        // 如果被攻击者没有目标，设置为攻击者;
        if (src.GetTarget() == null)
        {
            src.SetTarget(target);
        }

        // 将攻击者加入被攻击者仇恨列表;
        HateData targetHate = target.mHateData;
        if (targetHate == null)
        {
            Debug.LogError("未找到目标仇恨数据");
        }
        HateNode node = targetHate.AddHateTarget(src);

        if (hate > 0)
        {
            int newHate = node.value;
            newHate += hate;
            if (newHate <= 0 && node.value > 0)
            {
                node.value = 1;
                //targetHate.RemoveHateTarget(src);
                //srcHate.RemoveHateTarget(target);
            }
            else
            {
                node.value = newHate;
            }
            if (!node.mAttacked)
            {
                node.mAttacked = true;
            }
        }
    }

    public void RemoveHate(Character src, Character target)
    {
        HateData srcHate = src.mHateData;
        if (srcHate != null)
        {
            srcHate.RemoveHateTarget(target);

            // 仇恨清除后目标切换
            if (target == src.GetTarget())
            {
                src.SetTarget(GetNearHateTarget(src), hateLink: false);
            }
        }

        HateData targetHate = target.mHateData;
        if (targetHate != null)
        {
            targetHate.RemoveHateTarget(src);

            if (src == target.GetTarget())
            {
                target.SetTarget(GetNearHateTarget(target), hateLink: false);
            }
        }
    }

    public void Clear(Character src)
    {
        HateData srcHate = src.mHateData;
        if (srcHate != null)
        {
            for (int i = 0; i < srcHate.mHateList.Count; ++i)
            {
                HateNode node = srcHate.mHateList[i];
                HateData targetHate = node.mTarget.mHateData;
                if (targetHate != null)
                {
                    targetHate.RemoveHateTarget(src);
                }
            }
            srcHate.mHateList.Clear();
            srcHate.mHateMap.Clear();
        }
        
        UpdateFightState(src);
    }

    public bool HasHate(Character src)
    {
        HateData srcHate = src.mHateData;
        if (srcHate == null)
        {
            return false;
        }
        if (srcHate.mHateList.Count == 0)
        {
            return false;
        }
        return true;
    }

    public bool IsHate(Character src, Character target)
    {
        HateData srcHate = src.mHateData;
        if (srcHate == null)
        {
            return false;
        }

        return srcHate.mHateMap.ContainsKey(target);
    }

    public int GetHate(Character src, Character target)
    {
        HateData srcHate = src.mHateData;
        if (srcHate == null)
        {
            return 0;
        }
        HateNode node = null;
        if (!srcHate.mHateMap.TryGetValue(target, out node))
        {
            return 0;
        }
        return node.value;
    }

    public static void UpdateFightState(Character src)
    {

    }

    public Character GetNearHateTarget(Character src)
    {
        HateData srcHate = src.mHateData;
        if (srcHate == null)
        {
            return null;
        }
        
        if (srcHate.mHateList.Count == 0)
        {
            return null;
        }

        float distance = float.MaxValue;
        Character target = null;
        for (int i = 0; i < srcHate.mHateList.Count; ++i)
        {
            HateNode node = srcHate.mHateList[i];
            Vector3 p = node.mTarget.Position;
            p = target.Position - p;
            p.y = 0.0f;
            float dist = p.Length();

            if (distance > dist)
            {
                distance = dist;
                target = node.mTarget;
            }
        }
        return target;
    }

    public Character GetFirstAttackCha(Character src)
    {
        HateData srcHate = src.mHateData;
        if (srcHate == null)
        {
            return null;
        }

        for (int i = 0; i < srcHate.mHateList.Count; ++i)
        {
            HateNode node = srcHate.mHateList[i];
            if (node.mAttacked)
            {
                return node.mTarget;
            }
        }
        return null;
    }

    public Player GetFirstAttackPlayer(Character src)
    {
        HateData srcHate = src.mHateData;
        if (srcHate == null)
        {
            return null;
        }

        for (int i = 0; i < srcHate.mHateList.Count; ++i)
        {
            HateNode node = srcHate.mHateList[i];
            if (node.mAttacked && node.mTarget.IsPlayer())
            {
                return node.mTarget as Player;
            }
        }
        return null;
    }

    public Character GetLastAttackCha(Character src)
    {
        HateData srcHate = src.mHateData;
        if (srcHate == null)
        {
            return null;
        }

        for (int i = srcHate.mHateList.Count - 1; i >= 0; ++i)
        {
            HateNode node = srcHate.mHateList[i];
            if (node.mAttacked)
            {
                return node.mTarget;
            }
        }
        return null;
    }

    public Player GetLastAttackPlayer(Character src)
    {
        HateData srcHate = src.mHateData;
        if (srcHate == null)
        {
            return null;
        }

        for (int i = srcHate.mHateList.Count - 1; i >= 0; ++i)
        {
            HateNode node = srcHate.mHateList[i];
            if (node.mAttacked && node.mTarget.IsPlayer())
            {
                return node.mTarget as Player;
            }
        }
        return null;
    }

    public int GetHateNodeCount(Character src)
    {
        HateData srcHate = src.mHateData;
        if (srcHate == null)
        {
            return 0;
        }

        return srcHate.mHateList.Count;
    }

    public HateNode GetHateNodeByIndex(Character src, int index)
    {
        HateData srcHate = src.mHateData;
        if (srcHate == null)
        {
            return null;
        }

        if (index < 0 || index >= srcHate.mHateList.Count)
        {
            return null;
        }
        return srcHate.mHateList[index];
    }

    static HateSystem mInstance = null;
    public static HateSystem Instance
    {
        get
        {
            if (mInstance == null)
            {
                mInstance = new HateSystem();
                mInstance.Initialize();
            }
            return mInstance;
        }
    }
    public Dictionary<int, ScnHateLinks> mScnHateLinks = new Dictionary<int, ScnHateLinks>();
}