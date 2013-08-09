using System;
using System.Threading.Tasks;

namespace DeskToys
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
