using System;
using System.Collections.Generic;

public static class AtbCalcFuncRegister
{
    public static void DefaultCalcFunc(AtbNode node)
    {
        int value = 0;
        for (int i = 0; i < node.children.Count; ++i)
        {
            AtbNode child = node.children[i];
            excel_atb_data excel = child.excel;

            value += child.GetValue();
        }

        node.SetValue(value);
    }

    static AtbCalcFuncRegister()
    {

    }

    public static Dictionary<AtbType, AtbNode.CalcFunc> mFuncs = new Dictionary<AtbType, AtbNode.CalcFunc>();
}