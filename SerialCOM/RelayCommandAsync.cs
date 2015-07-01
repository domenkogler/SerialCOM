using System;
using System.Threading.Tasks;
using GalaSoft.MvvmLight.CommandWpf;

namespace Kogler.SerialCOM
{
    public class RelayCommandAsync : RelayCommand
    {
        public RelayCommandAsync(Func<Task> execute) :
            base(async () => await execute())
        { }

        public RelayCommandAsync(Func<Task> execute, Func<bool> canExecute) :
            base(async () => await execute(), canExecute)
        { }
    }
}
