using System.Threading.Tasks;

namespace DeskToys
{
    /// <summary>
    /// A launcher that can reset to an edge.
    /// </summary>
    /// <remarks>
    /// All launchers are potentially resettable, but non-edge-aware launchers must use a delay-based approach to resetting.
    /// </remarks>
    public interface IResettableLauncher : ILauncher
    {
        Task Reset(Edge edges);
    }
}
