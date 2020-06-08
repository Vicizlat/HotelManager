using System;
using System.IO;

namespace HotelManager
{
    internal class LogWriter
    {
        private static LogWriter ThisInstance { get; set; }
        private readonly TextWriter Writer;

        public static LogWriter Instance => ThisInstance ?? new LogWriter();

        public LogWriter()
        {
            Writer = new StreamWriter(Path.Combine(FileHandler.LocalPath,"Log.txt"));
            ThisInstance = this;
        }

        public void WriteLine(string text)
        {
            Writer.WriteLine($"{DateTime.Now:dd.MM.yyyy HH:mm:ss:ffff}: {text}");
            Writer.Flush();
        }

        internal void Close()
        {
            WriteLine("Logging ended");
            Writer.Dispose();
            FileHandler.TryUploadFile("Log.txt", true);
        }
    }
}