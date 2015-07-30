using System;
using System.Diagnostics;
using System.IO;
using System.IO.Ports;
using System.Text;
using System.Threading.Tasks;
using Kogler.SerialCOM.Infrastructure.Shared;

namespace Kogler.SerialCOM.PortConfig.Applications
{
    public class SerialPortSession : IDisposable
    {
        public SerialPortSession(string portName, SerialModel model)
        {
            Model = model;
            Port = model.GetSerialPort(portName);
            OpenPort();
        }

        public void Dispose()
        {
            Task.WaitAll(ClosePortAsync());
            Port.Dispose();
        }

        public SerialModel Model { get; }
        public SerialPort Port { get; }

        private void OpenPort()
        {
            try
            {
                Port.Open();
            }
            catch (Exception e)
            {
                Trace.TraceError(e.Message);
            }
            if (!Port.IsOpen) return;
            Model.Logger.Write($"{Port.PortName} port is open.");
            ReadDataAsync();
        }

        private Task ClosePortAsync()
        {
            var close = new Task(() =>
            {
                try
                {
                    Port.Close();
                    Model.Logger.Write($"{Port.PortName} port is closed.");
                }
                catch (IOException)
                {
                    Model.Logger.Write("Error closing port: SerialPort was not open.");
                    throw;
                }
                finally
                {
                    //SaveFiles();
                }
            });
            close.Start();
            return close;
        }

        private byte[] buffer = new byte[10240];
        private int bytesRead = 0;
        private async void ReadDataAsync()
        {
            while (Port.IsOpen)
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
                    Model.Logger.Write(e.Message);
                    return;
                }
                if (read <= 2) continue;
                string data = Encoding.ASCII.GetString(buffer, 0, bytesRead);
                var inline = !Model.AddData(data);
                Model.Logger.Write(data, inline);

                buffer = new byte[10240];
                bytesRead = 0;
            }
        }
    }
}