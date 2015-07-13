using System.ComponentModel.Composition;

namespace Kogler.SerialCOM
{
    [InheritedExport(typeof(ViewModelBase))]
    public abstract class ViewModelBase : GalaSoft.MvvmLight.ViewModelBase
    {

    }
}