using System;

namespace GZipTest.Algorithm
{
    public class CompressAlgorithm : IAlgorithm
    {

        /// <summary>
        /// Реализует сжатие файла
        /// </summary>
        /// <param name="sourceFilePath">Исходный файл (сжимаемый)</param>
        /// <param name="targetFilePath">Результирующий файл (сжатый)</param>
        public void Execute(string sourceFilePath, string targetFilePath)
        {
            var timeBeg = DateTime.Now.TimeOfDay.TotalMinutes;
            Logger.Instance.WriteInfo("Начато сжатие файла...");

            var reader = new Reader(sourceFilePath, HandleMode.Compress);
            var readedQueue = reader.Read();

            Archiver archiver = new Compressor(readedQueue);
            var compressedQueue = archiver.DoWork();
            
            var writer = new Writer(targetFilePath, compressedQueue, HandleMode.Compress);
            writer.Write();

            var timeEnd = DateTime.Now.TimeOfDay.TotalMinutes;
            Logger.Instance.WriteInfo(string.Format("Сжатие файла завершено. Время сжатия (мин): {0}", timeEnd - timeBeg));

            reader.Dispose();
            archiver.Dispose();
            writer.Dispose();
        }
    }
}
