using GZipTest.DataStruct;
using System.IO;
using System.IO.Compression;

namespace GZipTest.Algorithm
{
    public class Decompressor : Archiver
    {
        public Decompressor(BlockQueue inputQueue) : base(inputQueue)
        {
        }

        protected override void DoWorkImpl()
        {
            while (true)
            {
                var block = InputQueue.Dequeue();
                if (block == null) break;

                using (var inStream = new MemoryStream(block.Bytes))
                using (var gzipStream = new GZipStream(inStream, CompressionMode.Decompress))
                using (var outStream = new MemoryStream())
                {
                    gzipStream.CopyTo(outStream);

                    var decompressedBlock = new ByteBlock(block.ID, outStream.ToArray());
                    ResultQueue.Enqueue(decompressedBlock, true);
                }
            }
        }
    }
}
