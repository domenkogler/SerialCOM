using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;

namespace Kogler.SerialCOM
{
    public class MainViewModel : ViewModelBase
    {
        #region << Constructor & Desctructor >>

        public MainViewModel()
        {
            ////if (IsInDesignMode)
            ////{
            ////    // Code runs in Blend --> create design time data.
            ////}
            ////else
            ////{
            ////    // Code runs "for real"
            ////}

            RefreshPortsCommand = new RelayCommand(RefreshPorts);
            ///OpenPortCommand = new RelayCommand(OpenPort, ()=> CanOpenPort);
            //ClosePortCommand = new RelayCommand(async ()=> await ClosePortAsync(), ()=> IsPortOpen);

            RefreshPorts();
            

            //Model.AddSampleData();
        }

        public async override void Cleanup()
        {
            base.Cleanup();
            
        }

        #endregion

        #region << Properties >>
        
        public Logger Logger { get; } = new Logger();
        public List<SerialPortSession> Sessions { get; } = new List<SerialPortSession>();

        private string[] _ports;
        public string[] Ports
        {
            get { return _ports; }
            private set { Set(ref _ports, value); }
        }

        private string _selectedPort;
        public string SelectedPort
        {
            get { return _selectedPort; }
            set { Set(ref _selectedPort, value); }
        }
        
        public bool CanSelectPort => SelectedPort != null && Sessions.All(s => s.Port.PortName != SelectedPort);

        #endregion

        #region << Commands >>

        public RelayCommand RefreshPortsCommand { get; }
        public RelayCommand OpenPortCommand { get; }
        public RelayCommand ClosePortCommand { get; }
        
        private void RefreshPorts()
        {
            Ports = SerialPort.GetPortNames();
            if (!Ports.Any())
            {
                Logger.Write("No COM ports availible.");
                SelectedPort = null;
            }
            if (SelectedPort == null) SelectedPort = Ports.FirstOrDefault();
            //OpenPortCommand.RaiseCanExecuteChanged();
            //RefreshPortsCommand.RaiseCanExecuteChanged();
            //ClosePortCommand.RaiseCanExecuteChanged();
        }
        
        #endregion

        #region << Methods >>
        
        //private void SaveFiles()
        //{
        //    var file = $"{AppDomain.CurrentDomain.BaseDirectory}{SelectedPort}_{DateTime.Now.ToString("dd.MM.yyyy_HH.mm.ss")}";
        //    File.AppendAllText($"{file}.txt", Log.ToString());
        //    File.AppendAllText($"{file}.cvs", Model.ToString());
        //    RunInUI(() =>
        //    {
        //        Document.Blocks.Clear();
        //        Write($"Log was saved to {file}.txt");
        //        Write($"Data was saved to {file}.cvs");
        //    });
        //}

        #endregion
    }
}