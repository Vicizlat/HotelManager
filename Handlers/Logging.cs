using System;
using System.IO;

namespace Handlers
{
    public class Logging
    {
        public static Logging Instance => thisInstance ?? new Logging();
        private static Logging thisInstance;
        private readonly TextWriter writer;

        public Logging()
        {
            writer = new StreamWriter(Path.Combine(FileHandler.LocalPath,"Log.txt"));
            thisInstance = this;
        }

        public void WriteLine(string text)
        {
            writer.WriteLine($"{DateTime.Now:dd.MM.yyyy HH:mm:ss:ffff}: {text}");
            writer.Flush();
        }

        public void Close()
        {
            WriteLine("Logging ended");
            writer.Dispose();
        }
    }
}