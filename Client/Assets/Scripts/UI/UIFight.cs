using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIFight : MonoBehaviour
{
    void Awake()
    {
        
    }

    void Start()
    {
        MessageSystem.Instance.MsgRegister(MessageType.OnSetChaClass, OnSetChaClass);
	}

    private void OnDestroy()
    {
        
    }

    void Update()
    {
		
	}

    void OnSetChaClass(params object[] param)
    {
        excel_cha_class chaClass = param[0] as excel_cha_class;
        if (chaClass == null)
            return;
        if (btnTriggers == null)
            return;
        for (int i = 0; i < chaClass.skillIDs.Length && i < btnTriggers.Length; ++i)
        {
            int skillID = chaClass.skillIDs[i];
            excel_skill_list skillList = excel_skill_list.Find(skillID);
            if (skillList == null)
            {
                Debug.LogError("UIFight技能初始化失败，未找到ID为" + skillID + "的技能数据");
                continue;
            }

            EventTriggerListener listener = btnTriggers[i];

            GameObject go = listener.gameObject;
            SkillBtnData skillBtnData = new SkillBtnData();
            Transform joystick = go.transform.Find("Joystick");
            if (joystick != null)
            {
                skillBtnData.joystick = joystick.transform;
            }
            Transform joystickBG = go.transform.Find("Joystick_BG");
            if (joystickBG != null)
            {
                skillBtnData.joystickBG = joystickBG.gameObject;
            }
            skillBtnData.btnImage = go.GetComponent<Image>();
            skillBtnData.maxRadius = 64.0f;
            skillBtnData.opType = (SkillPreOpType)skillList.skillPreOpType;
            skillBtnData.opData1 = skillList.skillPreOpData1;
            skillBtnData.opData2 = skillList.skillPreOpData2;
            skillBtnData.skillID = skillID;
            skillDatas.Add(go, skillBtnData);
            
            listener.onDrag += OnSkillBtnDrag;
            listener.onPointerDown += OnSkillBtnPress;
            listener.onPointerUp += OnSkillBtnRelease;
        }
    }

    public void DoSkill(int skillID, bool autoTarget, Vector3 targetPos)
    {
        Player player = GameController.mMainPlayer;
        if (player == null)
            return;
        if (player.IsCannotFlag(CannotFlag.CannotSkill))
            return;
        Character target = player.GetTarget();
        if (GameApp.Instance.directGame)
        {
            SkillHandle handle = new SkillHandle();
            handle.skillID = skillID;
            handle.caster = player;
            handle.autoTargetPos = autoTarget;
            handle.targetPos = targetPos;
            handle.skillTargetID = 0;
            SkillHandle.UseSkill(handle);
        }
        else
        {
            ReqSkill reqSkill = new ReqSkill();
            reqSkill.skillID = skillID;
            reqSkill.position = Vector3Packat.FromVector3(player.Position);
            reqSkill.direction = Vector3Packat.FromVector3(player.Direction);
            reqSkill.targetID = target == null ? 0 : target.UserID;
            reqSkill.autoTargetPos = autoTarget;
            reqSkill.targetPos = Vector3Packat.FromVector3(targetPos);
            NetWork.SendPacket<ReqSkill>(CTS.CTS_SkillReq, reqSkill, null);
        }
    }

    Vector3 GetPointPos()
    {
        Vector3 touchPos = Input.mousePosition;
        if (Input.touchCount > 0)
        {
            touchPos = Input.touches[Input.touchCount - 1].position;
        }
        return touchPos;
    }

    // 技能图标开始拖拽（按下）;
    private void OnSkillBtnPress(GameObject btn)
    {
        SkillBtnData data = null;
        if (!skillDatas.TryGetValue(btn, out data))
        {
            return;
        }
        data.btnImage.enabled = false;
        data.joystickBG.SetActive(true);
        if (data.opType != SkillPreOpType.TargetDirLine
            && data.opType != SkillPreOpType.TargetDirFan
            && data.opType != SkillPreOpType.TargetPos)
        {
            return;
        }
        data.joystick.gameObject.SetActive(true);

        Vector3 touchPos = GetPointPos();
        data.joystick.position = touchPos;
        data.startPos = new Vector2(touchPos.x, touchPos.y);

        Player player = GameController.mMainPlayer;
        if (player == null)
            return;
        if (data.opType == SkillPreOpType.TargetDirLine)
        {
            float dist = data.opData1 * 0.001f;
            float width = data.opData2 * 0.001f;
            SkillWarning.CreateSkillWarning("GUI/SkillWarning/Prefabs/warning_line", SkillWarningType.PreOpRect, width, dist,
                -1.0f, player.Position, player.transform.rotation, true, player.transform, (o) =>
                {
                    o.NavLayer = player.NavLayer;
                    data.warning = o;
                });
        }
        else if (data.opType == SkillPreOpType.TargetDirFan)
        {
            float dist = data.opData1 * 0.001f;
            float angle = (float)data.opData2;
            SkillWarning.CreateSkillWarning("GUI/SkillWarning/Prefabs/warning_fan", SkillWarningType.PreOpFan, dist, angle,
                -1.0f, player.Position, player.transform.rotation, true, player.transform, (o) =>
                {
                    o.NavLayer = player.NavLayer;
                    data.warning = o;
                });
        }
        else if (data.opType == SkillPreOpType.TargetPos)
        {
            float radius = data.opData1 * 0.001f;
            SkillWarning.CreateSkillWarning("GUI/SkillWarning/Prefabs/warning_circle", SkillWarningType.PreOpCircle, radius, radius,
                -1.0f, player.Position, player.transform.rotation, true, player.transform, (o) =>
                {
                    o.NavLayer = player.NavLayer;
                    data.warning = o;
                });
        }
    }

    // 技能图标开始拖拽（放开）;
    private void OnSkillBtnRelease(GameObject btn)
    {
        SkillBtnData data = null;
        if (!skillDatas.TryGetValue(btn, out data))
        {
            return;
        }
        data.joystickBG.SetActive(false);
        data.btnImage.enabled = true;
        if (data.opType == SkillPreOpType.Click)
        {
            DoSkill(data.skillID, true, Vector3.zero);
        }
        if (data.opType != SkillPreOpType.TargetDirLine
            && data.opType != SkillPreOpType.TargetDirFan
            && data.opType != SkillPreOpType.TargetPos)
        {
            return;
        }
        data.joystick.gameObject.SetActive(false);

        Vector3 pos = data.joystick.position;
        Vector3 btnPos = btn.transform.position;
        Vector2 endPos = new Vector2(pos.x, pos.y);
        Vector3 delta = new Vector3(pos.x - btnPos.x, 0.0f, pos.y - btnPos.y);
        
        data.joystick.transform.localPosition = Vector3.zero;

        if (endPos == data.startPos)
        {
            return;
        }
        data.startPos = Vector2.zero;
        Player player = GameController.mMainPlayer;
        if (player == null)
            return;

        Quaternion rot = MobaMainCamera.MainCamera.transform.rotation;
        Vector3 dir = delta;
        dir = rot* dir;
        dir.y = 0.0f;
        dir.Normalize();

        if (data.opType == SkillPreOpType.TargetDirLine
            || data.opType == SkillPreOpType.TargetDirFan)
        {
            float dist = data.opData1 * 0.001f;
            Vector3 v = player.Position;
            v += dist * dir;
            DoSkill(data.skillID, false, v);

            if (data.warning != null)
            {
                data.warning.Release();
            }
        }
        else if (data.opType == SkillPreOpType.TargetPos)
        {
            float dist = data.opData1 * 0.001f;
            float r = delta.magnitude;
            float t = r / data.maxRadius;
            Vector3 v = player.Position;
            dist = dist * t;
            v += dist * dir;
            DoSkill(data.skillID, false, v);

            if (data.warning != null)
            {
                data.warning.Release();
            }
        }
    }

    // 技能图标拖拽中;
    private void OnSkillBtnDrag(GameObject btn)
    {
        SkillBtnData data = null;
        if (!skillDatas.TryGetValue(btn, out data))
        {
            return;
        }
        if (data.opType != SkillPreOpType.TargetDirLine
            && data.opType != SkillPreOpType.TargetDirFan
            && data.opType != SkillPreOpType.TargetPos)
        {
            return;
        }
        Vector3 touchPos = GetPointPos();
        Vector3 pos = data.joystick.position;
        pos.x = touchPos.x;
        pos.y = touchPos.y;

        Vector3 btnPos = btn.transform.position;
        Vector2 delta = new Vector2(pos.x - btnPos.x, pos.y - btnPos.y);
        
        float r = delta.magnitude;
        if (r >= data.maxRadius)
        {
            Vector2 dir2D = delta.normalized;
            Vector2 finalPos = dir2D * data.maxRadius;
            pos.x = btnPos.x + finalPos.x;
            pos.y = btnPos.y + finalPos.y;
        }

        data.joystick.position = pos;

        Player player = GameController.mMainPlayer;
        if (player == null)
            return;

        Quaternion rot = MobaMainCamera.MainCamera.transform.rotation;
        Vector3 dir = new Vector3(delta.x, 0.0f, delta.y);
        dir = rot * dir;
        dir.y = 0.0f;
        dir.Normalize();

        if (data.opType == SkillPreOpType.TargetDirLine
            || data.opType == SkillPreOpType.TargetDirFan)
        {
            float dist = data.opData1 * 0.001f;
            Vector3 v = player.Position;
            v += dist * dir;

            if (data.warning != null)
            {
                data.warning.transform.forward = dir;
            }
        }
        else if (data.opType == SkillPreOpType.TargetPos)
        {
            float dist = data.opData1 * 0.001f;
            float t = r / data.maxRadius;
            dist = dist * t;
            Vector3 v = player.Position;
            v += dist * dir;

            if (data.warning != null)
            {
                data.warning.transform.position = v;
            } 
        }
    }

    public class SkillBtnData
    {
        public Image btnImage;
        public Transform joystick;
        public GameObject joystickBG;
        public SkillPreOpType opType;
        public int opData1;
        public int opData2;
        public float maxRadius;
        public int skillID;
        public Vector2 startPos;
        public SkillWarning warning;
    }
    
    public EventTriggerListener[] btnTriggers;
    public Dictionary<GameObject, SkillBtnData> skillDatas = new Dictionary<GameObject, SkillBtnData>();

}
