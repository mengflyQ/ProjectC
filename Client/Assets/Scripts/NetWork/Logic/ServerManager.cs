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
            return "192.168.0.102:9001";
        }
    }
    public static string LoginServerUrl
    {
        get
        {
            return "192.168.0.102:9002";
        }
    }
    public static string LobbyServerUrl
    {
        get
        {
            return "192.168.0.102:9003";
        }
    }
    public static string RoomServerUrl
    {
        get
        {
            return "192.168.0.102:9004";
        }
    }
}
