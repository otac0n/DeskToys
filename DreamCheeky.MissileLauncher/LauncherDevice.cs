﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using HidLibrary;

namespace DreamCheeky.MissileLauncher
{
    public class LauncherDevice : IDisposable
    {
        private static readonly Dictionary<Command, byte[]> commands = new Dictionary<Command, byte[]>
        {
            { Command.Down, new byte[] { 0x00, 0x01 } },
            { Command.Up, new byte[] { 0x00, 0x02 } },
            { Command.Left, new byte[] { 0x00, 0x04 } },
            { Command.Right, new byte[] { 0x00, 0x08 } },
            { Command.Fire, new byte[] { 0x00, 0x10 } },
            { Command.Stop, new byte[] { 0x00, 0x20 } },
        };

        private static readonly byte[] readStatusCommand = { 0x00, 0x40 };

        private readonly object sync = new object();
        private readonly HidDevice device;
        private readonly Timer timer;
        private volatile Edge edges;

        public LauncherDevice(int deviceIndex = 0, int productId = 0x0701, int vendorId = 0x0A81)
        {
            this.device = HidDevices.Enumerate(vendorId, productId).Skip(deviceIndex).FirstOrDefault();

            if (this.device == null)
            {
                throw new ArgumentException("The given combination of vendorId, productId, and deviceIndex was invalid: no device found.");
            }

            this.timer = new Timer(Tick, null, TimeSpan.Zero, TimeSpan.FromMilliseconds(100));
        }

        public event EventHandler<EdgeChangeEventArgs> EdgeChange;

        public Edge Edges
        {
            get { return this.edges; }
        }

        public void Dispose()
        {
            this.device.Dispose();
        }

        public void Send(Command command)
        {
            this.WriteSync(commands[command]);
        }

        private void Tick(object state)
        {
            this.WriteSync(readStatusCommand);
            var data = this.device.Read();
            if (data.Status == HidDeviceData.ReadStatus.Success)
            {
                UpdateEdges((DeviceEdge)data.Data[0]);
            }
        }

        private void UpdateEdges(DeviceEdge newEdges)
        {
            var edges = (Edge)(int)(byte)newEdges;
            if (edges != this.edges)
            {
                var handler = this.EdgeChange;

                this.edges = edges;
                if (handler != null)
                {
                    handler(this, new EdgeChangeEventArgs(edges));
                }
            }
        }

        private bool WriteSync(byte[] data)
        {
            lock (this.sync)
            {
                lock (this.device)
                {
                    bool result = false;
                    this.device.Write(data, r =>
                    {
                        lock (this.device)
                        {
                            result = r;
                            Monitor.Pulse(this.device);
                        }
                    });
                    Monitor.Wait(this.device);
                    return result;
                }
            }
        }

        private enum DeviceEdge : byte
        {
            Bottom = 0x01,
            Top = 0x02,
            Left = 0x04,
            Right = 0x08,
            Fire = 0x10,
        }
    }
}
