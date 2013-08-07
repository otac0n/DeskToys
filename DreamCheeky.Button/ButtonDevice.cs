using System;
using System.Linq;
using System.Threading;
using HidLibrary;

namespace DreamCheeky.Button
{
    public class ButtonDevice : IDisposable
    {
        private static readonly byte[] readStatusCommand = { 0, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x02 };
        private readonly HidDevice device;
        private readonly Timer timer;
        private volatile bool state;

        public ButtonDevice(int deviceIndex = 0, int productId = 0x08, int vendorId = 0x1D34)
        {
            this.device = HidDevices.Enumerate(vendorId, productId).Skip(deviceIndex).FirstOrDefault();

            if (this.device == null)
            {
                throw new ArgumentException("The given combination of vendorId, productId, and deviceIndex was invalid: no device found.");
            }

            this.timer = new Timer(Tick, null, TimeSpan.Zero, TimeSpan.FromMilliseconds(10));
        }

        public event EventHandler<EventArgs> Down;

        public event EventHandler<EventArgs> Up;

        public bool State
        {
            get { return this.state; }
        }

        public void Dispose()
        {
            this.timer.Dispose();
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
                var handler = newState
                    ? this.Down
                    : this.Up;

                this.state = newState;
                if (handler != null)
                {
                    handler(this, new EventArgs());
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
