using UnityEngine;

public class NPC : Character
{
    protected override void Initialize()
    {
        base.Initialize();
        Type = CharacterType.NPC;
    }
}
