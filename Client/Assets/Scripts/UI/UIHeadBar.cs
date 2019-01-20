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

    struct HeadTextTweenCB
    {
        public void OnTweenFinish()
        {
            tween.transform.parent = uiRoot.mHeadTextPool.transform;
            uiRoot.mHeadTextPool.Despawn(tween.transform);
            tween.onFinished.RemoveAllListeners();
        }

        public TweenBase tween;
        public UIRoot3D uiRoot;
    }

    public void CreateHeadText(HPChgType hurtType, int hurt)
    {
        Transform t = UIRoot.mHeadTextPool.Spawn(UIRoot.headTextTransform);
        t.parent = transform;
        t.localPosition = Vector3.zero;
        t.localRotation = Quaternion.identity;
        t.localScale = new Vector3(0.3f, 0.3f, 0.3f);

        TweenBase tween = t.GetComponent<TweenBase>();
        tween.PlayForward();
        if (tween.onFinished == null)
        {
            tween.onFinished = new UnityEngine.Events.UnityEvent();
        }
        HeadTextTweenCB cb = new HeadTextTweenCB();
        cb.tween = tween;
        cb.uiRoot = UIRoot;
        tween.onFinished.AddListener(cb.OnTweenFinish);

        Text text = t.GetComponent<Text>();
        if (hurtType == HPChgType.PhyDamage)
        {
            text.color = Color.red;
        }
        else if (hurtType == HPChgType.MagDamage)
        {
            text.color = new Color(0.8f, 0.0f, 1.0f);
        }
        else if (hurtType == HPChgType.Cure)
        {
            text.color = Color.green;
        }
        text.text = string.Format("{0}", hurt);
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

    public UIRoot3D UIRoot
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