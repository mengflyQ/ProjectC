using System;
using System.Collections.Generic;
using UnityEngine;

public class SkillHit
{
    public static void Hit(Character cha, int hitID, SkillContext context)
    {
        excel_skill_hit hitExcel = excel_skill_hit.Find(hitID);
        if (hitExcel == null)
            return;

        DisplayHit(true, hitExcel, cha.gameObject);

        SkillHitShape type = (SkillHitShape)hitExcel.hitType;
        SkillTargetType targetType = (SkillTargetType)hitExcel.targetType;

        float data1 = (float)hitExcel.hitData1 * 0.001f;
        float data2 = (float)hitExcel.hitData2 * 0.001f;
        float height = (float)hitExcel.hitData3 * 0.001f;

        bool hitMultiple = hitExcel.hitType >= 3;

        List<Character> targets = new List<Character>();
        if (!hitMultiple)
        {
            Character skillTarget = context.SkillTarget;
            if (skillTarget == null)
                return;
            if (!IsMayHit(skillTarget, context, hitExcel))
                return;
            if (!CheckTargetType(cha, skillTarget, targetType))
                return;
            targets.Add(skillTarget);
        }
        else
        {
            Scene scn = SceneSystem.Instance.mCurrentScene;
            for (int i = 0; i < scn.GetCharacterCount(); ++i)
            {
                Character c = scn.GetCharacterByIndex(i);
                if (c == null)
                    continue;
                if (!IsMayHit(c, context, hitExcel))
                    return;
                if (!CheckTargetType(cha, c, targetType))
                    continue;
                targets.Add(c);
            }
        }

        Vector3 srcPosition = context.mOwner.Position;
        Vector3 srcForward = context.mOwner.Direction;

        int hitCount = 0;
        for (int i = 0; i < targets.Count; ++i)
        {
            Character target = targets[i];
            switch (type)
            {
                case SkillHitShape.FanSingle:
                case SkillHitShape.FanMultiple:
                    {
                        if (Hit_Fan(srcPosition, srcForward, target, data1, data2 * 1000.0f, height))
                        {
                            ++hitCount;
                            context.mHitTarget = target;
                            // calc damage
                            target.PlayAnimation(4, AnimPlayType.Addition);

                            AddHitCount(target, context, hitExcel);
                        }
                    }
                    break;
                case SkillHitShape.RectSingle:
                case SkillHitShape.RectMultiple:
                    {
                        if (Hit_Rect(srcPosition, srcForward, target, data1, data2, height))
                        {
                            ++hitCount;
                            context.mHitTarget = target;
                            // calc damage
                            target.PlayAnimation(4, AnimPlayType.Addition);

                            AddHitCount(target, context, hitExcel);
                        }
                    }
                    break;
                case SkillHitShape.CircleSingle:
                case SkillHitShape.CircleMultiple:
                    {
                        if (Hit_Circle(srcPosition, target, data1, height))
                        {
                            ++hitCount;
                            context.mHitTarget = target;
                            // calc damage
                            target.PlayAnimation(4, AnimPlayType.Addition);

                            AddHitCount(target, context, hitExcel);
                        }
                    }
                    break;
            }
            if (hitExcel.maxHitCount > 0 && hitCount > hitExcel.maxHitCount)
            {
                break;
            }
        }
    }

    static void DisplayHit(bool display, excel_skill_hit hitExcel, GameObject go)
    {
        if (!display)
            return;
        GameObject displayGO = new GameObject("Hit");
        displayGO.transform.position = go.transform.position;
        displayGO.transform.rotation = go.transform.rotation;
        displayGO.transform.localScale = go.transform.localScale;
        SkillHitDisplay hitDisplay = displayGO.AddComponent<SkillHitDisplay>();
        hitDisplay.mHitExcel = hitExcel;
    }

    static bool CheckTargetType(Character cha, Character target, SkillTargetType type)
    {
        switch (type)
        {
            case SkillTargetType.All:
                return true;
            case SkillTargetType.Enemy:
                {
                    if (CampSystem.IsEnemy(cha, target))
                        return true;
                    break;
                }
            case SkillTargetType.Friend:
                {
                    if (CampSystem.IsFriend(cha, target))
                        return true;
                    break;
                }
            case SkillTargetType.FriendDead:
                {
                    if (CampSystem.IsFriend(cha, target))
                    {
                        return true;
                    }
                    break;
                }
            case SkillTargetType.Neutral:
                {
                    if (!CampSystem.IsEnemy(cha, target) && !CampSystem.IsFriend(cha, target))
                        return true;
                    break;
                }
            case SkillTargetType.Self:
                {
                    if (cha == target)
                        return true;
                    break;
                }
        }
        return false;
    }

    static bool IsMayHit(Character cha, SkillContext context, excel_skill_hit hitExcel)
    {
        Dictionary<Character, int> hitmap;
        if (!context.mChaHitCount.TryGetValue(hitExcel.id, out hitmap))
        {
            return true;
        }

        int hitCount = 0;
        if (!hitmap.TryGetValue(cha, out hitCount))
        {
            return true;
        }
        int maxHitCount = (int)hitExcel.targetMaxHitCnt;
        if (maxHitCount <= 0)
            return true;
        if (hitCount > maxHitCount)
            return false;
        return true;
    }

    static void AddHitCount(Character cha, SkillContext context, excel_skill_hit hitExcel)
    {
        Dictionary<Character, int> hitmap;
        if (!context.mChaHitCount.TryGetValue(hitExcel.id, out hitmap))
        {
            hitmap = new Dictionary<Character, int>();
            context.mChaHitCount.Add(hitExcel.id, hitmap);

            hitmap.Add(cha, 1);
            return;
        }

        int hitCount = 0;
        if (!hitmap.TryGetValue(cha, out hitCount))
        {
            hitmap.Add(cha, 1);
            return;
        }
        int maxHitCount = (int)hitExcel.targetMaxHitCnt;
        if (maxHitCount > 0 && hitCount > maxHitCount)
            return;
        ++hitCount;
        hitmap[cha] = hitCount;
    }

    static bool Hit_Fan(Vector3 srcPosition, Vector3 srcForward, Character target, float radius, float halfAngle, float height)
    {
        Vector3 targetPos = target.Position;
        float targetHeight = target.Height;
        if (targetPos.y < (srcPosition.y - targetHeight) || targetPos.y > srcPosition.y + height)
            return false;

        Vector3 dir = targetPos - srcPosition;
        dir.y = 0.0f;

        if ((dir.magnitude - target.Radius) > radius)
        {
            return false;
        }

        float cos = Vector3.Dot(srcForward, dir.normalized);
        float radian = (float)System.Math.Acos((double)cos);

        float angleDiff = radian * Mathf.Rad2Deg;
        if (angleDiff > halfAngle)
        {
            return false;
        }
        return true;
    }

    static bool Hit_Rect(Vector3 srcPosition, Vector3 srcForward, Character target, float width, float depth, float height)
    {
        float targetHeight = target.Height;
        if (target.Position.y < (srcPosition.y - targetHeight) || target.Position.y > srcPosition.y + height)
            return false;

        Vector2 targetPos = new Vector2(target.Position.x, target.Position.z);
        Vector2 srcPos = new Vector2(srcPosition.x, srcPosition.z);

        targetPos = targetPos - srcPos;
        float dist = targetPos.magnitude;

        srcForward.y = 0.0f;
        srcForward.Normalize();
        Vector3 srcRight = Vector3.Cross(Vector3.up, srcForward);

        Vector2 right = new Vector2(srcRight.x, srcRight.z);
        right.Normalize();
        Vector2 forward = new Vector2(srcForward.x, srcForward.z);

        Vector2 dir = targetPos.normalized;

        float cosX = Vector2.Dot(right, dir);
        float cosZ = Vector2.Dot(forward, dir);

        targetPos.x = dist * cosX;
        targetPos.y = dist * cosZ;

        float w = width * 0.5f;
        if ((targetPos.x + target.Radius) < -w || (targetPos.x - target.Radius) > w)
            return false;
        if ((targetPos.y + target.Radius) < 0.0f || (targetPos.y - target.Radius) > depth)
            return false;
        return true;
    }

    static bool Hit_Circle(Vector3 srcPosition, Character target, float radius, float height)
    {
        Vector3 targetPos = target.Position;
        float targetHeight = target.Height;
        if (targetPos.y < (srcPosition.y - targetHeight) || targetPos.y > srcPosition.y + height)
            return false;

        Vector3 dir = targetPos - srcPosition;
        dir.y = 0.0f;

        if ((dir.magnitude - target.Radius) > radius)
        {
            return false;
        }
        return true;
    }
}