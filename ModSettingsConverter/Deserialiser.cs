using System;
using System.Text;

namespace ModSettingsConverter
{
    class Deserialiser
    {
        private readonly IReadStream stream;
        public readonly MapVersion mapVersion;
        private readonly byte[] primitiveBuffer = new byte[Math.Max(sizeof(double), sizeof(ulong))];

        public Deserialiser(IReadStream stream)
        {
            this.stream = stream;
            mapVersion = new MapVersion(this);
            if (mapVersion < MapVersion.Minimum)
                throw new Exception($"Unsupported map version {mapVersion} < {MapVersion.Minimum} (input is too old).");
        }

        private byte[] LoadRaw(byte[] inBuffer, int size)
        {
            if (stream.Read(inBuffer, size) != size)
                throw new Exception("Error reading from stream.");
            return inBuffer;
        }

        private byte[] LoadPrimitive(int size)
        {
            return LoadRaw(primitiveBuffer, size);
        }

        public bool LoadBool()
        {
            return BitConverter.ToBoolean(LoadPrimitive(sizeof(bool)), 0);
        }

        public byte LoadByte()
        {
            return LoadPrimitive(sizeof(byte))[0];
        }

        public ushort LoadUShort()
        {
            return BitConverter.ToUInt16(LoadPrimitive(sizeof(ushort)), 0);
        }

        public uint LoadUInt()
        {
            return BitConverter.ToUInt32(LoadPrimitive(sizeof(uint)), 0);
        }

        public ulong LoadULong()
        {
            return BitConverter.ToUInt64(LoadPrimitive(sizeof(ulong)), 0);
        }

        public double LoadDouble()
        {
            return BitConverter.ToDouble(LoadPrimitive(sizeof(double)), 0);
        }

        public string LoadString()
        {
            if (LoadBool()) // true if empty
                return string.Empty;

            uint stringSize = LoadByte();
            if (stringSize == byte.MaxValue)
                stringSize = LoadUInt();

            byte[] buffer = new byte[stringSize];
            LoadRaw(buffer, (int)stringSize);
            return Encoding.UTF8.GetString(buffer);
        }
    }
}
