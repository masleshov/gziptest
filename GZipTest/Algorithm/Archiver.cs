using GZipTest.DataStruct;
using System;
using System.Linq;
using System.Threading;

namespace GZipTest.Algorithm
{
    /// <summary>
    /// Класс, выполняющий некую работу по архивированию/разархивированию файла
    /// </summary>
    public abstract class Archiver : IDisposable
    {
        private bool _disposed;
        private BlockQueue _inputQueue;
        private BlockQueue _resultQueue;
        private Thread[] _threads;
        private BlockQueueDispatcher _queueDispatcher;
        private object _syncObject = new object();

        /// <summary>
        /// Создает новый экземпляр архиватора, неким образом обрабатывающего очередь блоков
        /// </summary>
        /// <param name="inputQueue">Входная очередь байтовых блоков, подлежащая обработке алгоритмом архивации/разархивации</param>
        protected Archiver(BlockQueue inputQueue)
        {
            _inputQueue = inputQueue;
            _resultQueue = new BlockQueue(1);
            _threads = new Thread[Environment.ProcessorCount];
            _queueDispatcher = new BlockQueueDispatcher(_resultQueue, _threads.Length);
        }

        protected BlockQueue InputQueue
        {
            get { return _inputQueue; }
        }

        protected BlockQueue ResultQueue
        {
            get { return _resultQueue; }
        }

        public BlockQueue DoWork()
        {
            for(int i=0; i<Environment.ProcessorCount; i++)
            {
                _threads[i] = new Thread(Archive);
                _threads[i].Start();
            }

            return _resultQueue;
        }

        protected abstract void DoWorkImpl();

        private void Archive()
        {
            try
            {
                DoWorkImpl();
                _queueDispatcher.CloseQueue();
            }
            catch(Exception ex)
            {
                Logger.Instance.WriteError(ex);
                return;
            }
        }

        #region IDisposable

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    if (_inputQueue != null) _inputQueue.Dispose();
                    if (_resultQueue != null) _resultQueue.Dispose();
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
