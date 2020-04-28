namespace GZipTest.DataStruct
{
    public class ByteBlock
    {
        private static int _defaultSize = 1000000;
        private int _id;
        private byte[] _bytes;

        public ByteBlock(int id, byte[] bytes)
        {
            _id = id;
            _bytes = bytes;
        }

        public static int DefaultSize
        {
            get { return _defaultSize; }
        }

        public int ID
        {
            get { return _id; }
        }

        public byte[] Bytes
        {
            get { return _bytes; }
        }
    }
}
