using System;

enum BTCompareOp
{
    Equal,
    NotEqual,
    Less,
    LessEqual,
    More,
    MoreEqual
};

public class BTCompare : BTCondition
{
    public BTCompare(Character self)
        : base(self)
    {

    }

    public override void Load(LitJson.JsonData json)
    {
        base.Load(json);

        Variable.LoadVariable(json["V1"], self, v1, out v1);
        Variable.LoadVariable(json["V2"], self, v2, out v2);
        op = (BTCompareOp)json["Op"].AsInt;
    }

    public Variable v1;
    public Variable v2;
    public BTCompareOp op;
}