using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.IO.Ports;
using System.Linq;
using System.Reflection;
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
            ComposeMEF();

            //Model.AddSampleData();
        }

        public async override void Cleanup()
        {
            base.Cleanup();
            
        }

        private void ComposeMEF()
        {
            var catalog = new AssemblyCatalog(Assembly.GetExecutingAssembly());
            var container = new CompositionContainer(catalog);
            container.ComposeParts(this);
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
            set { Set(ref _selectedPort, value);}
        }

        public bool CanSelectPort => SelectedPort != null && Sessions.All(s => s.Port.PortName != SelectedPort);
        
        private IEnumerable<Lazy<SerialModel, ISerialModelDescription>> _models;
        [ImportMany(typeof(SerialModel))]
        public IEnumerable<Lazy<SerialModel, ISerialModelDescription>> Models
        {
            get { return _models; }
            set
            {
                Set(ref _models, value);
                SelectedModel = Models.FirstOrDefault();
            }
        }

        private Lazy<SerialModel, ISerialModelDescription> _selectedModel;
        public Lazy<SerialModel, ISerialModelDescription> SelectedModel
        {
            get { return _selectedModel; }
            set { Set(ref _selectedModel, value); }
        }

        #endregion

        #region << Commands >>

        public RelayCommand RefreshPortsCommand { get; }
        public RelayCommand OpenPortCommand { get; }
        public RelayCommand ClosePortCommand { get; }
        
        private void RefreshPorts()
        {
            var usedPorts = Sessions.Select(s => s.Port.PortName).ToArray();
            Ports = SerialPort.GetPortNames().Where(p => !usedPorts.Contains(p)).ToArray();
            if (!Ports.Any())
            {
                if (!Sessions.Any()) Logger.Write("No COM ports availible.");
                SelectedPort = null;
            }
            if (SelectedPort == null) SelectedPort = Ports.FirstOrDefault();
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