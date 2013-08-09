using System;

namespace DeskToys
{
    public interface IButton : IDisposable
    {
        event EventHandler<EventArgs> Press;
    }
}
