using System;
using System.Collections.Generic;

public class AtbNode
{
    public AtbNode(excel_atb_data excel, AtbTree tree)
    {
        AtbType = (AtbType)excel.id;
        this.excel = excel;
        value = excel.defValue;
        atbTree = tree;
    }

    public void UpdateAtbTreeDown()
    {
        Stack<AtbNode> nodes = new Stack<AtbNode>();

        Queue<AtbNode> qChildrens = new Queue<AtbNode>();
        qChildrens.Enqueue(this);
        while (qChildrens.Count > 0)
        {
            AtbNode node = qChildrens.Dequeue();

            nodes.Push(node);

            for (int i = 0; i < node.children.Count; ++i)
            {
                AtbNode child = node.children[i];
                if (!child.Dirty)
                    continue;
                qChildrens.Enqueue(child);
            }
        }

        while (nodes.Count > 0)
        {
            AtbNode node = nodes.Pop();
            if (node == null)
                continue;
            if (node.Dirty)
            {
                if (mCalcFunc != null)
                {
                    mCalcFunc(node);
                }
                node.Dirty = false;
            }
        }
    }

    public void UpdateAtbTreeUp()
    {
        if (!Dirty)
        {
            return;
        }
        Queue<AtbNode> qParents = new Queue<AtbNode>();
        qParents.Enqueue(this);
        while (qParents.Count > 0)
        {
            AtbNode node = qParents.Dequeue();

            if (node.Dirty)
            {
                if (mCalcFunc != null)
                {
                    mCalcFunc(node);
                }
                node.Dirty = false;
            }

            for (int i = 0; i < node.children.Count; ++i)
            {
                AtbNode child = node.parents[i];
                if (!child.Dirty)
                    continue;
                qParents.Enqueue(child);
            }
        }
    }

    public int GetValue()
    {
        return value;
    }

    public void SetValue(int value)
    {
        int oldValue = this.value;
        this.value = Clamp(value);
        if (oldValue != this.value)
        {
            if (atbTree.mOnAtbChg != null)
            {
                atbTree.mOnAtbChg(AtbType, oldValue, value);
            }
        }
    }

    public int Clamp(int value)
    {
        if (maxNode != null && value > maxNode.value)
            value = maxNode.value;
        if (excel != null)
        {
            if (value > excel.maxValue)
                value = excel.maxValue;
            else if (value < excel.minValue)
                value = excel.minValue;
        }
        return value;
    }

    private bool dirty = true;
    public bool Dirty
    {
        set
        {
            dirty = value;
            if (dirty)
            {
                Queue<AtbNode> qParents = new Queue<AtbNode>();
                qParents.Enqueue(this);
                while (qParents.Count > 0)
                {
                    AtbNode top = qParents.Dequeue();

                    top.dirty = true;

                    for (int i = 0; i < top.parents.Count; ++i)
                    {
                        AtbNode parent = top.parents[i];
                        qParents.Enqueue(parent);
                    }
                }
            }
        }
        get
        {
            return dirty;
        }
    }

    public AtbType AtbType
    {
        private set;
        get;
    }

    public delegate void CalcFunc(AtbNode node);
    public CalcFunc mCalcFunc;
    public excel_atb_data excel;
    private int value = 0;
    public List<AtbNode> parents = new List<AtbNode>();
    public List<AtbNode> children = new List<AtbNode>();
    public AtbNode maxNode = null;
    public AtbTree atbTree = null;
}