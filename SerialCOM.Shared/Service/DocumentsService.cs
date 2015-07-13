using System.Collections.ObjectModel;

namespace Kogler.SerialCOM
{
    public class DocumentsService
    {
        public ObservableCollection<object> Documents { get; } = new ObservableCollection<object>();
        public ObservableCollection<object> Anchorables { get; } = new ObservableCollection<object>();
    }
}