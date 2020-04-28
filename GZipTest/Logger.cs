using System;

namespace GZipTest
{
    public class Logger
    {
        private ConsoleColor _defaultColor;
        private static Logger _instance;
        private static object _syncObject = new object();

        private Logger()
        {
            _defaultColor = Console.ForegroundColor;
        }

        public static Logger Instance
        {
            get
            {
                lock (_syncObject)
                    return _instance ?? (_instance = new Logger());
            }
        }

        public void WriteInfo(string message)
        {
            Write(message, ConsoleColor.White);
        }

        public void WriteSuccess(string message)
        {
            Write(message, ConsoleColor.Green);
        }

        public void WriteError(string message)
        {
            Write(message, ConsoleColor.Red);
        }

        public void WriteError(Exception exception)
        {
            var error = string.Format("Необработанное исключение.\n Метод: {0}\n Описание {1}\n Стек: {2}", 
                exception.TargetSite.Name, exception.Message, exception.StackTrace);
            Write(error, ConsoleColor.Red);
        }

        private void Write(string message, ConsoleColor color)
        {
            Console.ForegroundColor = color;
            Console.WriteLine(message);
            Console.ForegroundColor = _defaultColor;
        }
    }
}
