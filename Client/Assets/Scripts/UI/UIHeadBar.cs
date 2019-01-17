using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum UIHpBarType
{
    Self,
    Friend,
    Enemy,
    Count
}

public class UIHeadBar : MonoBehaviour
{
    private void OnEnable()
    {
        for (int i = 0; i < (int)UIHpBarType.Count; ++i)
        {
            Image bar = hpBar[i];
            bar.gameObject.SetActive(false);
        }
    }

    private void FixedUpdate()
    {
        Player mainPlayer = GameController.mMainPlayer;
        if (mainPlayer == null)
            return;
        UpdateBarActive(UIHpBarType.Self, mainPlayer == Owner);
        UpdateBarActive(UIHpBarType.Friend, CampSystem.IsFriend(mainPlayer, Owner));
        UpdateBarActive(UIHpBarType.Enemy, CampSystem.IsEnemy(mainPlayer, Owner));

        int maxHP = Owner.GetAtb(AtbType.MaxHP);
        if (maxHP > 0)
        {
            float t = (float)Owner.HP / (float)maxHP;
            mCurHpBar.fillAmount = t;
        }
    }

    private void LateUpdate()
    {
        Matrix4x4 proj = WorldCamera.projectionMatrix;
        Matrix4x4 view = WorldCamera.worldToCameraMatrix;
        var viewProj = proj * view;
        Vector3 projPos = viewProj.MultiplyPoint(Hinge.position);

        proj = UICamera.projectionMatrix.inverse;
        view = UICamera.cameraToWorldMatrix;
        viewProj = view * proj;

        projPos = viewProj.MultiplyPoint(projPos);
        transform.position = projPos;
    }

    void UpdateBarActive(UIHpBarType barType, bool show)
    {
        Image bar = hpBar[(int)barType];
        if (show && !bar.gameObject.activeSelf)
        {
            bar.gameObject.SetActive(true);
            mCurHpBar = bar;
        }
        else if (!show && bar.gameObject.activeSelf)
        {
            bar.gameObject.SetActive(false);
        }
    }

    public Camera UICamera
    {
        set;
        get;
    }

    public Camera WorldCamera
    {
        set;
        get;
    }

    public Transform Hinge
    {
        set;
        get;
    }

    public Character Owner
    {
        set;
        get;
    }

    public Image expBar;
    public Text levelTxt;
    public Image[] hpBar;
    public Image mpBar;
    public Image seperator;

    private List<Image> seperators = new List<Image>();
    private List<Image> displaySeperators = new List<Image>();

    private Image mCurHpBar;
}