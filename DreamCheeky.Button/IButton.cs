using System;

namespace DreamCheeky.Button
{
    public interface IButton : IDisposable
    {
        public event EventHandler<EventArgs> Press;
    }
}
