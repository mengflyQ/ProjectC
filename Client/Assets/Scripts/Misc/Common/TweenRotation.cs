using System;
using UnityEngine;

public class TweenRotation : TweenBase
{
    protected override void UpdateTween(float t)
    {
        if (transferType == TweenTransformType.World)
        {
            transform.eulerAngles = Vector3.Lerp(from, to, t);
        }
        else if (transferType == TweenTransformType.Local)
        {
            transform.localEulerAngles = Vector3.Lerp(from, to, t);
        }
    }

    public TweenTransformType transferType = TweenTransformType.World;
    public Vector3 from;
    public Vector3 to;
}