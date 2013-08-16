using System;
using System.Collections.Generic;
using System.Threading;
using HidLibrary;

namespace DeskToys.Implementations
{
    public class BigRedButton : IStateAwareButton
    {
        private static readonly byte[] initCommand = { 0, 0x03, 0x02, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };
        private static readonly byte[] readStatusCommand = { 0, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x02 };
        private readonly HidDevice device;
        private bool disposed;
        private bool initialized;
        private readonly Timer timer;
        private Thread thread;
        private volatile bool state;

        private BigRedButton(HidDevice device)
        {
            this.device = device;
            this.device.OpenDevice(HidDevice.DeviceMode.Overlapped, HidDevice.DeviceMode.NonOverlapped);
            this.timer = new Timer(Tick, null, TimeSpan.Zero, TimeSpan.FromMilliseconds(20));
            this.thread = new Thread(Poll);
            this.thread.IsBackground = true;
            this.thread.Start();
        }

        public event EventHandler<EventArgs> Down;

        public event EventHandler<EventArgs> Press;

        public event EventHandler<EventArgs> Up;

        public bool State
        {
            get { return this.state; }
        }

        public static IEnumerable<Service> Enumerate()
        {
            foreach (var device in HidDevices.Enumerate(0x1D34, 0x000D))
            {
                yield return new Service<BigRedButton>("Dream Cheeky Big Red Button", () => new BigRedButton(device));
            }
        }

        public void Dispose()
        {
            this.disposed = true;
            this.timer.Dispose();

            if (this.thread != null)
            {
                this.thread.Join();
            }

            this.device.Dispose();
        }

        private void Tick(object state)
        {
            if (!this.initialized)
            {
                this.initialized = true;
                this.device.WriteAsync(initCommand).Wait();
            }

            this.device.WriteAsync(readStatusCommand).Wait();
        }

        private void Poll()
        {
            while (!this.disposed)
            {
                var result = this.device.Read(50);
                if (result.Status == HidDeviceData.ReadStatus.Success)
                {
                    var newState = result.Data[1] == 22;
                    this.UpdateState(newState);
                }
            }
        }

        private void UpdateState(bool newState)
        {
            if (newState != this.state)
            {
                this.state = newState;
                if (newState)
                {
                    var handler = this.Down;
                    if (handler != null)
                    {
                        handler(this, new EventArgs());
                    }
                }
                else
                {
                    var handler = this.Press;
                    if (handler != null)
                    {
                        handler(this, new EventArgs());
                    }

                    handler = this.Up;
                    if (handler != null)
                    {
                        handler(this, new EventArgs());
                    }
                }
            }
        }
    }
}