using System.Collections.Generic;

namespace ModSettingsConverter
{
    class ListReadStream : IReadStream
    {
        private List<byte> data;
        private int pos = 0;

        public ListReadStream(List<byte> data)
        {
            this.data = data;
        }

        public bool EOF()
        {
            return pos == data.Count;
        }

        public int Read(byte[] buffer, int size)
        {
            int offset = 0;

            while (!EOF() && offset < size)
                buffer[offset++] = data[pos++];

            return offset;
        }

        public int Remaining()
        {
            return data.Count - pos;
        }
    }
}
