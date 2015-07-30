using System.Text;

namespace Kogler.SerialCOM.Infrastructure.Shared
{
    public interface ILogger
    {
        StringBuilder Log { get; }
        void Write(string text, bool inline = false);
    }
}