using System;
using System.Collections.Generic;
using System.Threading;

namespace GZipTest.DataStruct
{
    public class BlockQueue : IDisposable
    {
        private bool _disposed;
        private Queue<ByteBlock> _queue;
        private int _enqueuedBlockId = 1;
        private bool _isClosed;
        private int _maxQueueItemCount;

        private readonly object _syncObject = new object();

        /// <summary>
        /// Создает новый экземпляр очереди байтовых блоков.
        /// </summary>
        public BlockQueue(int maxQueueItemCount)
        {
            _queue = new Queue<ByteBlock>();
            _maxQueueItemCount = maxQueueItemCount;
        }

        /// <summary>
        /// Количество элементов в очереди на текущий момент времени.
        /// </summary>
        public int Count
        {
            get
            {
                lock (_syncObject)
                    return _queue.Count;
            }
        }

        /// <summary>
        /// Флаг, указывающий пользователям очереди, стоит ли ожидать дальнейшего заполнения.
        /// </summary>
        public bool IsClosed
        {
            get
            {
                lock (_syncObject)
                    return _isClosed;
            }
            set
            {
                lock (_syncObject)
                {
                    if(_isClosed && !value) throw new ArgumentException("IsClosed");

                    _isClosed = value;
                    Monitor.PulseAll(_syncObject);
                }
            }
        }
        
        /// <summary>
        /// Метод добавления элементов в конец очереди
        /// </summary>
        /// <param name="block">Байтовый блок</param>
        /// <param name="ordered">Флаг, указывающий, будут ли блоки в очереди упорядочены по идентификатору</param>
        public void Enqueue(ByteBlock block, bool ordered = false)
        {
            if (block == null) throw new ArgumentNullException("block");
            
            lock(_syncObject)
            {
                if (_disposed) throw new ObjectDisposedException("BlockQueue");

                while (_queue.Count >= _maxQueueItemCount)
                    Monitor.Wait(_syncObject);

                if (_queue.Count >= _maxQueueItemCount)
                    throw new InvalidOperationException("Невозможно добавить блок в очередь - очередь полна.");

                if (ordered)
                {
                    while (block.ID != _enqueuedBlockId)
                        Monitor.Wait(_syncObject);
                }
                
                _queue.Enqueue(block);
                _enqueuedBlockId++;
                
                Monitor.PulseAll(_syncObject);
            }
        }

        /// <summary>
        /// Метод извлечения элементов с начала очереди
        /// </summary>
        /// <returns>Возвращает байтовый блок, находящийся в начале очереди</returns>
        public ByteBlock Dequeue()
        {
            lock(_syncObject)
            {
                if (_disposed) throw new ObjectDisposedException("BlockQueue");

                while(_queue.Count == 0 && !_isClosed)
                    Monitor.Wait(_syncObject);

                if (_queue.Count == 0)
                    return null;

                var block = _queue.Dequeue();
                
                if (_queue.Count < _maxQueueItemCount)
                    Monitor.PulseAll(_syncObject);

                return block;
            }
        }

        #region IDisposable

        protected virtual void Dispose(bool disposing)
        {
            lock(_syncObject)
            {
                if (!_disposed)
                {
                    if (disposing)
                    {
                        _queue.Clear();
                    }

                    _queue = null;

                    _disposed = true;
                }
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }
        #endregion
    }
}
