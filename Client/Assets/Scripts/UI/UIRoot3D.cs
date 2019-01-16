using System;
using System.Collections.Generic;
using UnityEngine;

public class UIRoot3D : MonoBehaviour
{
    private void Awake()
    {
        MessageSystem.Instance.MsgRegister(MessageType.InitHeadBar, OnInitHeadBar);
    }

    private void OnDestroy()
    {
        MessageSystem.Instance.MsgUnregister(MessageType.InitHeadBar, OnInitHeadBar);
    }

    void OnInitHeadBar(params object[] param)
    {
        Character cha = param[0] as Character;

        ResourceSystem.LoadAsync<GameObject>("GUI/UI_HeadBar", (o) =>
        {
            GameObject go = GameObject.Instantiate(o) as GameObject;
            if (go == null)
                return;
            go.transform.parent = transform;
            go.transform.localPosition = Vector3.zero;
            go.transform.localRotation = Quaternion.identity;
            go.transform.localScale = new Vector3(0.6f, 0.6f, 0.6f);

            UIHeadBar headBar = go.GetComponent<UIHeadBar>();
            if (headBar == null)
                return;
            headBar.UICamera = uiCamera;
            if (MobaMainCamera.MainCamera != null)
            {
                headBar.WorldCamera = MobaMainCamera.MainCamera;
            }
            headBar.Owner = cha;

            if (cha == null || cha.HingePoints == null)
                return;
            Transform hinge = cha.HingePoints.GetHingeName("HeadBar");
            if (hinge == null)
                return;
            headBar.Hinge = hinge;

            cha.headBar = headBar;
        });
    }

    public Camera uiCamera;
}