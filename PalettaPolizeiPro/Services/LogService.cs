using System.Text;

namespace PalettaPolizeiPro.Services
{
    public static class LogService
    {
        private static object _myLock = new object();
        private static string? _path; 
        public static void Init(string path)
        { 
            _path = path;
        }
        public static void Log(object log, LogLevel level)
        {

            if (_path is null) { throw new NullReferenceException(); }
            string writable = $"[{DateTime.Now}] [{level.ToString()}] {log} \n\n";
            lock( _myLock )
            {
                FileStream stream;
                if (!File.Exists(_path))
                { 
                    stream = File.Create( _path );
                }
                else
                {
                    stream = File.OpenWrite(_path);    
                }
                long endPoint = stream.Length;
                stream.Seek(endPoint, SeekOrigin.Begin);
                byte[] toWrite = new UTF8Encoding(true).GetBytes(writable);
                stream.Write(toWrite,0,toWrite.Length);
                stream.Dispose();

                LogToConsole(writable, level);

            }

        }
        public static void LogException(Exception ex)
        {
            string full = ex.ToString();
            if (ex.InnerException is not null)
            {
                full += "\n" + ex.InnerException.ToString();
            }
            Log(ex.ToString(),LogLevel.Error);
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
            Console.WriteLine(log);
            Console.ForegroundColor = def;    
        }
    }
}
