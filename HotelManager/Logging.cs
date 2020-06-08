using System;
using System.IO;

namespace HotelManager
{
    internal class Logging
    {
        private static Logging ThisInstance { get; set; }
        private readonly TextWriter Writer;

        public static Logging Instance => ThisInstance ?? new Logging();

        public Logging()
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
        }
    }
}