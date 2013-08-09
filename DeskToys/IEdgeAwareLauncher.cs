using System;
using System.Threading.Tasks;

namespace DeskToys
{
    /// <summary>
    /// A launcher that can detect the its edges.
    /// </summary>
    public interface IEdgeAwareLauncher : IResettableLauncher
    {
        /// <summary>
        /// Fired when the launcher's detected edge changes.
        /// </summary>
        event EventHandler<EdgeChangeEventArgs> EdgeChange;

        /// <summary>
        /// Gets the currently detected edges of the launcher.
        /// </summary>
        Edge Edges { get; }
    }

    internal static class IEdgeAwareLauncherTraits
    {
        /// <summary>
        /// Provides <see cref="IResetttableLauncher.Reset"/> for any <see cref="IEdgeAwareLauncher"/>.
        /// </summary>
        /// <param name="this">The launcher to reset.</param>
        /// <param name="edges">The edges to which the launcher should be reset.</param>
        /// <returns>A task that will be completed once the launcher has reached the specified edges.</returns>
        public static async Task Reset(IEdgeAwareLauncher @this, Edge edges)
        {
            if (edges.HasFlag(Edge.Fire))
            {
                throw new ArgumentOutOfRangeException("edges");
            }

            var bottom = edges.HasFlag(Edge.Bottom);
            var top = edges.HasFlag(Edge.Top);
            var left = edges.HasFlag(Edge.Left);
            var right = edges.HasFlag(Edge.Right);

            if ((top && bottom) || (left && right))
            {
                throw new ArgumentOutOfRangeException("edges");
            }

            if (left)
            {
                await @this.Go(Edge.Left, Command.Left);
            }
            else if (right)
            {
                await @this.Go(Edge.Right, Command.Right);
            }

            if (top)
            {
                await @this.Go(Edge.Top, Command.Up);
            }
            else if (bottom)
            {
                await @this.Go(Edge.Bottom, Command.Down);
            }
        }

        private static async Task Go(this IEdgeAwareLauncher @this, Edge edge, Command command)
        {
            var flag = new AsyncAutoResetEvent();
            EventHandler<EdgeChangeEventArgs> handler;
            @this.EdgeChange += (handler = (o, e) =>
            {
                if (e.Edges.HasFlag(edge))
                {
                    flag.Set();
                }
            });

            if (!@this.Edges.HasFlag(edge))
            {
                await @this.Send(command);
                await flag.WaitAsync();
            }

            @this.EdgeChange -= handler;
        }

        public static async Task Fire(this IEdgeAwareLauncher @this)
        {
            var flag = new AsyncAutoResetEvent();
            EventHandler<EdgeChangeEventArgs> handler;
            @this.EdgeChange += (handler = (o, e) =>
            {
                if (e.PreviousEdges.HasFlag(Edge.Fire) && !e.Edges.HasFlag(Edge.Fire))
                {
                    flag.Set();
                }
            });

            await @this.Send(Command.Fire);
            await flag.WaitAsync();

            @this.EdgeChange -= handler;
        }
    }
}
