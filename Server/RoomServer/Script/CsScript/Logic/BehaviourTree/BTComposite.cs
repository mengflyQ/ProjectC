using System;
using System.Collections.Generic;

//复合节点基类;
public class BTComposite : BTBehavior
{
    public BTComposite(Character self)
        : base(self)
    {

    }

    public override void AddChild(BTBehavior child)
    {
        children.Add(child);
    }

    public override void Load(LitJson.JsonData json)
    {
        LitJson.JsonData jsonChildren = json["Children"];
        if (jsonChildren == null)
            return;
        children.Clear();
        for (int i = 0; i < jsonChildren.Count; ++i)
        {
            LitJson.JsonData jsonBehavior = jsonChildren[i];
            BTNodeType nodeType = (BTNodeType)jsonBehavior["Type"].AsInt;
            BTBehavior child = BTBehaviorTree.CreateBehavior(nodeType, self);
            children.Add(child);
        }
    }

    public void RemoveChild(BTBehavior child)
    {
        children.Remove(child);
    }

    public void ClearChild()
    {
        children.Clear();
    }

    protected List<BTBehavior> children = new List<BTBehavior>();
}