using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace StockViewApp
{
    public static class LogManager
    {
        private static string filename = "ErrorLogs.txt";
        static LogManager()
        {
            File.Open(filename, FileMode.OpenOrCreate).Close();
        }
        public static void Write(string message)
        {
            File.AppendAllText(filename, message);
        }
    }
}
