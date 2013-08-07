using System;

namespace DreamCheeky.MissileLauncher
{
    public interface ILauncher : IDisposable
    {
        void Send(Command command);
    }
}
