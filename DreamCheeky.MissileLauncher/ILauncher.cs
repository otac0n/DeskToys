using System;
using System.Threading.Tasks;

namespace DreamCheeky.MissileLauncher
{
    /// <summary>
    /// A launcher.
    /// </summary>
    public interface ILauncher : IDisposable
    {
        void Send(Command command);

        Task Fire();
    }
}
