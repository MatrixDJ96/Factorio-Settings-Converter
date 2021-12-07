
namespace ModSettingsConverter
{
    interface IWriteStream
    {
        void Write(byte[] data);
        void Write(byte data);
    }
}
