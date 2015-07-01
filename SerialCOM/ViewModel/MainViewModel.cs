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

            
            OpenCommand = new RelayCommand(OpenPort, CanOpenPort);
            ReadCommand = new RelayCommandAsync(ReadDataAsync, IsPortOpen);
            CloseCommand = new RelayCommandAsync(ClosePortAsync, IsPortOpen);

            Ports = SerialPort.GetPortNames();
            SelectedPort = Ports.FirstOrDefault();
            if (SelectedPort == null) Write("No COM ports availible.");
        }

        public async override void Cleanup()
        {
            base.Cleanup();
            if (Port.IsOpen) await ClosePortAsync();
            ReadCommand.RaiseCanExecuteChanged();
        }

        public FlowDocument Document { get; } = new FlowDocument();
        private StringBuilder Log { get; set; }

        private SerialPort Port { get; set; }
        public string[] Ports { get; }

        private string _selectedPort;
        public string SelectedPort
        {
            get { return _selectedPort; }
            set
            {
                _selectedPort = value;
                OpenCommand.RaiseCanExecuteChanged();
            }
        }
        
        public bool CanSelectPort => SelectedPort != null && (Port == null || !Port.IsOpen);
        
        public RelayCommand OpenCommand { get; }
        public RelayCommand ReadCommand { get; }
        public RelayCommand CloseCommand { get; }

        private Func<bool> CanOpenPort => () => SelectedPort != null && !IsPortOpen();
        private Func<bool> IsPortOpen => () => Port != null && Port.IsOpen;

        private void OpenPort()
        {
            if (Port != null) throw new InvalidOperationException($"{Port.PortName} port is already in use.");
            Port = new SerialPort(SelectedPort, 9600, Parity.None, 8, StopBits.One);
            Log = new StringBuilder();
            Port.Open();
            Write($"{SelectedPort} port is open.");
            ReadCommand.RaiseCanExecuteChanged();
            // ReSharper disable once ExplicitCallerInfoArgument
            RaisePropertyChanged(nameof(CanSelectPort));
        }

        private async Task ReadDataAsync()
        {
            byte[] buffer = new byte[4096];
            Task<int> readStringTask = Port.BaseStream.ReadAsync(buffer, 0, 100);

            if (!readStringTask.IsCompleted) Write("Waiting data...");
            int bytesRead = 0;
            try
            {
               bytesRead = await readStringTask;
            }
            catch(IOException)
            {
            }
            string data = Encoding.ASCII.GetString(buffer, 0, bytesRead);
            Write(data);
        }

        private Task ClosePortAsync()
        {
            Task close = new Task(async () =>
            {
                try
                {
                    Port.Close();
                    await RunInUI(() => Write($"{Port.PortName} port is closed."));
                    OpenCommand.RaiseCanExecuteChanged();
                    ReadCommand.RaiseCanExecuteChanged();
                    CloseCommand.RaiseCanExecuteChanged();
                    RaisePropertyChanged(nameof(CanSelectPort));
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
                    RaisePropertyChanged(nameof(CanSelectPort));
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