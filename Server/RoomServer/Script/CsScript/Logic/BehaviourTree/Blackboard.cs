using System;
using MathLib;
using System.Collections.Generic;

public class Blackboard
{
    public Variable SetVariableValue(string varName, VariableType type, object value)
    {
        Variable var = GetVariable(varName);
        bool notFind = (var == null);

        if (notFind)
        {
            var = Variable.CreateVariable(type);
            mVars.Add(varName, var);
        }
        var.Value = value;

        return var;
    }

    public object GetVariableValue(string varName)
    {
        Variable var = GetVariable(varName);
        if (var == null)
            return null;
        
        return var.Value;
    }

    public Variable GetVariable(string varName)
    {
        Variable var = null;
        if (mVars.TryGetValue(varName, out var))
        {
            return var;
        }
        return null;
    }

    public void AddVariable(Variable var)
    {
        if (var == null)
            return;
        if (mVars.ContainsKey(var.name))
        {
            return;
        }
        mVars.Add(var.name, var);
    }

    private Dictionary<string, Variable> mVars = new Dictionary<string, Variable>();

    public static string BuildinName_Target = "Target";
}