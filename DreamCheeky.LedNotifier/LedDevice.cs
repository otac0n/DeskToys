using System;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using HidLibrary;

namespace DreamCheeky.LedNotifier
{
    public class LedDevice : IDisposable
    {
        public static readonly int MailboxFriendsAlertProductId = 0x0A;
        public static readonly int WebMailNotifierProductId = 0x04;

        private static readonly byte[] colorData = { 0, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x1F, 0x05 };

        private static readonly byte[][] initData =
        {
            new byte[] { 0, 0x1F, 0x02, 0x00, 0x5F, 0x00, 0x00, 0x1F, 0x03 },
            new byte[] { 0, 0x00, 0x02, 0x00, 0x5F, 0x00, 0x00, 0x1F, 0x04 },
            new byte[] { 0, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x01 },
        };

        private readonly HidDevice device;
        private bool initialized;
        private byte redRange = 60;
        private byte greenRange = 60;
        private byte blueRange = 60;

        public LedDevice(int deviceIndex = 0, int productId = 0x0A, int vendorId = 0x1D34)
        {
            this.device = HidDevices.Enumerate(vendorId, productId).Skip(deviceIndex).FirstOrDefault();

            if (this.device == null)
            {
                throw new ArgumentException("The given combination of vendorId, productId, and deviceIndex was invalid: no device found.");
            }
        }

        public byte RedRange
        {
            get { return this.redRange; }
            set { this.redRange = value; }
        }

        public byte GreenRange
        {
            get { return this.greenRange; }
            set { this.greenRange = value; }
        }

        public byte BlueRange
        {
            get { return this.blueRange; }
            set { this.blueRange = value; }
        }

        public static int GetDeviceCount(int vendorId, int productId)
        {
            return HidDevices.Enumerate(vendorId, productId).Count();
        }

        public Task SetColor(Color color)
        {
            var data = new byte[colorData.Length];
            Array.Copy(colorData, data, colorData.Length);
            data[1] = (byte)Math.Floor(this.redRange * (color.R / 255.0));
            data[2] = (byte)Math.Floor(this.greenRange * (color.G / 255.0));
            data[3] = (byte)Math.Floor(this.blueRange * (color.B / 255.0));

            return Write(data);
        }

        private bool Initialize()
        {
            bool success = true;
            for (int i = 0; i < initData.Length; i++)
            {
                success = success && this.WriteSync(initData[i]);
            }
            this.initialized = true;
            return success;
        }

        private Task<bool> Write(byte[] data)
        {
            return Task.Factory.StartNew(() =>
            {
                bool success = this.initialized
                    ? true
                    : this.Initialize();

                return success && this.WriteSync(data);
            });
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

        public void Dispose()
        {
            this.device.Dispose();
        }
    }
}
