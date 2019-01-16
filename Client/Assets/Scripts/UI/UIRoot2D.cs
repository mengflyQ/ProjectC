using System;
using System.Collections.Generic;
using UnityEngine;

public class UIRoot2D : MonoBehaviour
{
    private void Awake()
    {
        Instance = this;
    }

    public static UIRoot2D Instance = null;
}