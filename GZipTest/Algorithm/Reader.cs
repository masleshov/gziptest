using GZipTest.DataStruct;
using System;
using System.IO;
using System.Threading;

namespace GZipTest.Algorithm
{
    public class Reader : IDisposable
    {
        private bool _disposed;
        private BlockQueue _queue;
        private string _filePath;
        private HandleMode _mode;
        private BlockQueueDispatcher _queueDispatcher;

        /// <summary>
        /// Создает экземпляр читателя файла с указанным режимом чтения
        /// </summary>
        /// <param name="filePath">Путь до читаемого файла</param>
        /// <param name="mode">Режим чтения - чтение распакованного файла (для последующего сжатия) 
        /// или чтение сжатого файла (для дальнейшей распаковки)</param>
        public Reader(string filePath, HandleMode mode)
        {
            _queue = new BlockQueue(Environment.ProcessorCount);
            _queueDispatcher = new BlockQueueDispatcher(_queue, 1);
            _filePath = filePath;
            _mode = mode;
        }

        /// <summary>
        /// Запускает асинхронную задачу чтения файла
        /// </summary>
        /// <returns>Очередь из прочитанных файловых блоков</returns>
        public BlockQueue Read()
        {
            var thread = new Thread(ProcessRead);
            thread.Start();
            
            return _queue;
        }

        private void ProcessRead()
        {
            try
            {
                switch (_mode)
                {
                    case HandleMode.Compress:
                        ReadUncompressed();
                        break;
                    case HandleMode.Decompress:
                        ReadCompressed();
                        break;

                }
            }
            catch (Exception ex)
            {
                Logger.Instance.WriteError(ex);
                return;
            }
        }

        private void ReadUncompressed()
        {
            using (var stream = File.Open(_filePath, FileMode.Open, FileAccess.Read))
            {
                int id = 1;
                while (stream.Position < stream.Length)
                {
                    var arraySize = GetBlockSize(stream.Length, stream.Position);
                    var byteArray = new byte[arraySize];
                    stream.Read(byteArray, 0, arraySize);

                    var block = new ByteBlock(id, byteArray);
                    _queue.Enqueue(block);
                    id++;
                }

                _queueDispatcher.CloseQueue();
            }
        }

        private void ReadCompressed()
        {
            using (var stream = File.Open(_filePath, FileMode.Open, FileAccess.Read))
            {
                int id = 1;
                while(stream.Position < stream.Length)
                {
                    var blockLengthArray = new byte[8];
                    stream.Read(blockLengthArray, 0, blockLengthArray.Length);
                    var blockLength = BitConverter.ToInt32(blockLengthArray, 4);
                    stream.Position = stream.Position - 8;

                    var block = new ByteBlock(id, new byte[blockLength]);
                    stream.Read(block.Bytes, 0, blockLength);
                    _queue.Enqueue(block);
                    id++;
                }

                _queueDispatcher.CloseQueue();
            }
        }

        private int GetBlockSize(long streamLenght, long streamPosition)
        {
            var remainingBytes = streamLenght - streamPosition;
            return Convert.ToInt32(remainingBytes >= ByteBlock.DefaultSize ? ByteBlock.DefaultSize : remainingBytes);
        }

        #region IDisposable

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    if (_queue != null) _queue.Dispose();
                    _queueDispatcher = null;
                }

                _disposed = true;
            }
        }
        public void Dispose()
        {
            Dispose(true);
        }
        #endregion
    }
}
