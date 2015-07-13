using GalaSoft.MvvmLight.Ioc;
using Microsoft.Practices.ServiceLocation;

namespace Kogler.SerialCOM
{
    public partial class App
    {
        public App()
        {
            ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);
            SimpleIoc.Default.Register<LoggerService>();
            SimpleIoc.Default.Register<SessionsService>();
            SimpleIoc.Default.Register<DocumentsService>();
            MefService.Init();
        }
    }
}
