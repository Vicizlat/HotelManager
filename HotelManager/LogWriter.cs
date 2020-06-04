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
            Writer = new StreamWriter($"{FileHandler.LocalPath}\\Log.txt");
            ThisInstance = this;
        }

        public void WriteLine(string text)
        {
            Writer.WriteLine($"{DateTime.Now}: {text}");
            Writer.Flush();
        }

        internal void Close()
        {
            WriteLine("Logging ended");
            FileHandler.TryUploadFile("Log.txt");
            Writer.Dispose();
        }
    }
}