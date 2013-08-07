using System;

namespace DreamCheeky.MissileLauncher
{
    public class EdgeChangeEventArgs : EventArgs
    {
        private readonly Edge previousEdges;
        private readonly Edge edges;

        public EdgeChangeEventArgs(Edge previousEdges, Edge edges)
        {
            this.previousEdges = previousEdges;
            this.edges = edges;
        }

        public Edge PreviousEdges
        {
            get { return this.previousEdges; }
        }

        public Edge Edges
        {
            get { return this.edges; }
        }
    }
}
