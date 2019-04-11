using System;

//装饰器;
public class BTDecorator : BTBehavior
{
    public BTDecorator(Character self)
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
        child.Load(jsonBehavior);
    }

    public override void AddChild(BTBehavior child)
    {
        this.child = child;
    }

    protected BTBehavior child;
}