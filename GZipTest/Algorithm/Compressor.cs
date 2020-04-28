using GZipTest.DataStruct;
using System.IO;
using System.IO.Compression;

namespace GZipTest.Algorithm
{
    public class Compressor : Archiver
    {
        public Compressor(BlockQueue inputQueue) : base(inputQueue)
        {
        }

        protected override void DoWorkImpl()
        {
            while (true)
            {
                var block = InputQueue.Dequeue();
                if (block == null) break;

                using (var memoryStream = new MemoryStream())
                {
                    using (var gzipStream = new GZipStream(memoryStream, CompressionMode.Compress))
                    {
                        gzipStream.Write(block.Bytes, 0, block.Bytes.Length);
                    }

                    var compressedArray = memoryStream.ToArray();
                    var compressedBlock = new ByteBlock(block.ID, compressedArray);
                    
                    ResultQueue.Enqueue(compressedBlock, true);
                }
            }
        }
    }
}
