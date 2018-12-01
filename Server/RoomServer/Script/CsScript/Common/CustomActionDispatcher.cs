using System;
using System.Net;
using System.Text;
using System.Web;
using System.IO;
using System.Collections.Generic;
using ZyGames.Framework.Common.Log;
using ZyGames.Framework.Common.Serialization;
using ZyGames.Framework.Game.Contract;
using ZyGames.Framework.Game.Service;
using ZyGames.Framework.RPC.IO;
using ZyGames.Framework.RPC.Sockets;
using GameServer.CommonLib;

namespace GameServer.CommonLib
{
    public class CustomActionDispatcher : IActionDispatcher
    {
        public bool TryDecodePackage(ConnectionEventArgs e, out RequestPackage package)
        {
            byte[] content = null;
            CTSPackageHead head = ReadMessageHead(e.Data, out content);
            if (head == null)
            {
                var packageReader = new PackageReader(e.Data, Encoding.UTF8);
                if (TryBuildPackage(packageReader, out package))
                {
                    package.OpCode = e.Meaage.OpCode;
                    package.CommandMessage = e.Socket.IsWebSocket && e.Meaage.OpCode == OpCode.Text
                    ? e.Meaage.Message
                    : null;
                    return true;
                }
                package = null;
                return false;
            }

            package = new RequestPackage(head.MsgId, head.SessionId, head.ActionId, head.UserId) { Message = content };
            return true;
        }

        private CTSPackageHead ReadMessageHead(byte[] data, out byte[] content)
        {
            CTSPackageHead headPackage = null;
            content = new byte[0];
            try
            {
                int pos = 0;
                byte[] headLenBytes = new byte[4];
                Buffer.BlockCopy(data, pos, headLenBytes, 0, headLenBytes.Length);
                pos += headLenBytes.Length;
                int headSize = BitConverter.ToInt32(headLenBytes, 0);
                if (headSize < data.Length)
                {
                    byte[] headBytes = new byte[headSize];
                    Buffer.BlockCopy(data, pos, headBytes, 0, headBytes.Length);
                    pos += headBytes.Length;
                    headPackage = ProtoBufUtils.Deserialize<CTSPackageHead>(headBytes);

                    if (data.Length > pos)
                    {
                        int len = data.Length - pos;
                        content = new byte[len];
                        Buffer.BlockCopy(data, pos, content, 0, content.Length);
                    }
                }
                else
                {
                    TraceLog.ReleaseWriteFatal("Can not parse head package.");
                }
            }
            catch (Exception ex)
            {
                TraceLog.WriteError(ex.Message);
            }
            return headPackage;
        }

        protected virtual bool TryBuildPackage(PackageReader packageReader, out RequestPackage package)
        {
            package = null;
            Guid proxySid;
            packageReader.TryGetParam("ssid", out proxySid);
            int actionid;
            if (!packageReader.TryGetParam("actionid", out actionid))
            {
                return false;
            }
            int msgid;
            if (!packageReader.TryGetParam("msgid", out msgid))
            {
                return false;
            }
            int userId;
            packageReader.TryGetParam("uid", out userId);
            string sessionId;
            string proxyId;
            packageReader.TryGetParam("sid", out sessionId);
            packageReader.TryGetParam("proxyId", out proxyId);

            package = new RequestPackage(msgid, sessionId, actionid, userId)
            {
                ProxySid = proxySid,
                ProxyId = proxyId,
                IsProxyRequest = packageReader.ContainsKey("isproxy"),
                RouteName = packageReader.RouteName,
                Params = packageReader.Params,
                Message = packageReader.InputStream,
                OriginalParam = packageReader.RawParam
            };
            return true;
        }

        public bool TryDecodePackage(HttpListenerRequest request, out RequestPackage package, out int statusCode)
        {
            statusCode = 200;
            package = null;
            byte[] content;
            var bytes = GetRequestStream(request.InputStream);
            CTSPackageHead head = ReadMessageHead(bytes, out content);
            if (head == null)
            {
                return false;
            }
            package = new RequestPackage(head.MsgId, head.SessionId, head.ActionId, head.UserId) { Message = content };
            return true;
        }

        public bool TryDecodePackage(HttpListenerContext context, out RequestPackage package)
        {
            int statuscode;
            if (TryDecodePackage(context.Request, out package, out statuscode))
            {
                return true;
            }
            return false;
        }

        public bool TryDecodePackage(HttpContext context, out RequestPackage package)
        {
            package = null;
            CTSPackageHead head = null;
            byte[] content = new byte[0];
            var bytes = GetRequestStream(context.Request.InputStream);
            head = ReadMessageHead(bytes, out content);
            if (head == null)
            {
                return false;
            }
            package = new RequestPackage(head.MsgId, head.SessionId, head.ActionId, head.UserId) { Message = content };
            return true;
        }

        public ActionGetter GetActionGetter(RequestPackage package, GameSession session)
        {
            return new ActionGetter(package, session);
        }

        public void ResponseError(BaseGameResponse response, ActionGetter actionGetter, int errorCode, string errorInfo)
        {
            STCPackageHead packetHead = new STCPackageHead()
            {
                MsgId = actionGetter.GetMsgId(),
                ActionId = actionGetter.GetActionId(),
                ErrorCode = errorCode,
                ErrorInfo = errorInfo
            };

            byte[] headBytes = ProtoBufUtils.Serialize(packetHead);
            byte[] buffer = BufferUtils.AppendHeadBytes(headBytes);
            response.BinaryWrite(buffer);
            //actionGetter.GetSession().SendAsync(buffer, 0, buffer.Length);
        }

        private static byte[] GetRequestStream(Stream stream)
        {
            using (BinaryReader readStream = new BinaryReader(stream))
            {
                List<byte> dataBytes = new List<byte>();
                int size = 0;
                while (true)
                {
                    var buffer = new byte[512];
                    size = readStream.Read(buffer, 0, buffer.Length);
                    if (size == 0)
                    {
                        break;
                    }
                    byte[] temp = new byte[size];
                    Buffer.BlockCopy(buffer, 0, temp, 0, size);
                    dataBytes.AddRange(temp);
                }
                return dataBytes.ToArray();
            }
        }
    }
}
