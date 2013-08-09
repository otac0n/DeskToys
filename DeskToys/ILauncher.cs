using System;
using System.Threading.Tasks;

namespace DeskToys
{
    /// <summary>
    /// A launcher.
    /// </summary>
    public interface ILauncher : IDisposable
    {
        Task Send(Command command);

        Task Fire();
    }
}
