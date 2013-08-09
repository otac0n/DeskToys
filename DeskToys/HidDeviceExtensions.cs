using System.Threading;
using HidLibrary;

namespace DeskToys
{
    internal static class HidDeviceExtensions
    {
        public static bool WriteSync(this HidDevice device, byte[] data)
        {
            lock (device)
            {
                bool result = false;
                device.Write(data, r =>
                {
                    lock (device)
                    {
                        result = r;
                        Monitor.Pulse(device);
                    }
                });
                Monitor.Wait(device);
                return result;
            }
        }
    }
}
