using System.ComponentModel.Composition;

namespace Kogler.SerialCOM
{
    [InheritedExport(typeof(AnchorableViewModel))]
    public abstract class AnchorableViewModel : PaneViewModel
    {
        private bool _isVisible = true;
        public bool IsVisible
        {
            get { return _isVisible; }
            set { Set(ref _isVisible, value); }
        }
    }
}