using System;

namespace GZipTest.Algorithm
{
    public interface IAlgorithm //: IDisposable
    {
        /// <summary>
        /// Исполняет алгоритм обработки файла (сжатие/распаковка)
        /// </summary>
        /// <param name="sourceFilePath">Исходный файл (сжимаемый или распаковываемый)</param>
        /// <param name="targetFilePath">Результирующий файл (сжатый или распакованный)</param>
        void Execute(string sourceFilePath, string targetFilePath);
    }

    public enum HandleMode
    {
        Compress = 0,
        Decompress = 1
    }
}
