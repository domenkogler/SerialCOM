using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.IO.Ports;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Threading;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;

namespace Kogler.SerialCOM
{
    public static class MefService
    {
        static MefService()
        {
            Catalog.Catalogs.Add(new AssemblyCatalog(Assembly.GetExecutingAssembly()));
        }

        public static AggregateCatalog Catalog { get; } = new AggregateCatalog();
        public static CompositionContainer Container { get; } = new CompositionContainer(Catalog);

        public static void ComposeParts(params object[] parts)
        {
            Container.ComposeParts(parts);
        }
    }

    public class MainViewModel : ViewModelBase
    {
        #region << Constructor & Desctructor >>

        public MainViewModel()
        {
            if (IsInDesignMode)
            {
                Sessions.Add(new SerialPortSession("COM1", new BisVista()));
                
            }

            Dispatcher.CurrentDispatcher.BeginInvoke(new Action(() =>
            {

                Anchorables.Add(new AnchorableViewModel {Title = "Ports", Content = Resource("Ports")});
                Anchorables.Add(new AnchorableViewModel {Title = "Sessions", Content = Resource("Sessions")});

            }));

            RefreshPortsCommand = new RelayCommand(RefreshPorts);
            OpenPortCommand = new RelayCommand(OpenPort);//, ()=> CanOpenPort);
            //ClosePortCommand = new RelayCommand(async ()=> await ClosePortAsync(), ()=> IsPortOpen);

            RefreshPorts();
            //Model.AddSampleData();
        }

        //public async override void Cleanup()
        //{
        //    base.Cleanup();
            
        //}

        private static object Resource(string key)
        {
            return Resource<object>(key);
        }

        private static T Resource<T>(string key) where T : class
        {
            return Application.Current.Resources[key] as T;
        }

        

        #endregion

        #region << Properties >>
        
        public Logger Logger { get; } = new Logger();
        public ObservableCollection<SerialPortSession> Sessions { get; } = new ObservableCollection<SerialPortSession>();
        public ObservableCollection<object> Documents { get; } = new ObservableCollection<object>(); 
        public ObservableCollection<object> Anchorables { get; } = new ObservableCollection<object>();

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

        private bool _isSessionsActive;
        public bool IsSessionsActive
        {
            get { return _isSessionsActive; }
            set { Set(ref _isSessionsActive, value); }
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

        private void OpenPort()
        {
            var session = new SerialPortSession(SelectedPort, SelectedModel.Value);
            Sessions.Add(session);
            Documents.Add(session.Model);
            RefreshPorts();
            IsSessionsActive = true;
        }

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