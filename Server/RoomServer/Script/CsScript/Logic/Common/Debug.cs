using System;

public static class Debug
{
    public static void LogError(string format, params object[] args)
    {
        ConsoleColor origColor = Console.ForegroundColor;
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine(format, args);
        Console.ForegroundColor = origColor;
    }
}