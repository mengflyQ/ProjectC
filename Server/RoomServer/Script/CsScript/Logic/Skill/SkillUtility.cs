using System;

public static class SkillUtility
{
    public static float GetDistance(Character a, Character b, DistanceCalcType calcType)
    {
        float dist = (b.Position - a.Position).Length();
        switch (calcType)
        {
            case DistanceCalcType.Center:
                break;
            case DistanceCalcType.OuterA:
                dist -= a.Radius;
                dist = MathLib.Mathf.Max(dist, 0.0f);
                break;
            case DistanceCalcType.OuterB:
                dist -= b.Radius;
                dist = MathLib.Mathf.Max(dist, 0.0f);
                break;
            case DistanceCalcType.OuterAB:
                dist -= b.Radius;
                dist -= a.Radius;
                dist = MathLib.Mathf.Max(dist, 0.0f);
                break;
        }
        return dist;
    }
}
