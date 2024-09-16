using System.Text;

namespace PalettaPolizeiPro.Services;

public static class LogService
{
    private static object _myLock = new object();
    private static string? _folder;
    private static bool _consoleLogging;

    public static void Init(string folder, bool enableConsoleLog = true)
    {
        _consoleLogging = enableConsoleLog;
        _folder = folder;
    }

    public static void Log(object? log, LogLevel level = LogLevel.Information)
    {
        Task.Run(() =>
        {
            lock (_myLock)
            {
                if (_folder is null)
                {
                    throw new NullReferenceException();
                }

                if (!Directory.Exists(_folder))
                {
                    Directory.CreateDirectory(_folder);
                }

                string path = Path.Combine(_folder, DateTime.Now.ToString("yyyy_MM_dd") + ".txt");
                string writable = $"[{DateTime.Now}] [{level.ToString()}] {log}\n";
                FileStream stream;
                if (!File.Exists(path))
                {
                    stream = File.Create(path);
                }
                else
                {
                    stream = File.OpenWrite(path);
                }

                long endPoint = stream.Length;
                stream.Seek(endPoint, SeekOrigin.Begin);
                byte[] toWrite = new UTF8Encoding(true).GetBytes(writable);
                stream.Write(toWrite, 0, toWrite.Length);
                stream.Dispose();

                if (_consoleLogging)
                {
                    LogToConsole(writable, level);
                }
            }
        });
    }

    public static void LogException(Exception ex)
    {
        string full = ex.ToString();
        if (ex.InnerException is not null)
        {
            full += "\n" + ex.InnerException.ToString();
        }

        Log(ex.ToString(), LogLevel.Error);
    }

    private static void LogToConsole(string log, LogLevel level)
    {
        var def = Console.ForegroundColor;
        ConsoleColor color = level switch
        {
            LogLevel.Warning => ConsoleColor.Yellow,
            LogLevel.Error => ConsoleColor.Red,
            LogLevel.Information => ConsoleColor.Blue,
            _ => def,
        };
        Console.ForegroundColor = color;
        Console.Write(log);
        Console.ForegroundColor = def;
    }

    public static void EnableConsoleLog()
    {
        _consoleLogging = true;
    }

    public static void DisableConsoleLog()
    {
        _consoleLogging = false;
    }
}