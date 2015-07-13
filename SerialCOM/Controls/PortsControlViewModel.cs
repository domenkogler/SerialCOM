using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO.Ports;
using System.Linq;
using GalaSoft.MvvmLight.CommandWpf;

namespace Kogler.SerialCOM
{
    public class PortsControlViewModel : ViewModelBase
    {
        public PortsControlViewModel(SessionsService sessionsService, LoggerService loggerService, DocumentsService documentsService)
        {
            SessionsService = sessionsService;
            LoggerService = loggerService;
            DocumentsService = documentsService;

            RefreshPortsCommand = new RelayCommand(RefreshPorts);
            OpenPortCommand = new RelayCommand(OpenPort);//, ()=> CanOpenPort);
            //ClosePortCommand = new RelayCommand(async ()=> await ClosePortAsync(), ()=> IsPortOpen);

            RefreshPorts();
            //Model.AddSampleData();
        }

        #region << Properties >>

        private SessionsService SessionsService { get; }
        private LoggerService LoggerService { get; }
        private DocumentsService DocumentsService { get; }
        
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

        public bool CanSelectPort => SelectedPort != null && SessionsService.Sessions.All(s => s.Port.PortName != SelectedPort);

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
            var usedPorts = SessionsService.Sessions.Select(s => s.Port.PortName).ToArray();
            Ports = SerialPort.GetPortNames().Where(p => !usedPorts.Contains(p)).ToArray();
            if (!Ports.Any())
            {
                if (!SessionsService.Sessions.Any()) LoggerService.Logger.Write("No COM ports availible.");
                SelectedPort = null;
            }
            if (SelectedPort == null) SelectedPort = Ports.FirstOrDefault();
        }

        #endregion

        #region << Methods >>

        private void OpenPort()
        {
            var session = new SerialPortSession(SelectedPort, SelectedModel.Value);
            SessionsService.Sessions.Add(session);
            DocumentsService.Documents.Add(session.Model);
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