using System;
using System.Collections.Generic;
using MathLib;

public enum SearchTargetType
{
    Passive,
    ActiveEnemy,
    ActiveFriend
};

public enum SearchTargetCondition
{
    Nearest,
    Farest,
    HateHighest,
    LessHP,
    MostHP,
    Random
};

public class BTSerachTarget : BTAction
{
    public BTSerachTarget(Character self)
        : base(self)
    {

    }

    public override void Load(LitJson.JsonData json)
    {
        base.Load(json);

        mSearchTargetType = (SearchTargetType)json["SearchTargetType"].AsInt;
        mSearchTargetCondition = (SearchTargetCondition)json["SearchTargetCondition"].AsInt;
        LitJson.JsonData jsonSearchDist = json["SearchDist"];
        Variable.LoadVariable(jsonSearchDist, self, mSearchDist, out mSearchDist);
    }

    protected override BTStatus Update()
    {
        List<Character> selected = new List<Character>();
        Scene scn = self.mScene;

        float minDist = float.MaxValue;
        Character minDistCha = null;
        for (int i = 0; i < scn.GetCharacterCount(); ++i)
        {
            Character c = scn.GetCharacterByIndex(i);

            if (mSearchTargetType == SearchTargetType.Passive)
            {
                continue;
            }
            if (mSearchTargetType == SearchTargetType.ActiveEnemy
                && !CampSystem.IsEnemy(c, self))
            {
                continue;
            }
            if (mSearchTargetType == SearchTargetType.ActiveFriend
                && !CampSystem.IsFriend(c, self))
            {
                continue;
            }

            Vector3 d = c.Position - self.Position;
            d.y = 0.0f;
            float dist = d.Length();

            VariableFloat vf = mSearchDist as VariableFloat;
            if (dist > vf.value)
                continue;

            if (minDist > dist)
            {
                minDist = dist;
                minDistCha = c;
            }

            selected.Add(c);
        }

        Character target = SearchTarget(mSearchTargetCondition, selected, minDistCha);

        self.SetTarget(target);
        if (target == null)
        {
            return BTStatus.Failure;
        }
        return BTStatus.Success;
    }

    Character SearchTarget(SearchTargetCondition condition, List<Character> characters, Character nearest)
    {
        if (SearchTargetCondition.Random == condition)
        {
            int index = Mathf.RandRange(0, characters.Count - 1);
            return characters[index];
        }

        float maxFloatValue = float.MinValue;
        int maxIntValue = int.MinValue;
        int minIntValue = int.MaxValue;
        Character target = nearest;
        for (int i = 0; i < characters.Count; ++i)
        {
            Character c = characters[i];
            switch (condition)
            {
                case SearchTargetCondition.Farest:
                    {
                        Vector3 d = c.Position - self.Position;
                        d.y = 0.0f;
                        float dist = d.Length();
                        if (dist > maxFloatValue)
                        {
                            maxFloatValue = dist;
                            target = c;
                        }
                    }
                    break;
                case SearchTargetCondition.HateHighest:
                    {
                        int hate = HateSystem.Instance.GetHate(c, self);
                        if (hate == 0)
                        {
                            break;
                        }
                        if (maxIntValue < hate)
                        {
                            maxIntValue = hate;
                            target = c;
                        }
                    }
                    break;
                case SearchTargetCondition.LessHP:
                    {
                        int hp = c.HP;
                        if (hp == 0)
                        {
                            break;
                        }
                        if (minIntValue > hp)
                        {
                            minIntValue = hp;
                            target = c;
                        }
                    }
                    break;
                case SearchTargetCondition.MostHP:
                    {
                        int hp = c.HP;
                        if (hp == 0)
                        {
                            break;
                        }
                        if (maxIntValue < hp)
                        {
                            minIntValue = hp;
                            target = c;
                        }
                    }
                    break;
            }
        }
        return target;
    }

    SearchTargetType mSearchTargetType;
    SearchTargetCondition mSearchTargetCondition;
    Variable mSearchDist;
}