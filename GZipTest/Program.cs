using GZipTest.Algorithm;
using System;
using System.IO;

namespace GZipTest
{
    class Program
    {
        private static string _commandLinePattern = "Команда запуска должна удовлетворять следующему шаблону: GZipTest.exe compress/decompress [имя исходного файла] [имя результирующего файла]";

        static void Main(string[] args)
        {
            if (args.Length < 3)
            {
                Logger.Instance.WriteError(string.Format("Невозможно запустить приложение без обязательных параметров. {0}", _commandLinePattern));
                return;
            }

            var modeParameter = args[0];
            var inputFilePath = args[1];
            var outputFilePath = args[2];

            var validationResult = ValidateParameters(modeParameter, inputFilePath, outputFilePath);
            if (!validationResult) return;

            IAlgorithm algorithm = GetAlgorithm(modeParameter);
            var fileHandler = new FileHandler(algorithm);

            //АОП конечно хорошо для логирования, но в рамках этого приложения - как из пушки по воробьям.
            try
            {
                fileHandler.Handle(inputFilePath, outputFilePath);
            }
            catch (Exception ex)
            {
                Logger.Instance.WriteError(ex);
                return;
            }

            Console.ReadLine();
        }

        private static bool ValidateParameters(string modeParameter, string inputFilePath, string outputFilePath)
        {
            if (string.Compare(modeParameter.ToLower(), "compress", StringComparison.Ordinal) != 0 &&
                string.Compare(modeParameter.ToLower(), "decompress", StringComparison.Ordinal) != 0)
            {
                Logger.Instance.WriteError(string.Format("Параметр режима запуска приложения указан неверно. {0}", _commandLinePattern));
                return false;
            }

            if (string.IsNullOrEmpty(inputFilePath))
            {
                Logger.Instance.WriteError(string.Format("Не указан исходный файл. {0}", _commandLinePattern));
                return false;
            }

            if (!File.Exists(inputFilePath))
            {
                Logger.Instance.WriteError("Исходный файл отсутствует.");
                return false;
            }

            if (string.IsNullOrEmpty(outputFilePath))
            {
                Logger.Instance.WriteError(string.Format("Не указан результирующий файл. {0}", _commandLinePattern));
                return false;
            }

            if (string.Compare(outputFilePath.Substring(outputFilePath.Length-3).ToLower(), ".gz", StringComparison.Ordinal) != 0 
                && string.Compare(modeParameter.ToLower(), "compress", StringComparison.Ordinal) == 0)
            {
                Logger.Instance.WriteError("Расширение результирующего файла отличается от \".gz\".");
                return false;
            }

            if (string.Compare(inputFilePath.Substring(inputFilePath.Length - 3).ToLower(), ".gz", StringComparison.Ordinal) != 0
                && string.Compare(modeParameter.ToLower(), "decompress", StringComparison.Ordinal) == 0)
            {
                Logger.Instance.WriteError(string.Format("Расширение исходного файла отличается от \".gz\"."));
                return false;
            }

            if (File.Exists(outputFilePath))
            {
                Logger.Instance.WriteError("Результирующий файл уже существует. В процессе работы программы он будет перезаписан.");
            }

            return true;
        }

        private static IAlgorithm GetAlgorithm(string modeParameter)
        {
            switch (modeParameter.ToLower())
            {
                case "compress": return new CompressAlgorithm();
                case "decompress": return new DecompressAlgorithm();
                default: return null;
            }
        }
    }
}
