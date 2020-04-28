using System;

namespace GZipTest.Algorithm
{
    public class DecompressAlgorithm : IAlgorithm
    {
        public void Execute(string sourceFilePath, string targetFilePath)
        {
            var timeBeg = DateTime.Now.TimeOfDay.TotalMinutes;
            Logger.Instance.WriteInfo("Начато разархивирование файла...");

            var reader = new Reader(sourceFilePath, HandleMode.Decompress);
            var readedQueue = reader.Read();

            Archiver archiver = new Decompressor(readedQueue);
            var compressedQueue = archiver.DoWork();

            var writer = new Writer(targetFilePath, compressedQueue, HandleMode.Decompress);
            writer.Write();

            var timeEnd = DateTime.Now.TimeOfDay.TotalMinutes;
            Logger.Instance.WriteInfo(string.Format("Разархивирование файла завершено. Время разархивирования (мин): {0}", timeEnd - timeBeg));

            reader.Dispose();
            archiver.Dispose();
            writer.Dispose();
        }
    }
}
