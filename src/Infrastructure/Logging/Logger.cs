using System;

namespace Infrastructure.Logging;

public static class Logger
{
    public static class LoggerConfig
    {
        public static bool Enabled { get; set; } = true;
    }

    public static void Log(string message)
    {
        if (!LoggerConfig.Enabled) return;
        Console.WriteLine("[LOG] " + DateTime.Now + " - " + message);
    }

    public static void Try(Action a)
    {
        try { a(); } catch(Exception e) { Console.WriteLine($"Ha ocurrido un error: {e}"); }
    }
}
