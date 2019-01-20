using System;
using UnityEngine;
using UnityEngine.Events;

public enum TweenStyle
{
    Once,
    Loop,
    PingPong
}

public enum LerpMethod
{
    Linear,
    Curve,
}

public enum TweenTransformType
{
    World,
    Local,
}

public abstract class TweenBase : MonoBehaviour
{
    void OnEnable()
    {
        time = 0;
        delay = stateDelay;
        UpdateTime();
    }

    private void OnDisable()
    {
        time = 0;
        delay = stateDelay;
    }

    void UpdateTime()
    {
        time += Time.deltaTime * sign;
        if (time > duration)
        {
            if (style == TweenStyle.Once)
            {
                time = duration;
                if (onFinished != null)
                {
                    onFinished.Invoke();
                }
                enabled = false;
            }
            else if (style == TweenStyle.Loop)
                time = 0.0f;
            else if (style == TweenStyle.PingPong)
                sign = -1.0f;
        }
        if (time < 0.0f)
        {
            if (style == TweenStyle.PingPong)
                sign = 1.0f;
        }

        float t = 0.0f;
        if (lerpMethod == LerpMethod.Curve)
        {
            if (curve != null)
            {
                t = curve.Evaluate(time / duration);
            }
        }
        else if (lerpMethod == LerpMethod.Linear)
        {
            t = time / duration;
        }

        UpdateTween(t);
    }

    protected abstract void UpdateTween(float t);

    private void Update()
    {
        if (duration <= 0.0f)
            return;
        if (!enabled) return;
        if (delay > 0.0f)
        {
            delay -= Time.deltaTime;
            return;
        }

        UpdateTime();
    }

    public void PlayForward()
    {
        time = 0;
        sign = 1.0f;
        delay = stateDelay;
        if (!enabled) enabled = true;
        UpdateTime();
    }

    public void PlayReverse()
    {
        time = duration;
        sign = -1.0f;
        delay = stateDelay;
        if (!enabled) enabled = true;
        UpdateTime();
    }

    public TweenStyle style = TweenStyle.Once;
    public LerpMethod lerpMethod = LerpMethod.Curve;
    public AnimationCurve curve = new AnimationCurve(new Keyframe(0f, 0f, 0f, 1f), new Keyframe(1f, 1f, 1f, 0f));
    public float duration = 1.0f;
    public float stateDelay = 0.0f;
    public UnityEvent onFinished;

    float delay = 0.0f;
    float time = 0.0f;
    float sign = 1.0f;
}