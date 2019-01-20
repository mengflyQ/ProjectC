using System;
using UnityEngine;
using UnityEngine.UI;

public class TweenUIColor : TweenBase
{
    private void Awake()
    {
        graphic = GetComponent<Graphic>();
    }

    protected override void UpdateTween(float t)
    {
        if (graphic == null)
            return;
        graphic.color = Color.Lerp(from, to, t);
    }

    private Graphic graphic = null;
    public Color from = Color.black;
    public Color to = Color.black;
}
