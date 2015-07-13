using System.Collections.ObjectModel;

namespace Kogler.SerialCOM
{
    public class SessionsService
    {
        public ObservableCollection<SerialPortSession> Sessions { get; } = new ObservableCollection<SerialPortSession>();
    }
}