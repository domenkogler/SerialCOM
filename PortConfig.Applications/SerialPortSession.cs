﻿using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
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

        [Import(typeof(ILogger), RequiredCreationPolicy = CreationPolicy.NonShared)]
        public ILogger Logger { get; internal set; }
        public SerialModel Model { get; }
        public System.IO.Ports.SerialPort Port { get; }

        public KeyValuePair<string, object>[] Documents =>
            new[]
            {
                new KeyValuePair<string, object>(Model.Description, Model),
                new KeyValuePair<string, object>("Log", Logger)
            };

        private void OpenPort()
        {
            try
            {
                Port.Open();
            }
            catch (Exception e)
            {
                Logger.Write(e.Message);
            }
            if (!Port.IsOpen) return;
            Logger.Write($"{Port.PortName} port is open.");
            ReadDataAsync();
        }

        private Task ClosePortAsync()
        {
            var close = new Task(() =>
            {
                try
                {
                    Port.Close();
                    Logger.Write($"{Port.PortName} port is closed.");
                }
                catch (IOException)
                {
                    Logger.Write("Error closing port: SerialPort was not open.");
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
                    Logger.Write(e.Message);
                    return;
                }
                if (read <= 2) continue;
                string data = Encoding.ASCII.GetString(buffer, 0, bytesRead);
                var inline = !Model.AddData(data);
                Logger.Write(data, inline);

                buffer = new byte[10240];
                bytesRead = 0;
            }
        }
    }
}