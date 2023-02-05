
namespace ModSettingsConverter
{
    class MapVersion
    {
        public static readonly MapVersion Minimum = new MapVersion(1, 1, 0, 0);
        public static readonly MapVersion Latest = new MapVersion(1, 1, 76, 0);

        private readonly ulong version;

        private ushort GetMainVersion() { return (ushort)(version >> 48); }
        private ushort GetMajorVersion() { return (ushort)(version >> 32); }
        private ushort GetMinorVersion() { return (ushort)(version >> 16); }
        private ushort GetDeveloperVersion() { return (ushort)(version); }

        public MapVersion(ushort mainVersion, ushort majorVersion, ushort minorVersion, ushort developerVersion)
        {
            version = (ulong)developerVersion | ((ulong)minorVersion << 16) | ((ulong)majorVersion << 32) | ((ulong)mainVersion << 48);
        }

        public MapVersion(Deserialiser input)
        {
            ushort mainVersion = input.LoadUShort();
            ushort majorVersion = input.LoadUShort();
            ushort minorVersion = input.LoadUShort();
            ushort developerVersion = input.LoadUShort();
            version = (ulong)developerVersion | ((ulong)minorVersion << 16) | ((ulong)majorVersion << 32) | ((ulong)mainVersion << 48);
            // Skip empty byte
            input.LoadByte();
        }

        public void Save(Serialiser output)
        {
            output.Write(GetMainVersion());
            output.Write(GetMajorVersion());
            output.Write(GetMinorVersion());
            output.Write(GetDeveloperVersion());
            // Write empty byte
            output.Write(byte.MinValue);
        }

        public static bool operator <(MapVersion a, MapVersion b)
        {
            return a.version < b.version;
        }

        public static bool operator >(MapVersion a, MapVersion b)
        {
            return a.version > b.version;
        }

        public override string ToString()
        {
            return $"{GetDeveloperVersion()}.{GetMajorVersion()}.{GetMinorVersion()}.{GetDeveloperVersion()}";
        }
    }
}
