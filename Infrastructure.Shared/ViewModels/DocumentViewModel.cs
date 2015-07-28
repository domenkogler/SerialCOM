using Kogler.Framework;

namespace Kogler.SerialCOM.Infrastructure.Shared
{
    public abstract class DocumentViewModel : PaneViewModel
    {
        protected DocumentViewModel(IView view) : base(view) { }
        //public DocumentViewModel(string title)
        //{
        //    Title = title;

        //    //Set the icon only for open documents (just a test)
        //    //IconSource = ISC.ConvertFromInvariantString(@"pack://application:,,/Images/document.png") as ImageSource;
        //}
    }
}