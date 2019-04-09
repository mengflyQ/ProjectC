using System;
using LitJson;
using System.IO;

public class BTBehaviorTree
{
    public BTBehaviorTree(Character self)
    {
        root = new BTEntry(self);
        this.self = self;
        LocalBlackboard = new Blackboard();
    }

    void LoadFromFile(string filePath)
    {
        string json = string.Empty;
        using (FileStream file = new FileStream(filePath, FileMode.Open))
        {
            int fsLength = (int)file.Length;
            byte[] buffer = new byte[fsLength];
            file.Read(buffer, 0, buffer.Length);
            json = System.Text.Encoding.UTF8.GetString(buffer);
            file.Close();
        }
        JsonData root = JsonMapper.ToObject(json);

        JsonData vars = root["Vars"];
        JsonData enterNode = root["Nodes"];

        for (int i = 0; i < vars.Count; ++i)
        {
            JsonData jsonVar = vars[i];
            VariableType varType = (VariableType)jsonVar["Type"].AsInt;
            Variable var = Variable.CreateVariable(varType);
            var.Load(jsonVar);
            LocalBlackboard.AddVariable(var);
        }

        BTNodeType nodeType = (BTNodeType)enterNode["Type"].AsInt;

        BTBehavior node = BTBehaviorTree.CreateBehavior(nodeType, self);
        node.Load(enterNode);
    }

    public static BTBehavior CreateBehavior(BTNodeType type, Character self)
    {
        switch (type)
        {
            case BTNodeType.Entry:
                return new BTEntry(self);
            case BTNodeType.Selector:
                return new BTSelector(self);
            case BTNodeType.ActiveSelector:
                return new BTActiveSelector(self);
            case BTNodeType.Sequence:
                return new BTSequence(self);
            case BTNodeType.Parallel:
                return new BTParallel(self);
            case BTNodeType.Repeat:
                return new BTRepeat(self);
        }
        return null;
    }

    public void Update()
    {
        root.Tick();
    }

    public Blackboard LocalBlackboard
    {
        set;
        get;
    }

    private BTBehavior root;
    private Character self;
}