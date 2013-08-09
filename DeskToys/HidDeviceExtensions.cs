using System.Threading.Tasks;
using HidLibrary;

namespace DeskToys
{
    internal static class HidDeviceExtensions
    {
        public static Task<bool> WriteAsync(this HidDevice device, byte[] data)
        {
            var source = new TaskCompletionSource<bool>();
            device.Write(data, r => { source.SetResult(r); });
            return source.Task;
        }
    }
}
