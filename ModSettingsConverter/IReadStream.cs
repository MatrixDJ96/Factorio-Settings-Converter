
namespace ModSettingsConverter
{
    interface IReadStream
    {
        int Read(byte[] buffer, int size);
        bool EOF();
        int Remaining();
    }
}
