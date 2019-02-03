using System;
using MathLib;

public class Transform
{
    private Vector3 mForward = Vector3.forward;
    public Vector3 forward
    {
        set
        {
            mForward = value;
            mForward.y = 0.0f;
            mForward.Normalize();
        }
        get
        {
            return mForward;
        }
    }

    public Vector3 right
    {
        get
        {
            Vector3 v = Vector3.Cross(Vector3.up, forward);
            return v;
        }
    }

    public Vector3 up
    {
        get
        {
            return Vector3.up;
        }
    }

    public Transform parent;
    public Vector3 position;
    
}