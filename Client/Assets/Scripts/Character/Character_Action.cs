using UnityEngine;

public partial class Character : MonoBehaviour
{
    void LogicTickAction()
    {
        mContainer.LogicTick();
    }

    void RenderTickAction()
    {
        mContainer.RenderTick();
    }

    public void AddAction(IAction action)
    {
        mContainer.AddAction(action);
    }

    public void DelAction(ChaActionType type)
    {
        mContainer.DelAction(type);
    }

    public IAction GetAcion(ChaActionType type)
    {
        return mContainer.GetAction(type);
    }

    public bool HasAction(ChaActionType type)
    {
        return mContainer.HasAction(type);
    }

    ActionContainer mContainer = new ActionContainer();
}