using System;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;

namespace SerialCOM.ViewModel
{
    public class MainViewModel : ViewModelBase
    {
        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
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

            
            OpenCommand = new RelayCommand(OpenPort, () => SelectedPort != null && (Port == null || !Port.IsOpen));
            ReadCommand = new RelayCommand(async ()=> await ReadDataAsync(), ()=> Port != null && Port.IsOpen);
            CloseCommand = new RelayCommand(async () => await ClosePort(), ()=> Port != null && Port.IsOpen);

            Ports = SerialPort.GetPortNames();
            SelectedPort = Ports.FirstOrDefault();
        }

        public async override void Cleanup()
        {
            base.Cleanup();
            if (Port.IsOpen) await ClosePort();
            ReadCommand.RaiseCanExecuteChanged();
        }

        private SerialPort Port { get; set; }
        public string[] Ports { get; }

        private string m_SelectedPort;
        public string SelectedPort
        {
            get { return m_SelectedPort; }
            set
            {
                m_SelectedPort = value;
                OpenCommand.RaiseCanExecuteChanged();
                // ReSharper disable once ExplicitCallerInfoArgument
                RaisePropertyChanged(nameof(CanSelectPort));
            }
        }
        
        public bool CanSelectPort => SelectedPort != null && (Port == null || !Port.IsOpen);

        public FlowDocument Document { get; } = new FlowDocument();

        public RelayCommand OpenCommand { get; }
        public RelayCommand ReadCommand { get; }
        public RelayCommand CloseCommand { get; }

        private void OpenPort()
        {
            try
            {
                if (Port == null) Port = new SerialPort(SelectedPort, 9600, Parity.None, 8, StopBits.One);
                Port.Open();
                Write($"{SelectedPort} port is open.");
                ReadCommand.RaiseCanExecuteChanged();
            }
            catch (Exception)
            {
                
                throw;
            }
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

        private Task ClosePort()
        {
            Task close = new Task(() =>
            {
                try
                {
                    Port.Close();
                    OpenCommand.RaiseCanExecuteChanged();
                    ReadCommand.RaiseCanExecuteChanged();
                    CloseCommand.RaiseCanExecuteChanged();
                }
                catch (IOException)
                {
                    Write("Error closing port: SerialPort was not open.");
                    throw;
                }
            });
            close.Start();
            return close;
        }

        private void Write(string text)
        {
            if (string.IsNullOrEmpty(text)) return;
            var p = new Paragraph();
            var time = new Run(DateTime.Now.ToLongTimeString() + ": ");
            var t = new Run(text);
            p.Inlines.Add(time);
            p.Inlines.Add(t);
            Document.Blocks.Add(p);
        }

    }
}