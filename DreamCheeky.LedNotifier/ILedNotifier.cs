using System;
using System.Drawing;
using System.Threading.Tasks;

namespace DreamCheeky.LedNotifier
{
    public interface ILedNotifier : IDisposable
    {
        Task SetColor(Color color);

        float RedIntensity { get; set; }

        float GreenIntensity { get; set; }

        float BlueIntensity { get; set; }
    }
}
