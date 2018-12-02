using System;

public enum CTS
{
    CTS_Default,
    CTS_Login,
    CTS_Regist,

    CTS_EnterLobby,
    CTS_Match,
    CTS_MatchReady,

    CTS_EnterScn,
    CTS_LoadedScn,
}

public enum STC
{
    STC_Default,
    STC_RoomInfo,
    STC_RoomAddPlayer,
    STC_MatchSuccess,
    STC_MatchFailed,
    STC_MatchReady,
    STC_SceneReady,

    STC_ScnLoad,
    STC_StartClienGame,
}

public enum STS
{
    STS_Default,
    STS_CreateScn,
}