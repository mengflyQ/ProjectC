using System;
using LitJson;
using GameServer.Model;
using System.Collections.Generic;
using ZyGames.Framework.Game.Service;
using ZyGames.Framework.Game.Contract;
using ZyGames.Framework.Cache.Generic;
using ZyGames.Framework.Common.Serialization;


namespace GameServer.LoginServer
{
    public class NetWorkRegister
    {
        static void OnLogin(byte[] data, Action5001 action)
        {
            LoginData loginData = null;
            loginData = ProtoBufUtils.Deserialize<LoginData>(data);

            var cache = CacheSet.UserInfoCach;

            var listUser = cache.FindAll(t => t.Account == loginData.UserName);
            UserInfo userInfo = null;
            if (listUser.Count >= 1)
                userInfo = listUser[0];

            bool regist = false;
            LoginResponse response = null;

            if (userInfo == null)
            {
                userInfo = new UserInfo();
                userInfo.UserId = (uint)cache.GetNextNo();
                userInfo.Account = loginData.UserName;
                userInfo.Platform = loginData.Platform;
                userInfo.DeviceID = loginData.DeviceUniqueIdentifier;
                userInfo.DeviceModel = loginData.DeviceModel;
                userInfo.DeviceType = loginData.DeviceTypeStr;
                userInfo.RegisterTime = DateTime.Now;
                userInfo.LastLoginTime = DateTime.Now;
                userInfo.Token = 1;
                if (!cache.Add(userInfo))
                {
                    Console.WriteLine("Regist UserInfo Failed!");
                }
                regist = true;
            }
            else
            {
                response = new LoginResponse();
                response.NickName = userInfo.NickName;
                response.Level = userInfo.Level;
                response.Exp = userInfo.Exp;
                response.Money = userInfo.Money;
                response.VIPLevel = userInfo.VIPLevel;

                userInfo.ModifyLocked(() =>
                {
                    userInfo.Account = loginData.UserName;
                    userInfo.Platform = loginData.Platform;
                    userInfo.DeviceID = loginData.DeviceUniqueIdentifier;
                    userInfo.DeviceModel = loginData.DeviceModel;
                    userInfo.DeviceType = loginData.DeviceTypeStr;
                    userInfo.LastLoginTime = DateTime.Now;
                    userInfo.Token = userInfo.Token + 1;
                });
                regist = false;
                if (string.IsNullOrEmpty(response.NickName))
                {
                    regist = true;
                }
            }

            int uid = (int)userInfo.UserId;

            if (!regist && response != null)
            {
                byte[] responseData = ProtoBufUtils.Serialize(response);
                action.SetResponseData(responseData);
            }
            else
            {
                byte[] responseData = BitConverter.GetBytes(uid);
                action.SetResponseData(responseData);
            }

            SessionUser user = new SessionUser();
            user.UserId = uid;
            GameSession session = action.GetActionGetter().GetSession();

            var OldSession = GameSession.Get(uid);
            if (OldSession != null)
            {
                OldSession.Close();
            }

            session.Bind(user);
        }

        static void OnRegist(byte[] data, Action5001 action)
        {
            int uid = action.GetActionGetter().GetSession().UserId;

            var cache = CacheSet.UserInfoCach;
            var userInfo = cache.FindKey(uid);
            if (userInfo == null)
                return;

            RegistData registData = ProtoBufUtils.Deserialize<RegistData>(data);

            userInfo.ModifyLocked(() =>
            {
                userInfo.NickName = registData.NickName;
            });

            LoginResponse response = new LoginResponse();
            response.UserID = uid;
            response.NickName = userInfo.NickName;
            response.Level = userInfo.Level;
            response.Exp = userInfo.Exp;
            response.Money = userInfo.Money;
            response.VIPLevel = userInfo.VIPLevel;
            byte[] responseData = ProtoBufUtils.Serialize(response);
            action.SetResponseData(responseData);
        }

        public static void Initialize()
        {
            NetWork.RegisterMessage(CTS.CTS_Login, OnLogin);
            NetWork.RegisterMessage(CTS.CTS_Regist, OnRegist);
        }

        public static void Uninitialize()
        {
            NetWork.UnregisterMessage(CTS.CTS_Login);
            NetWork.UnregisterMessage(CTS.CTS_Regist);
        }
    }
}