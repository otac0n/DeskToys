using System;

namespace DreamCheeky.MissileLauncher
{
    public class EdgeChangeEventArgs : EventArgs
    {
        private readonly Edge edges;

        public EdgeChangeEventArgs(Edge edges)
        {
            this.edges = edges;
        }

        public Edge Edges
        {
            get { return this.edges; }
        }
    }
}
