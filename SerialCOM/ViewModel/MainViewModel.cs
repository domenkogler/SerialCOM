using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
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
            OpenPortCommand = new RelayCommand(OpenPort, ()=> CanOpenPort);
            ClosePortCommand = new RelayCommand(async ()=> await ClosePortAsync(), ()=> IsPortOpen);

            RefreshPorts();
        }

        public async override void Cleanup()
        {
            base.Cleanup();
            if (Port.IsOpen) await ClosePortAsync();
        }

        #endregion

        #region << Properties >>

        public FlowDocument Document { get; } = new FlowDocument();
        private StringBuilder Log { get; set; }
        private SerialPort Port { get; set; }
        public BisModel Model { get; private set; }

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

        #endregion

        #region << Commands >>

        public RelayCommand RefreshPortsCommand { get; }
        public RelayCommand OpenPortCommand { get; }
        public RelayCommand ClosePortCommand { get; }

        private bool CanOpenPort => SelectedPort != null && !IsPortOpen;
        private bool IsPortOpen => Port != null && Port.IsOpen;
        // ReSharper disable once ExplicitCallerInfoArgument
        private void CanSelectPortRaiseCanExecuteChanged() => RaisePropertyChanged(nameof(CanSelectPort));

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
            InitProperties();
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

        private Task ClosePortAsync()
        {
            var close = new Task(() =>
            {
                try
                {
                    Port.Close();
                    Write($"{Port.PortName} port is closed.");
                    OpenPortCommand.RaiseCanExecuteChanged();
                    ClosePortCommand.RaiseCanExecuteChanged();
                    CanSelectPortRaiseCanExecuteChanged();
                }
                catch (IOException)
                {
                    Write("Error closing port: SerialPort was not open.");
                    throw;
                }
                finally
                {
                    SaveFiles();
                    CleanupProperties();
                    CanSelectPortRaiseCanExecuteChanged();
                }
            });
            close.Start();
            return close;
        }

        #endregion

        #region << Methods >>

        private void InitProperties()
        {
            Port = new SerialPort(SelectedPort, 9600, Parity.None, 8, StopBits.One);
            Log = new StringBuilder();
            Model = new BisModel();
        }

        private void CleanupProperties()
        {
            Port.Dispose();
            Port = null;
            Log = null;
            Model = null;
        }

        private void SaveFiles()
        {
            var file = $"{AppDomain.CurrentDomain.BaseDirectory}{SelectedPort}_{DateTime.Now.ToString("dd.MM.yyyy_HH.mm.ss")}";
            File.AppendAllText($"{file}.txt", Log.ToString());
            File.AppendAllText($"{file}.cvs", Model.ToString());
            RunInUI(() =>
            {
                Document.Blocks.Clear();
                Write($"Log was saved to {file}.txt");
                Write($"Data was saved to {file}.cvs");
            });
        }

        private byte[] buffer = new byte[10240];
        private int bytesRead = 0;
        private async void ReadDataAsync()
        {
            while (IsPortOpen)
            {
                var stream = Port.BaseStream;
                Task<int> readStringTask = stream.ReadAsync(buffer, bytesRead, 1024);
                int read = 0;
                try
                {
                    read = await readStringTask;
                    //if (read > 1)
                    bytesRead += read;
                }
                catch (IOException e)
                {
                    Write(e.Message);
                    return;
                }
                if (read <= 2) continue;
                string data = Encoding.ASCII.GetString(buffer, 0, bytesRead);
                var inline = !Model.AddData(data);
                Write(data, inline);

                buffer = new byte[10240];
                bytesRead = 0;
            }
        }
        
        private void Write(string text, bool inline = false)
        {
            if (string.IsNullOrEmpty(text)) return;
            RunInUI(() =>
            {
                Paragraph p = null;
                if (inline)
                {
                    p = Document.Blocks.LastBlock as Paragraph;
                }
                if (p == null)
                {
                    p = new Paragraph { TextAlignment = TextAlignment.Left };
                    Document.Blocks.Add(p);
                }
                //var time = new Run(DateTime.Now.ToLongTimeString() + ": ");
                //p.Inlines.Add(time);
                var t = new Run(text);
                p.Loaded += ParagrafToView;
                p.Inlines.Add(t);
                Log?.AppendLine(text);
            });
        }

        private static async void RunInUI(Action action)
        {
            if (SynchronizationContext.Current != null) action();
            else await Application.Current.Dispatcher.BeginInvoke(action);
        }

        private static void ParagrafToView(object sender, RoutedEventArgs e)
        { 
            var p = (Paragraph) sender;
            p.Loaded -= ParagrafToView;
            p.BringIntoView();
        }

        #endregion
    }

    public static class GridViewColumns
    {
        [AttachedPropertyBrowsableForType(typeof (GridView))]
        public static IEnumerable<GridViewColumn> GetColumnsSource(DependencyObject obj)
        {
            return (IEnumerable<GridViewColumn>) obj.GetValue(ColumnsSourceProperty);
        }

        public static void SetColumnsSource(DependencyObject obj, IEnumerable<GridViewColumn> value)
        {
            obj.SetValue(ColumnsSourceProperty, value);
        }

        // Using a DependencyProperty as the backing store for ColumnsSource.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ColumnsSourceProperty =
            DependencyProperty.RegisterAttached(
                "ColumnsSource",
                typeof(IEnumerable<GridViewColumn>),
                typeof(GridViewColumns),
                new UIPropertyMetadata(
                    null,
                    ColumnsSourceChanged));

        private static void ColumnsSourceChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            GridView gridView = obj as GridView;
            IEnumerable<GridViewColumn> columns = e.NewValue as IEnumerable<GridViewColumn>;
            if (gridView == null || columns == null) return;
            gridView.Columns.Clear();
            foreach (var column in columns)
            {
                gridView.Columns.Add(column);
            }
        }
    }
}