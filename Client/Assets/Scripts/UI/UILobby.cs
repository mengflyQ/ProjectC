using System;
using System.Collections.Generic;
using UnityEngine;

public class UILobby : MonoBehaviour
{
    void Awake()
    {
        Instance = this;
    }

    public UILobby Instance
    {
        private set;
        get;
    }
}
