using System;
using MathLib;
using LitJson;

public enum VariableType
{
    Bool,
    Int,
    Float,
    String,
    Vector2,
    Vector3,
    Vector4,
    Charactor,
    Player,
    Npc,
    NeutralNpc,
    Monster,
}

public class Variable
{
    public Variable(VariableType type)
    {
        this.type = type;
    }

    public static void LoadVariable(LitJson.JsonData data, Character self, Variable inVar, out Variable outVar)
    {
        outVar = null;
        string name = data["Name"].AsString;
        bool isValue = data["IsValue"].AsInt > 0;

        if (isValue)
        {
            outVar = inVar;
            inVar.Load(data);
        }
        else
        {
            if (self == null || self.mBehaviorTree == null)
                return;
            var blackboard = self.mBehaviorTree.LocalBlackboard;
            if (blackboard == null)
                return;
            Variable v = blackboard.GetVariable(name);
            outVar = v;
        }
    }

    public static void BlackboardLoadVariable(LitJson.JsonData data, ref Variable var)
    {
        var.Load(data);
    }

    protected virtual void Load(LitJson.JsonData data)
    {
        name = data["Name"].AsString;
        type = (VariableType)data["Type"].AsInt;
    }

    public static Variable CreateVariable(VariableType type)
    {
        switch (type)
        {
            case VariableType.Bool:
                return new VariableBool();
            case VariableType.Int:
                return new VariableInt();
            case VariableType.Float:
                return new VariableFloat();
            case VariableType.String:
                return new VariableString();
            case VariableType.Vector2:
                return new VariableVector2();
            case VariableType.Vector3:
                return new VariableVector3();
            case VariableType.Vector4:
                return new VariableVector4();
            case VariableType.Charactor:
                return new VariableCharactor();
            case VariableType.Player:
                return new VariablePlayer();
            case VariableType.Npc:
                return new VariableNpc();
            case VariableType.NeutralNpc:
                return new VariableNeutralNpc();
            case VariableType.Monster:
                return new VariableMonster();
        }
        return null;
    }

    public virtual object Value
    {
        set;
        get;
    }

    public string name;
	public VariableType type;
}

public class VariableFloat : Variable
{
    public VariableFloat()
        : base(VariableType.Float)
    {
        value = 0.0f;
    }

    protected override void Load(LitJson.JsonData data)
    {
        base.Load(data);

        value = data["Value"].AsFloat;
    }

    public override object Value
    {
        get
        {
            return this.value;
        }
        set
        {
            this.value = (float)value;
        }
    }

    public float value;
}

public class VariableInt : Variable
{
    public VariableInt()
        : base(VariableType.Int)
    {
        value = 0;
    }

    protected override void Load(LitJson.JsonData data)
    {
        base.Load(data);

        value = data["Value"].AsInt;
    }

    public override object Value
    {
        get
        {
            return this.value;
        }
        set
        {
            this.value = (int)value;
        }
    }

    public int value;
}

public class VariableBool : Variable
{
    public VariableBool()
        : base(VariableType.Bool)
    {

    }

    protected override void Load(LitJson.JsonData data)
    {
        base.Load(data);

        value = data["Value"].AsInt > 0;
    }

    public override object Value
    {
        get
        {
            return this.value;
        }
        set
        {
            this.value = (bool)value;
        }
    }

    public bool value;
}

public class VariableString : Variable
{
    public VariableString()
        : base(VariableType.String)
    {

    }

    protected override void Load(LitJson.JsonData data)
    {
        base.Load(data);

        value = data["Value"].AsString;
    }

    public override object Value
    {
        get
        {
            return this.value;
        }
        set
        {
            this.value = (string)value;
        }
    }

	public string value;
}

public class VariableVector2 : Variable
{
    public VariableVector2()
        : base(VariableType.Vector2)
    {

    }

    protected override void Load(LitJson.JsonData data)
    {
        base.Load(data);

        JsonData v = data["Value"];
        value.x = v[0].AsFloat;
        value.y = v[1].AsFloat;
    }

    public override object Value
    {
        get
        {
            return this.value;
        }
        set
        {
            this.value = (Vector2)value;
        }
    }

    public Vector2 value;
}

public class VariableVector3 : Variable
{
    public VariableVector3()
        : base(VariableType.Vector3)
    {

    }

    protected override void Load(LitJson.JsonData data)
    {
        base.Load(data);

        JsonData v = data["Value"];
        value.x = v[0].AsFloat;
        value.y = v[1].AsFloat;
        value.z = v[2].AsFloat;
    }

    public override object Value
    {
        get
        {
            return this.value;
        }
        set
        {
            this.value = (Vector3)value;
        }
    }

    public Vector3 value;
}

public class VariableVector4 : Variable
{
    public VariableVector4()
        : base(VariableType.Vector4)
    {

    }

    protected override void Load(LitJson.JsonData data)
    {
        base.Load(data);

        JsonData v = data["Value"];
        value.x = v[0].AsFloat;
        value.y = v[1].AsFloat;
        value.z = v[2].AsFloat;
        value.w = v[3].AsFloat;
    }

    public override object Value
    {
        get
        {
            return this.value;
        }
        set
        {
            this.value = (Vector4)value;
        }
    }

    public Vector4 value;
}

public class VariableCharactor : Variable
{
    public VariableCharactor()
        : base(VariableType.Charactor)
    {

    }

    protected override void Load(LitJson.JsonData data)
    {
        base.Load(data);

        gid = data["Value"].AsInt;
    }

    public override object Value
    {
        get
        {
            return this.gid;
        }
        set
        {
            gid = (int)value;
        }
    }

    public int gid;
}

public class VariablePlayer : Variable
{
    public VariablePlayer()
        : base(VariableType.Player)
    {

    }

    public override void Load(LitJson.JsonData data)
    {
        base.Load(data);

        gid = data["Value"].AsInt;
    }

    public override object Value
    {
        get
        {
            return this.gid;
        }
        set
        {
            gid = (int)value;
        }
    }

    public int gid;
}

public class VariableNpc : Variable
{
    public VariableNpc()
        : base(VariableType.Npc)
    {

    }

    public override void Load(LitJson.JsonData data)
    {
        base.Load(data);

        gid = data["Value"].AsInt;
    }

    public override object Value
    {
        get
        {
            return this.gid;
        }
        set
        {
            gid = (int)value;
        }
    }

    public int gid;
}

public class VariableNeutralNpc : Variable
{
    public VariableNeutralNpc()
        : base(VariableType.NeutralNpc)
    {

    }

    public override void Load(LitJson.JsonData data)
    {
        base.Load(data);

        gid = data["Value"].AsInt;
    }

    public override object Value
    {
        get
        {
            return this.gid;
        }
        set
        {
            gid = (int)value;
        }
    }

    public int gid;
}

public class VariableMonster : Variable
{
    public VariableMonster()
        : base(VariableType.Monster)
    {

    }

    public override void Load(LitJson.JsonData data)
    {
        base.Load(data);

        gid = data["Value"].AsInt;
    }

    public override object Value
    {
        get
        {
            return this.gid;
        }
        set
        {
            gid = (int)value;
        }
    }

    public int gid;
}