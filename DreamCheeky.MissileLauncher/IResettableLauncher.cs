using System.Threading.Tasks;

namespace DreamCheeky.MissileLauncher
{
    public interface IResettableLauncher : ILauncher
    {
        Task Reset(Edge edges);
    }
}
