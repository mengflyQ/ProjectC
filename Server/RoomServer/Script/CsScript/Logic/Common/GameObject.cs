using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class GameObject
{
    public GameObject()
    {
        gameObjects.Add(this);
    }

    public virtual void Destroy()
    {
        Destroy(this);
    }

    public static void Destroy(GameObject go)
    {
        gameObjects.Remove(go);
    }

    public virtual void Update() { }

    public static void DoTick()
    {
        for (int i = 0; i < gameObjects.Count; ++i)
        {
            GameObject go = gameObjects[i];
            if (go == null)
                continue;
            go.Update();
        }
    }

    public Transform transform = new Transform();
    public static List<GameObject> gameObjects = new List<GameObject>();
}