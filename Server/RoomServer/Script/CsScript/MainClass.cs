/****************************************************************************
Copyright (c) 2013-2015 scutgame.com

http://www.scutgame.com

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in
all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
THE SOFTWARE.
****************************************************************************/
using System;
using ZyGames.Framework.Game.Contract;
using ZyGames.Framework.Game.Runtime;
using ZyGames.Framework.Game.Service;
using ZyGames.Framework.RPC.Sockets;
using ZyGames.Framework.Script;
using GameServer.CommonLib;
using GameServer.RoomServer;

namespace Game.Script
{
    public class MainClass : GameSocketHost, IMainScript
    {
        public MainClass()
        {
            GameEnvironment.Setting.ActionDispatcher = new CustomActionDispatcher();
            ExcelLoader.Init();
            NavigationSystem.LoadAllNavigation();
        }
     
        protected override void OnStartAffer()
        {
            NetWorkRegister.Initialize();
            NetWork.InitialAllServers();
            Time.Start();
        }

        protected override void OnServiceStop()
        {
            Time.Stop();
            NetWorkRegister.Uninitialize();
            GameEnvironment.Stop();
        }

        protected override void OnConnectCompleted(object sender, ConnectionEventArgs e)
        {
            Console.WriteLine("客户端IP:[{0}]已与服务器连接成功", e.Socket.RemoteEndPoint);
            base.OnConnectCompleted(sender, e);
        }

        protected override void OnDisconnected(GameSession session)
        {
            Console.WriteLine("客户端UserId:[{0}]已与服务器断开", session.RemoteAddress);
            Player player = SceneManager.Instance.FindPlayer(session.UserId);
            if (player != null)
            {
                player.mScene.DelPlayer(player);
            }
            base.OnDisconnected(session);
        }

        protected override void OnHeartbeat(GameSession session)
        {
            byte[] head = new byte[5 * sizeof(int)];
            byte[] actionId = BitConverter.GetBytes((int)ActionType.Heartbeat);
            Buffer.BlockCopy(actionId, 0, head, 12, sizeof(int));

            byte[] body = BitConverter.GetBytes(Time.ElapsedSeconds);

            byte[] buffer = new byte[sizeof(int) + head.Length + body.Length];

            byte[] streamLenBytes = BitConverter.GetBytes(buffer.Length);

            int pos = 0;
            Buffer.BlockCopy(streamLenBytes, 0, buffer, pos, streamLenBytes.Length);
            pos += streamLenBytes.Length;
            Buffer.BlockCopy(head, 0, buffer, pos, head.Length);
            pos += head.Length;
            Buffer.BlockCopy(body, 0, buffer, pos, body.Length);

            session.SendAsync(OpCode.Binary, buffer, 0, buffer.Length, asyncResult =>
            {
                Console.WriteLine("Push Action -> {0} result is -> {1}", (int)ActionType.Heartbeat, asyncResult.Result == ResultCode.Success ? "ok" : "fail");
            });

            // Console.WriteLine("{0}>>Hearbeat package: {1} userid {2} session count {3}", DateTime.Now.ToString("HH:mm:ss"), session.RemoteAddress, session.UserId, GameSession.Count);
            base.OnHeartbeat(session);
        }

        protected override void OnRequested(ActionGetter actionGetter, BaseGameResponse response)
        {
            Console.WriteLine("Client {0} request action {1}", actionGetter.GetSessionId(), actionGetter.GetActionId());
        }
    }
}