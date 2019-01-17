using System;
using System.Collections.Generic;
using MathLib;

public class SkillHit
{
    public static void Hit(Character cha, int hitID, SkillContext context)
    {
        excel_skill_hit hitExcel = excel_skill_hit.Find(hitID);
        if (hitExcel == null)
            return;

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
            Scene scn = cha.mScene;
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
                            OnHit(context.mOwner, target, hitExcel);
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
                            OnHit(context.mOwner, target, hitExcel);
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
                            OnHit(context.mOwner, target, hitExcel);
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

    static int CalcDamage(Character src, Character target, excel_skill_hit hitExcel, excel_skill_list skillList)
    {
        // 参数;
        float param = 350.0f;
        if (hitExcel.hurtType == (int)SkillHurtType.PhyDamage)
        {
            // 攻击力;
            float atk = (float)src.GetAtb(AtbType.PhyAtk);
            // 面板伤害 = 技能基础伤害 + 攻击力 + 技能伤害加成(%);
            float panelDamage = skillList.phyDamage + src.GetAtb(AtbType.PhyAtk) + atk * (float)skillList.phyPct * 0.0001f;
            // 防御力;
            float def = (float)target.GetAtb(AtbType.PhyDef);
            // 穿透;
            float pen = (float)src.GetAtb(AtbType.PhyPen);
            // 防御穿透比;
            float penPct = src.GetAtbPct(AtbType.PhyPenPct);
            // 伤害上下浮动;
            float ud = MathLib.Math.RandRange(-1.0f, 1.0f);
            // 伤害 = 面板伤害 / ( (防御力 - 穿透) * (1 - 防御穿透比) / 参数 + 1) + [-1, 1];
            float damage = panelDamage / ((def - pen) * (1.0f - penPct) / param + 1.0f) + ud;

            return (int)MathLib.Math.Max(1.0f, damage);
        }
        else if (hitExcel.hurtType == (int)SkillHurtType.MagDamage)
        {
            // 攻击力;
            float atk = (float)src.GetAtb(AtbType.MagAtk);
            // 面板伤害 = 技能基础伤害 + 攻击力 + 技能伤害加成(%);
            float panelDamage = skillList.magDamage + src.GetAtb(AtbType.MagAtk) + atk * (float)skillList.magPct * 0.0001f;
            // 防御力;
            float def = (float)target.GetAtb(AtbType.MagDef);
            // 穿透;
            float pen = (float)src.GetAtb(AtbType.MagPen);
            // 防御穿透比;
            float penPct = src.GetAtbPct(AtbType.MagPenPct);
            // 伤害上下浮动;
            float ud = MathLib.Math.RandRange(-1.0f, 1.0f);
            // 伤害 = 面板伤害 / ( (防御力 - 穿透) * (1 - 防御穿透比) / 参数 + 1) + [-1, 1];
            float damage = panelDamage / ((def - pen) * (1.0f - penPct) / param + 1.0f) + ud;

            return (int)MathLib.Math.Max(1.0f, damage);
        }
        return 0;
    }

    static void OnHit(Character src, Character target, excel_skill_hit hitExcel)
    {
        excel_skill_list skillList = excel_skill_list.Find(hitExcel.skillID);
        if (skillList == null)
        {
            Debug.LogError("判定表[" + hitExcel.id + "]的所属技能错误！");
            return;
        }
        int hurt = CalcDamage(src, target, hitExcel, skillList);
        int oldHP = target.HP;
        target.HP = target.HP - hurt;
        if (hitExcel.hurtType == (int)SkillHurtType.PhyDamage)
        {
            FightUtility.SendHpChg(src.uid, target.uid, hurt, HPChgType.PhyDamage);
        }
        else if (hitExcel.hurtType == (int)SkillHurtType.MagDamage)
        {
            FightUtility.SendHpChg(src.uid, target.uid, hurt, HPChgType.MagDamage);
        }
        else if (hitExcel.hurtType == (int)SkillHurtType.Cure)
        {
            FightUtility.SendHpChg(src.uid, target.uid, hurt, HPChgType.Cure);
        }
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

        if ((dir.Length() - target.Radius) > radius)
        {
            return false;
        }

        float cos = Vector3.Dot(srcForward, dir.normalize);
        float radian = (float)System.Math.Acos((double)cos);

        float angleDiff = radian * MathLib.Math.Rad2Deg;
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
        float dist = targetPos.Length();

        srcForward.y = 0.0f;
        srcForward.Normalize();
        Vector3 srcRight = Vector3.Cross(Vector3.up, srcForward);

        Vector2 right = new Vector2(srcRight.x, srcRight.z);
        right.Normalize();
        Vector2 forward = new Vector2(srcForward.x, srcForward.z);

        Vector2 dir = targetPos.normalize;

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

        if ((dir.Length() - target.Radius) > radius)
        {
            return false;
        }
        return true;
    }
}