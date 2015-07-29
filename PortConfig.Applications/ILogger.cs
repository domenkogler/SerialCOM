using System.Text;

namespace Kogler.SerialCOM.PortConfig.Applications
{
    public interface ILogger
    {
        StringBuilder Log { get; }
        void Write(string text, bool inline = false);
    }
}