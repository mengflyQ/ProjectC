using System;
using System.Collections.Generic;
using GameServer.Model;
using ZyGames.Framework.Game.Contract;
using MathLibrary;

namespace GameServer.RoomServer
{
    public class Character
    {
        public virtual void Update()
        {
            if (Speed > 0.0f)
            {
                Postion += Time.DeltaTime * mDirection;
            }
        }

        public int UID
        {
            set;
            get;
        }

        public float Speed
        {
            set { mSpeed = value; }
            get { return mSpeed; }
        }

        public Vector3 Direction
        {
            set
            {
                mDirection = value;
                mDirection.y = 0.0f;
                mDirection.Normalize();
            }
            get
            {
                return mDirection;
            }
        }

        public Vector3 Postion
        {
            set
            {
                mPosition = value;
            }
            get
            {
                return mPosition;
            }
        }

        public string mNickName;
        public Scene mScene = null;
        public excel_cha_list mChaList = null;

        protected float mSpeed;
        protected Vector3 mDirection;
        protected Vector3 mPosition;
    }
}
