// AutoStartRedis.cpp : 定义控制台应用程序的入口点。
//

#include "stdafx.h"
#include <string>
#include <vector>
#include <windows.h>

static std::vector<std::string> servers = {
	"LoginServer",
	"LobbyServer",
	"RoomServer",
};

int _tmain(int argc, _TCHAR* argv[])
{
	TCHAR szFilePath[MAX_PATH + 1] = { 0 };
	GetModuleFileName(NULL, szFilePath, MAX_PATH);

	std::string exePath = szFilePath;
	size_t rootPos = exePath.find("Tools");
	if (rootPos == 0)
		return 0;
	std::string root = exePath.substr(0, rootPos);
	
	for (size_t i = 0; i < servers.size(); ++i)
	{
		std::string dir = root + servers[i];
		std::string redisCmd = dir + "\\redis-server.exe " + dir + "\\redis.windows.conf";
		WinExec(redisCmd.c_str(), 1);
	}
	return 0;
}

