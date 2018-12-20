using System;
using System.Collections.Generic;
using UnityEngine;

public static class PickObject
{
    public static void Pick(Vector3 pos)
    {
        int layer = LayerMask.NameToLayer("Character");
        Ray ray = MobaMainCamera.MainCamera.ScreenPointToRay(pos);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, MobaMainCamera.MainCamera.farClipPlane, 1 << layer))
        {
            Transform parent = hit.transform.parent;
            if (parent == null)
                return;
            Character cha = parent.GetComponent<Character>();
            if (cha == null || cha == GameController.mMainPlayer)
                return;
            GameController.mMainPlayer.SetTarget(cha);
        }
    }
}
