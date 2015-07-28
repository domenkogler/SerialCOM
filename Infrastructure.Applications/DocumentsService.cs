using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using Kogler.Framework;
using Kogler.SerialCOM.Infrastructure.Shared;

namespace Kogler.SerialCOM.Infrastructure.Applications
{
    public interface IDocumentsService
    {
        ObservableCollection<PaneViewModel> Documents { get; }
        ObservableCollection<PaneViewModel> Anchorables { get; }

        PaneViewModel ActiveContent { get; set; }
    }

    [Export(typeof(IDocumentsService)), Export]
    internal class DocumentsService : Model, IDocumentsService
    {
        public ObservableCollection<PaneViewModel> Documents { get; } = new ObservableCollection<PaneViewModel>();
        public ObservableCollection<PaneViewModel> Anchorables { get; } = new ObservableCollection<PaneViewModel>();

        private PaneViewModel activeContent;
        public PaneViewModel ActiveContent
        {
            get { return activeContent; }
            set { Set(ref activeContent, value); }
        }
    }
}