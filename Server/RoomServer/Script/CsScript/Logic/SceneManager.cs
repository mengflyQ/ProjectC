using System;
using System.Collections.Generic;
using ZyGames.Framework.Common.Timing;
using LitJson;

namespace GameServer.RoomServer
{
    public class SceneManager : BaseSystem
    {
        void Initialize()
        {
            BaseSystem.StartTick(mInstance);
        }

        public void RemoveRoom(Scene room)
        {
            mScenes.Remove(room);
        }

        public int GenSceneID()
        {
            int cid = 0;
            for (int i = 0; i < mScenes.Count; ++i)
            {
                Scene r = mScenes[i];
                if (cid != r.ScnUID)
                {
                    return cid;
                }
                else
                {
                    ++cid;
                }
            }
            return -1;
        }

        protected override void Tick()
        {
            for (int i = mScenes.Count - 1; i >= 0; --i)
            {
                Scene scene = mScenes[i];
                if (scene.IsVanish)
                {
                    int lastIndex = mScenes.Count - 1;
                    Scene tmp = scene;
                    mScenes[i] = mScenes[lastIndex];
                    mScenes[lastIndex] = scene;
                    mScenes.RemoveAt(lastIndex);
                    mScenes.Sort(CompareScene);
                    continue;
                }
                scene.Tick();
            }
        }

        int CompareScene(Scene r1, Scene r2)
        {
            return r1.ScnUID.CompareTo(r2.ScnUID);
        }

        Scene BinarySearchScene(int low, int high, int id)
        {
            Scene highScene = mScenes[high];
            Scene lowScene = mScenes[low];

            int mid = (low + high) / 2;

            if (lowScene.ScnUID <= highScene.ScnUID)
            {
                Scene midScene = mScenes[mid];
                if (midScene.ScnUID == id)
                    return midScene;
                else if (midScene.ScnUID > id)
                    return BinarySearchScene(low, mid - 1, id);
                else
                    return BinarySearchScene(mid + 1, high, id);
            }
            return null;
        }

        public Scene FindScene(int scnUID)
        {
            if (mScenes == null || mScenes.Count <= 0)
                return null;

            return BinarySearchScene(0, mScenes.Count - 1, scnUID);
        }

        static SceneManager mInstance = null;
        public static SceneManager Instance
        {
            get
            {
                if (mInstance == null)
                {
                    mInstance = new SceneManager();
                    Instance.Initialize();
                }
                return mInstance;
            }
        }
        public List<Scene> mScenes = new List<Scene>();
    }
}