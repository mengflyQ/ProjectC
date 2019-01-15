using System;
using System.Collections.Generic;

public class AtbTree
{
    public AtbTree()
    {
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

    // List<AtbNode> trees = new List<AtbNode>();
    Dictionary<AtbType, AtbNode> atbNodes = new Dictionary<AtbType, AtbNode>();

    public delegate void OnAtbChgFunc(AtbType atb, int oldValue, int newValue);
    public OnAtbChgFunc mOnAtbChg = null;

}