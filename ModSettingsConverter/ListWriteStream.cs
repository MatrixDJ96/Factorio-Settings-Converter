using System.Collections.Generic;

namespace ModSettingsConverter
{
    class ListWriteStream : IWriteStream
    {
        public List<byte> buffer = new List<byte>();

        public void Write(byte[] data)
        {
            for (int i = 0; i < data.Length; ++i)
                buffer.Add(data[i]);
        }

        public void Write(byte data)
        {
            buffer.Add(data);
        }
    }
}
