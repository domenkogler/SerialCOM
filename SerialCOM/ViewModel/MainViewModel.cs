using System;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Documents;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;

namespace Kogler.SerialCOM
{
    public class MainViewModel : ViewModelBase
    {
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
            OpenPortCommand = new RelayCommand(OpenPort, ()=> CanOpenPort);
            ClosePortCommand = new RelayCommand(async ()=> await ClosePortAsync(), ()=> IsPortOpen);

            RefreshPorts();
        }

        private void RefreshPorts()
        {
            Ports = SerialPort.GetPortNames();
            if (!Ports.Any())
            {
                Write("No COM ports availible.");
                SelectedPort = null;
            }
            if (SelectedPort == null) SelectedPort = Ports.FirstOrDefault();
            OpenPortCommand.RaiseCanExecuteChanged();
            RefreshPortsCommand.RaiseCanExecuteChanged();
            ClosePortCommand.RaiseCanExecuteChanged();
        }

        public async override void Cleanup()
        {
            base.Cleanup();
            if (Port.IsOpen) await ClosePortAsync();
        }

        public FlowDocument Document { get; } = new FlowDocument();
        private StringBuilder Log { get; set; }

        private SerialPort Port { get; set; }

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
        
        public bool CanSelectPort => SelectedPort != null && (Port == null || !Port.IsOpen);

        // ReSharper disable once ExplicitCallerInfoArgument
        private void CanSelectPortRaiseCanExecuteChanged() => RaisePropertyChanged(nameof(CanSelectPort));

        public RelayCommand OpenPortCommand { get; }
        public RelayCommand RefreshPortsCommand { get; }
        public RelayCommand ClosePortCommand { get; }

        private bool CanOpenPort => SelectedPort != null && !IsPortOpen;
        private bool IsPortOpen => Port != null && Port.IsOpen;

        private void OpenPort()
        {
            RefreshPorts();
            if (SelectedPort == null) return;
            if (Port != null)
            {
                var pName = Port.PortName;
                Write($"{pName} port is already in use. Closing this port... If this is taking long time try to disconect the port.");
                Task.WaitAll(ClosePortAsync());
                Write($"Port {pName} is closed.");
            }
            Port = new SerialPort(SelectedPort, 9600, Parity.None, 8, StopBits.One);
            Log = new StringBuilder();
            try
            {
                Port.Open();
            }
            catch (Exception e)
            {
                Write(e.Message);
            }
            if (!IsPortOpen) return;
            Write($"{SelectedPort} port is open.");
            CanSelectPortRaiseCanExecuteChanged();
            ReadDataAsync();
        }

        private async void ReadDataAsync()
        {
            byte[] buffer = new byte[4096];
            Task<int> readStringTask = Port.BaseStream.ReadAsync(buffer, 0, 100);

            if (!readStringTask.IsCompleted) Write("Waiting data...");
            int bytesRead = 0;
            try
            {
               bytesRead = await readStringTask;
            }
            catch(IOException e)
            {
                Write(e.Message);
                return;
            }
            string data = Encoding.ASCII.GetString(buffer, 0, bytesRead);
            Write($"Data: {data}");
#pragma warning disable 4014
            readStringTask.ContinueWith(task =>
            {
                if (!IsPortOpen) return;
                ReadDataAsync();
            });
#pragma warning restore 4014
        }

        private Task ClosePortAsync()
        {
            Task close = new Task(async () =>
            {
                try
                {
                    Port.Close();
                    await RunInUI(() => Write($"{Port.PortName} port is closed."));
                    OpenPortCommand.RaiseCanExecuteChanged();
                    ClosePortCommand.RaiseCanExecuteChanged();
                    CanSelectPortRaiseCanExecuteChanged();
                }
                catch (IOException)
                {
                    await RunInUI(() => Write("Error closing port: SerialPort was not open."));
                    throw;
                }
                finally
                {
                    var path = $"{AppDomain.CurrentDomain.BaseDirectory}{SelectedPort}_{DateTime.Now.ToString("dd.MM.yyyy_HH.mm.ss")}.txt";
                    File.AppendAllText(path, Log.ToString());
                    Port.Dispose();
                    Port = null;
                    Log = null;
#pragma warning disable 4014
                    RunInUI(() =>
                    {
                        Document.Blocks.Clear();
                        Write($"Last log was saved to {path}");
                    });
#pragma warning restore 4014
                    CanSelectPortRaiseCanExecuteChanged();
                }
            });
            close.Start();
            return close;
        }

        private static async Task RunInUI(Action action)
        {
            await Application.Current.Dispatcher.BeginInvoke(action);
        }

        private void Write(string text)
        {
            if (string.IsNullOrEmpty(text)) return;
            var p = new Paragraph {TextAlignment = TextAlignment.Left};
            var time = new Run(DateTime.Now.ToLongTimeString() + ": ");
            var t = new Run(text);
            p.Inlines.Add(time);
            p.Inlines.Add(t);
            Document.Blocks.Add(p);
            p.Loaded += ParagrafToView;
            Log?.AppendLine(text);
        }

        private static void ParagrafToView(object sender, RoutedEventArgs e)
        {
            var p = (Paragraph) sender;
            p.Loaded -= ParagrafToView;
            p.BringIntoView();
        }
    }
}