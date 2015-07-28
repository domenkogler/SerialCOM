using Kogler.Framework;

namespace Kogler.SerialCOM.Infrastructure.Shared
{
    public abstract class AnchorableViewModel : PaneViewModel
    {
        protected AnchorableViewModel(IView view) : base(view) { }

        private bool isVisible = true;
        public bool IsVisible
        {
            get { return isVisible; }
            set { Set(ref isVisible, value); }
        }
    }
}