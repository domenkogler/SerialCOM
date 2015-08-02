using Kogler.Framework;
using Kogler.SerialCOM.PortConfig.Applications;

namespace Kogler.SerialCOM.PortConfig.Presentation
{
    public class PortConfigDesignViewModel : ViewModel, IPortConfigViewModel
    {
        public PortConfigDesignViewModel(IView view) : base(view) { }
    }
}