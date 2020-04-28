namespace GZipTest.DataStruct
{
    /// <summary>
    /// Класс, управляющий состоянием очереди и обеспечивающий согласованность её закрытия
    /// </summary>
    public class BlockQueueDispatcher
    {
        private BlockQueue _queue;
        private int _producersCount;
        private int _counter;
        private object _syncObject = new object();

        public BlockQueueDispatcher(BlockQueue queue, int producersCount)
        {
            _queue = queue;
            _producersCount = producersCount;
        }

        /// <summary>
        /// Обрабатывает запрос на закрытие очереди и в случае, если последний поставщик данных очереди
        /// запрашивает её закрытие - закрывает очередь.
        /// </summary>
        public void CloseQueue()
        {
            lock(_syncObject)
            {
                _counter++;
                if (_counter.CompareTo(_producersCount) == 0)
                    _queue.IsClosed = true;
            }
        }
    }
}
