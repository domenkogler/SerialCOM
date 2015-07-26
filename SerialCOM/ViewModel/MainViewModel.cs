using System.Collections.Generic;
using System.ComponentModel.Composition;
using Kogler.Framework;

namespace Kogler.SerialCOM
{
    public class MainViewModel : ViewModelBase
    {
        #region << Constructor & Desctructor >>

        public MainViewModel(DocumentsService documentsService)
        {
            DocumentsService = documentsService;

            if (Dispatcher.IsInDesignMode)
            {
                //var sessions = SimpleIoc.Default.GetInstance<SessionsService>().Sessions;
                //sessions.Add(new SerialPortSession("COM1", new BisVista()));
            }

            //Dispatcher.CurrentDispatcher.BeginInvoke(new Action(() =>
            //{

            //    Anchorables.Add(new AnchorableViewModel {Title = "Ports", Content = Resource("Ports")});
            //    Anchorables.Add(new AnchorableViewModel {Title = "Sessions", Content = Resource("Sessions")});

            //}));
        }

        public DocumentsService DocumentsService { get; }

        [ImportMany(typeof(AnchorableViewModel))]
        public IEnumerable<AnchorableViewModel> Anchorables
        {
            set
            {
                DocumentsService.Anchorables.Clear();
                foreach (var anchorable in value)
                {
                    DocumentsService.Anchorables.Add(anchorable);
                }
            }
        }

        //public async override void Cleanup()
        //{
        //    base.Cleanup();
            
        //}

        //private static object Resource(string key)
        //{
        //    return Resource<object>(key);
        //}

        //private static T Resource<T>(string key) where T : class
        //{
        //    return Application.Current.Resources[key] as T;
        //}

        #endregion
    }
}