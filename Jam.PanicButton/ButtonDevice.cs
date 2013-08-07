using System;
using System.Linq;
using System.Threading;
using HidLibrary;

namespace Jam.PanicButton
{
    public class ButtonDevice : IDisposable
    {
        private readonly HidDevice device;
        private readonly Timer timer;

        public ButtonDevice(int deviceIndex = 0, int productId = 0x202, int vendorId = 0x1130)
        {
            this.device = HidDevices.Enumerate(vendorId, productId).Where(d => d.Capabilities.NumberFeatureButtonCaps > 0).Skip(deviceIndex).FirstOrDefault();

            if (this.device == null)
            {
                throw new ArgumentException("The given combination of vendorId, productId, and deviceIndex was invalid: no device found.");
            }

            byte[] dummy;
            this.device.ReadFeatureData(out dummy, 0);

            this.timer = new Timer(Tick, null, TimeSpan.Zero, TimeSpan.FromMilliseconds(10));
        }

        public event EventHandler<EventArgs> Click;

        public void Dispose()
        {
            this.timer.Dispose();
        }

        private void Tick(object state)
        {
            byte[] data;
            if (this.device.ReadFeatureData(out data, 0))
            {
                for (var i = 0; i < data[1]; i++)
                {
                    this.RaiseClick();
                }
            }
        }

        private void RaiseClick()
        {
            var handler = this.Click;
            if (handler != null)
            {
                handler(this, new EventArgs());
            }
        }
    }
}
