using GZipTest.DataStruct;
using System;
using System.IO;
using System.Threading;

namespace GZipTest.Algorithm
{
    public class Writer : IDisposable
    {
        private bool _disposed;
        private string _filePath;
        private BlockQueue _queue;
        private HandleMode _mode;

        public Writer(string filePath, BlockQueue queue, HandleMode mode)
        {
            _filePath = filePath;
            _queue = queue;
            _mode = mode;
        }

        public void Write()
        {
            var thread = new Thread(ProcessWrite);
            thread.Start();
            thread.Join();
        }

        private void ProcessWrite()
        {
            try
            {
                switch (_mode)
                {
                    case HandleMode.Compress:
                        WriteCompressed();
                        break;
                    case HandleMode.Decompress:
                        WriteUncompressed();
                        break;
                }
            }
            catch(Exception ex)
            {
                Logger.Instance.WriteError(ex);
                return;
            }
        }

        private void WriteUncompressed()
        {
            using (var stream = File.Open(_filePath, FileMode.Append, FileAccess.Write))
            {
                while (true)
                {
                    var block = _queue.Dequeue();
                    if (block == null) break;

                    stream.Write(block.Bytes, 0, block.Bytes.Length);
                }
            }
        }

        private void WriteCompressed()
        {
            using (var stream = File.Open(_filePath, FileMode.Append, FileAccess.Write))
            {
                while (true)
                {
                    var block = _queue.Dequeue();
                    if (block == null) break;

                    //Записываем данные о длине блока на место информации о MTIME. 
                    //Удобно, т.к. offset всего 4 байта и это самые безобидные байты, изменив которые не теряется ничего существенного.
                    //Однако, в случае изменения архива извне, информация о длине блока будет потеряна и распаковка окажется невозможной.
                    //Можно поизучать документацию глубже и разобраться, куда правильнее писать эту информацию. 
                    //Разобравшись в этом, можно сделать алгоритм распаковки универсальным для всех gz-архивов.
                    //Глядя на документацию, кажется, что либо в XLEN, либо в ISIZE.
                    //Но на это уйдет много времени, а смысл тестового, как я понимаю, больше в многопоточности, нежели в знании формата gzip.
                    //http://www.gzip.org/zlib/rfc-gzip.html#member-format
                    var blockLength = BitConverter.GetBytes(block.Bytes.Length);
                    blockLength.CopyTo(block.Bytes, 4);
                    stream.Write(block.Bytes, 0, block.Bytes.Length);
                }
            }
        }

        #region IDisposable

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    if (_queue != null) _queue.Dispose();
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