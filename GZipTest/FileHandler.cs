using GZipTest.Algorithm;
using System;

namespace GZipTest
{
    /// <summary>
    /// Класс, исполняющий некую обработку файла по заданному алгоритму
    /// </summary>
    public class FileHandler
    {
        private IAlgorithm _algorithm;

        /// <summary>
        /// Создает новый экземпляр обработчика файлов по заданному алгоритму
        /// </summary>
        /// <param name="algorithm">Алгоритм обработки файла</param>
        public FileHandler(IAlgorithm algorithm)
        {
            _algorithm = algorithm;
        }

        /// <summary>
        /// Запускает исполнение алгоритма обработки файла
        /// </summary>
        /// <param name="sourceFilePath">Исходный файл (сжимаемый)</param>
        /// <param name="targetFilePath">Результирующий файл (сжатый)</param>
        public void Handle(string sourceFilePath, string targetFilePath)
        {
            //Вообще, в декомпозиции можно было пойти и дальше.
            //Алгоритм однотипен - прочитать с диска, сжать/разжать и записать на диск.
            //Можно было бы применить стратегию непосредственно к классам FileReader, FileCompressor (переименовать его), FileWriter.
            //Но это усложнило бы код. Цель - не максимальное дробление классов, а сокрытие внутренней логики работы обработчика файлов от внешнего пользователя.
            _algorithm.Execute(sourceFilePath, targetFilePath);
        }
    }
}
