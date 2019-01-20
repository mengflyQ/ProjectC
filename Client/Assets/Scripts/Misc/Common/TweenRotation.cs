using System;
using UnityEngine;

public class TweenRotation : TweenBase
{
    protected override void UpdateTween(float t)
    {
        transform.eulerAngles = Vector3.Lerp(from, to, t);
    }

    public Vector3 from;
    public Vector3 to;
}