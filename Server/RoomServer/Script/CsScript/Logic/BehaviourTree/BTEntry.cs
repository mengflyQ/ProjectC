using System;

public class BTEntry : BTBehavior
{
    public BTEntry(Character self)
        : base(self)
    {

    }

    public override void Load(LitJson.JsonData json)
    {
        LitJson.JsonData jsonChildren = json["Children"];
        if (jsonChildren == null || jsonChildren.Count != 1)
            return;
        LitJson.JsonData jsonBehavior = jsonChildren[0];
        if (jsonBehavior == null)
            return;

        BTNodeType nodeType = (BTNodeType)jsonBehavior["Type"].AsInt;
        child = BTBehaviorTree.CreateBehavior(nodeType, self);
    }

    protected override BTStatus Update()
    {
        if (child != null)
        {
            return child.Tick();
        }
        return BTStatus.Invalid;
    }

    BTBehavior child = null;
}