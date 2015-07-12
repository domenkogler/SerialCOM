using System.Windows;
using System.Windows.Media;
using GalaSoft.MvvmLight;

namespace Kogler.SerialCOM
{
    class PaneViewModel : ViewModelBase
    {
        public static ImageSourceConverter ISC = new ImageSourceConverter();

        #region Properties

        private string _title;
        public string Title
        {
            get { return _title; }
            set { Set(ref _title, value); }
        }
        
        public ImageSource IconSource { get; protected set; }
        
        private string _contentId;
        public string ContentId
        {
            get { return _contentId; }
            set { Set(ref _contentId, value); }
        }

        private bool _isSelected;
        public bool IsSelected
        {
            get { return _isSelected; }
            set { Set(ref _isSelected, value); }
        }

        private bool _isActive;
        public bool IsActive
        {
            get { return _isActive; }
            set { Set(ref _isSelected, value); }
        }

        public object Content { get; set; }

        #endregion
    }
}