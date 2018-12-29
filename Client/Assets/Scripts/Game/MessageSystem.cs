using System;
using System.Collections.Generic;

public enum MessageType
{
    SetPlayerInfo,
    OnSetChaClass,
}

public class MessageSystem
{
    public delegate void Msg(params object[] objs);

    public void MsgRegister(MessageType msgType, Msg msg)
    {
        Msg msgDelegate;
        if (mMsgs.TryGetValue(msgType, out msgDelegate))
        {
            msgDelegate += msg;
        }
        else
        {
            mMsgs.Add(msgType, msg);
        }
    }

    public void MsgUnregister(MessageType msgType, Msg msg)
    {
        Msg msgDelegate;
        if (mMsgs.TryGetValue(msgType, out msgDelegate))
        {
            msgDelegate -= msg;
            if (msgDelegate == null)
            {
                mMsgs.Remove(msgType);
            }
        }
    }

    public void MsgDispatch(MessageType msgType, params object[] objs)
    {
        Msg msgDelegate;
        if (mMsgs.TryGetValue(msgType, out msgDelegate))
        {
            msgDelegate(objs);
        }
    }

    static MessageSystem mInstance = null;

    public static MessageSystem Instance
    {
        get
        {
            if (mInstance == null)
                mInstance = new MessageSystem();
            return mInstance;
        }
    }

    private Dictionary<MessageType, Msg> mMsgs = new Dictionary<MessageType, Msg>();
}