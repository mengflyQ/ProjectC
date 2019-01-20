using System;
using UnityEngine;
using UnityEngine.UI;

public class TweenUIAlpha : TweenBase
{
    private void Awake()
    {
        graphic = GetComponent<Graphic>();
    }

    protected override void UpdateTween(float t)
    {
        if (graphic == null)
            return;
        Color c = graphic.color;
        c.a = Mathf.Lerp(from, to, t);
        graphic.color = c;
    }

    private Graphic graphic = null;
    public float from = 1.0f;
    public float to = 1.0f;
}