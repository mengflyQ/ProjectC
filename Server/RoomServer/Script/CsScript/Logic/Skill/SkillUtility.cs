using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
                dist = MathLib.Math.Max(dist, 0.0f);
                break;
            case DistanceCalcType.OuterB:
                dist -= b.Radius;
                dist = MathLib.Math.Max(dist, 0.0f);
                break;
            case DistanceCalcType.OuterAB:
                dist -= b.Radius;
                dist -= a.Radius;
                dist = MathLib.Math.Max(dist, 0.0f);
                break;
        }
        return dist;
    }
}
