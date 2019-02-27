using System;
using System.Collections.Generic;
using GameServer.RoomServer;

public class AtbTree
{
    public AtbTree(Character cha)
    {
        mCharacter = cha;

        List<AtbNode> nodes = new List<AtbNode>();
        List<bool> token = new List<bool>();
        for (int i = 0; i < excel_atb_data.Count; ++i)
        {
            excel_atb_data excel = excel_atb_data.GetByIndex(i);
            AtbNode node = new AtbNode(excel, this);

            AtbNode.CalcFunc func = null;
            if (AtbCalcFuncRegister.mFuncs.TryGetValue(node.AtbType, out func))
            {
                node.mCalcFunc = func;
            }
            else
            {
                node.mCalcFunc = AtbCalcFuncRegister.DefaultCalcFunc;
            }

            atbNodes.Add(node.AtbType, node);
            nodes.Add(node);
            token.Add(false);
        }

        // Build Tree
        for (int i = 0; i < nodes.Count; ++i)
        {
            AtbNode node = nodes[i];
            if (node.excel.inflAtbs == null)
                continue;
            for (int j = 0; j < node.excel.inflAtbs.Length; ++j)
            {
                int inflID = node.excel.inflAtbs[j];
                AtbType childAtb = (AtbType)inflID;

                // 获取被影响节点;
                AtbNode inflNode = null;
                if (!atbNodes.TryGetValue(childAtb, out inflNode))
                {
                    continue;
                }
                inflNode.children.Add(node);
                node.parents.Add(inflNode);
            }
        }

        // Update Tree
        for (int i = 0; i < nodes.Count; ++i)
        {
            AtbNode node = nodes[i];
            if (node.parents.Count == 0)
            {
                node.UpdateAtbTreeDown();
            }
        }
    }

    public void SetAtb(AtbType atb, int value)
    {
        AtbNode node = null;
        if (!atbNodes.TryGetValue(atb, out node))
        {
            return;
        }
        int oldValue = node.GetValue();
        if (oldValue == value)
            return;

        node.SetValue(value);
        node.Dirty = true;
        node.UpdateAtbTreeUp();

        mCharacter.OnAtbChg(atb, oldValue, value);
    }

    public int GetAtb(AtbType atb)
    {
        AtbNode node = null;
        if (!atbNodes.TryGetValue(atb, out node))
        {
            return 0;
        }
        return node.GetValue();
    }

    public float GetAtbPct(AtbType atb)
    {
        AtbNode node = null;
        if (!atbNodes.TryGetValue(atb, out node))
        {
            return 0.0f;
        }
        int v = node.GetValue();
        return (float)v * 0.0001f;
    }

    void OnAtbChg(AtbType atb, int oldValue, int newValue)
    {

    }

    public void PacketToMsg()
    {
        if (mAtbMsgAround == null)
        {
            mAtbMsgAround = new NotifyAtb();
            mAtbMsgAround.uid = mCharacter.gid;
        }
        if (mAtbMsgSelf == null)
        {
            mAtbMsgSelf = new NotifyAtb();
            mAtbMsgSelf.uid = mCharacter.gid;
        }
        foreach (var kv in atbNodes)
        {
            AtbNode node = kv.Value;
            if (node == null)
                return;
            node.PacketToMsg();
        }
    }

    public void Update()
    {
        if (mAtbMsgSelf != null)
        {
            Player self = null;
            if (mCharacter.Type != CharacterType.Player)
            {
                mAtbMsgSelf = null;
                return;
            }
            self = mCharacter as Player;
            mAtbMsgSelf.uid = mCharacter.gid;
            NetWork.NotifyMessage<NotifyAtb>(self.UserID, STC.STC_AtbNotify, mAtbMsgSelf);
            mAtbMsgSelf = null;
        }
        if (mAtbMsgAround != null)
        {
            Scene scn = mCharacter.mScene;
            if (scn == null)
            {
                mAtbMsgAround = null;
                return;
            }
            mAtbMsgAround.uid = mCharacter.gid;
            for (int i = 0; i < scn.GetPlayerCount(); ++i)
            {
                Player p = scn.GetPlayerByIndex(i);
                NetWork.NotifyMessage<NotifyAtb>(p.UserID, STC.STC_AtbNotify, mAtbMsgAround);
            }
            mAtbMsgAround = null;
        }
    }

    // List<AtbNode> trees = new List<AtbNode>();
    Dictionary<AtbType, AtbNode> atbNodes = new Dictionary<AtbType, AtbNode>();

    public delegate void OnAtbChgFunc(AtbType atb, int oldValue, int newValue);
    public OnAtbChgFunc mOnAtbChg = null;

    private Character mCharacter = null;

    public NotifyAtb mAtbMsgSelf = null;
    public NotifyAtb mAtbMsgAround = null;
}