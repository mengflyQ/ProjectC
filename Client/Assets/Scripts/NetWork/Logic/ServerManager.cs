using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class ServerManager
{
    public static string GateServerUrl
    {
        get
        {
            return "192.168.0.103:9001";
        }
    }
    public static string LoginServerUrl
    {
        get
        {
            return "192.168.0.103:9002";
        }
    }
    public static string LobbyServerUrl
    {
        get
        {
            return "192.168.0.103:9003";
        }
    }
    public static string RoomServerUrl
    {
        get
        {
            return "192.168.0.103:9004";
        }
    }
}
