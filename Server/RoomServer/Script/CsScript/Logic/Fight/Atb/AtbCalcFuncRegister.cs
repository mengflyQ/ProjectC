using System;
using System.Collections.Generic;

public static class AtbCalcFuncRegister
{
    public static void DefaultCalcFunc(AtbNode node)
    {
        int value = node.GetValue();
        for (int i = 0; i < node.children.Count; ++i)
        {
            if (i == 0)
            {
                value = 0;
            }
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