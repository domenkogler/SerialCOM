using System.Windows.Media;
using Kogler.Framework;

namespace Kogler.SerialCOM.Infrastructure.Shared
{
    public abstract class PaneViewModel : ViewModel
    {
        protected PaneViewModel(IView view) : base(view) { }

        public static ImageSourceConverter ISC = new ImageSourceConverter();

        #region Properties

        private string title;
        public string Title
        {
            get { return title; }
            set { Set(ref title, value); }
        }

        public ImageSource IconSource { get; protected set; }

        private string contentId;
        public string ContentId
        {
            get { return contentId; }
            set { Set(ref contentId, value); }
        }

        private bool isSelected;
        public bool IsSelected
        {
            get { return isSelected; }
            set { Set(ref isSelected, value); }
        }

        private bool isActive;
        public bool IsActive
        {
            get { return isActive; }
            set { Set(ref isActive, value); }
        }

        public object Content { get; set; }

        #endregion
    }
}