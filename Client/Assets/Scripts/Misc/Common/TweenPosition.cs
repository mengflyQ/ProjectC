using System;
using UnityEngine;

public class TweenPosition : TweenBase
{
    protected override void UpdateTween(float t)
    {
        transform.position = Vector3.Lerp(from, to, t);
    }

    public Vector3 from = Vector3.zero;
    public Vector3 to = Vector3.zero;
}