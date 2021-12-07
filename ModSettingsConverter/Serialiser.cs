using System;
using System.Text;

namespace ModSettingsConverter
{
    class Serialiser
    {
        private IWriteStream stream;

        public Serialiser(IWriteStream stream, MapVersion mapVersion)
        {
            this.stream = stream;
            mapVersion.Save(this);
        }

        public void Write(bool value)
        {
            stream.Write(BitConverter.GetBytes(value));
        }

        public void Write(byte value)
        {
            stream.Write(value);
        }

        public void Write(ushort value)
        {
            stream.Write(BitConverter.GetBytes(value));
        }

        public void Write(uint value)
        {
            stream.Write(BitConverter.GetBytes(value));
        }

        public void Write(ulong value)
        {
            stream.Write(BitConverter.GetBytes(value));
        }

        public void Write(double value)
        {
            stream.Write(BitConverter.GetBytes(value));
        }

        public void Write(string value)
        {
            Write(value.Length == 0);
            if (value.Length == 0)
                return;

            byte[] buffer = Encoding.UTF8.GetBytes(value);

            if (buffer.Length < byte.MaxValue)
                Write((byte)buffer.Length);
            else
            {
                Write(byte.MaxValue);
                Write((uint)buffer.Length);
            }

            stream.Write(buffer);
        }
    }
}
