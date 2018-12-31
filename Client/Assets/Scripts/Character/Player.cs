using UnityEngine;

public class Player : Character
{
    protected override void Initialize()
    {
        base.Initialize();
        Type = CharacterType.Player;
    }

    private bool mIsControl = false;
    public bool IsControl
    {
        set
        {
            if (mIsControl == value)
                return;
            mIsControl = value;
        }
        get
        {
            return mIsControl;
        }
    }

    public excel_cha_class mChaClass = null;
}