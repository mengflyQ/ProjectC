using UnityEngine;

public class NPC : Character
{
    protected override void Initialize()
    {
        base.Initialize();
        Type = CharacterType.NPC;
    }

    public override void Destroy()
    {
        base.Destroy();

        Scene curScn = SceneSystem.Instance.mCurrentScene;
        if (curScn != null)
        {
            curScn.DelNPC(this);
        }
        Destroy(gameObject);
    }
}
