using UnityEngine;

public class Player : Character
{
    protected override void Initialize()
    {
        base.Initialize();
        Type = CharacterType.Player;
    }

    public override void SetCannotFlag(CannotFlag flag, OptType type, bool cannot)
    {
        if (flag == CannotFlag.CannotControl && cannot)
        {
            IsControl = false;
        }
        base.SetCannotFlag(flag, type, cannot);
    }

    private bool mIsControl = false;
    public bool IsControl
    {
        set
        {
            if (mIsControl == value)
                return;
            mIsControl = value;
            if (mIsControl && mCurSkill != null)
            {
                mCurSkill.OnMove();
            }
        }
        get
        {
            return mIsControl;
        }
    }

    public excel_cha_class mChaClass = null;
}