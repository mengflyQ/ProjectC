using UnityEngine;
using ProtoBuf;
using System.Collections.Generic;
using ZyGames.Framework.Common.Serialization;

public partial class Character : MonoBehaviour
{
    void InitGraphic()
    {
        mRenderers = GetComponentsInChildren<Renderer>();
    }

    float mAlpha = 1.0f;
    public float Alpha
    {
        set
        {
            if (mRenderers == null)
                return;
            for (int i = 0; i < mRenderers.Length; ++i)
            {
                Renderer r = mRenderers[i];
                if (r == null)
                    continue;
                Material[] ms = r.materials;
                for (int j = 0; j < ms.Length; ++j)
                {
                    Material m = ms[j];
                    if (m == null)
                        continue;
                    Color c = m.color;
                    c.a = value;
                    m.color = c;
                }
            }
            mAlpha = value;
        }
        get
        {
            return mAlpha;
        }
    }

    public Renderer[] mRenderers = null;
}
