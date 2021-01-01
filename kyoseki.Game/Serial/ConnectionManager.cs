﻿using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Threading;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Logging;

namespace kyoseki.Game.Serial
{
    public class ConnectionManager : Component
    {
        private readonly CancellationTokenSource cancellationToken = new CancellationTokenSource();

        private List<SerialPort> ports = new List<SerialPort>();

        public Bindable<ConnectionState> State = new Bindable<ConnectionState>(ConnectionState.Resetting);

        public event Action<string, string> MessageReceived;

        public ConnectionManager()
        {
            var thread = new Thread(run)
            {
                Name = "Serial.ConnectionManager",
                IsBackground = true
            };

            thread.Start();
        }

        private void run()
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                switch (State.Value)
                {
                    case ConnectionState.Ready:
                        foreach (var port in ports)
                        {
                            if (!port.IsOpen)
                            {
                                State.Value = ConnectionState.Resetting;
                                continue;
                            }
                            try
                            {
                                bool reachedEnd = false;
                                int iter = 0;

                                while (!reachedEnd)
                                {
                                    var message = port.ReadLine().Replace(port.NewLine, string.Empty);

                                    MessageReceived?.Invoke(port.PortName, message);

                                    iter++;
                                }
                            }
                            catch (TimeoutException _) { }
                        }
                        break;
                    case ConnectionState.Resetting:
                        ports.ForEach(p => p.Dispose());
                        ports.Clear();

                        foreach (var port in SerialPort.GetPortNames())
                        {
                            Logger.Log($"Connecting to serial port {port}", LoggingTarget.Network);

                            SerialPort s = new SerialPort(port, 115200)
                            {
                                NewLine = "\r\n",
                                ReadTimeout = 1
                            };

                            s.Open();
                            s.BaseStream.Flush();

                            ports.Add(s);
                        }

                        State.Value = ConnectionState.Ready;
                        break;
                }
            }
        }

        protected override void Dispose(bool isDisposing)
        {
            base.Dispose(isDisposing);

            cancellationToken.Cancel();

            ports.ForEach(p => p.Dispose());
        }
    }

    public enum ConnectionState
    {
        Ready,
        Resetting
    }
}