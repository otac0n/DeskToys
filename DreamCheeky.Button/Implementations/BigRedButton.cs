using System;
using System.Collections.Generic;
using System.Threading;
using HidLibrary;

namespace DreamCheeky.Button.Implementations
{
    public class BigRedButton : IButton
    {
        private static readonly byte[] readStatusCommand = { 0, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x02 };
        private readonly HidDevice device;
        private readonly Timer timer;
        private volatile bool state;

        private BigRedButton(HidDevice device)
        {
            this.device = device;
            this.timer = new Timer(Tick, null, TimeSpan.Zero, TimeSpan.FromMilliseconds(10));
        }

        public event EventHandler<EventArgs> Down;

        public event EventHandler<EventArgs> Press;

        public event EventHandler<EventArgs> Up;

        public bool State
        {
            get { return this.state; }
        }

        internal static IEnumerable<Service> Enumerate()
        {
            foreach (var device in HidDevices.Enumerate(0x1D34, 0x0008))
            {
                yield return new Service<BigRedButton>(() => new BigRedButton(device));
            }
        }

        public void Dispose()
        {
            this.timer.Dispose();
            this.device.Dispose();
        }

        private void Tick(object state)
        {
            var success = this.WriteSync(readStatusCommand);
            if (success)
            {
                var result = this.device.Read();
                if (result.Status == HidDeviceData.ReadStatus.Success)
                {
                    var newState = result.Data[1] == 0x1C;
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

        private bool WriteSync(byte[] data)
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
}
