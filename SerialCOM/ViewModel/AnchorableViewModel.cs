namespace Kogler.SerialCOM
{
    class AnchorableViewModel : PaneViewModel
    {
        private bool _isVisible = true;
        public bool IsVisible
        {
            get { return _isVisible; }
            set { Set(ref _isVisible, value); }
        }
    }
}