using System;

namespace DreamCheeky.MissileLauncher
{
    /// <summary>
    /// A launcher.
    /// </summary>
    public interface ILauncher : IDisposable
    {
        void Send(Command command);
    }
}
