using System;
using System.IO;

namespace HotelManager
{
    internal class Logging
    {
        public static Logging Instance => thisInstance ?? new Logging();
        private static Logging thisInstance;
        private readonly TextWriter Writer;

        public Logging()
        {
            Writer = new StreamWriter(Path.Combine(FileHandler.LocalPath,"Log.txt"));
            thisInstance = this;
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