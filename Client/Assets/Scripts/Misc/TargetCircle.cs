using UnityEngine;

public class TargetCircle : MonoBehaviour
{
    private void Update()
    {
        if (mTarget == null)
            return;
        transform.position = mTarget.transform.position;
    }

    public void SetTarget(Character target)
    {
        mTarget = target;
    }

    private Character mTarget = null;

    private static TargetCircle mInstance = null;
    public static TargetCircle Instance
    {
        get
        {
            if (mInstance == null)
            {
                GameObject targetCircleGO = ResourceSystem.Load<GameObject>("Particles/Prefabs/target_circle");
                if (targetCircleGO != null)
                {
                    targetCircleGO = GameObject.Instantiate(targetCircleGO);
                    mInstance = targetCircleGO.GetComponent<TargetCircle>();
                    targetCircleGO.transform.parent = null;
                    targetCircleGO.transform.position = Vector3.zero;
                    targetCircleGO.transform.rotation = Quaternion.identity;
                    targetCircleGO.transform.localScale = Vector3.one;
                }
            }
            return mInstance;
        }
    }
}