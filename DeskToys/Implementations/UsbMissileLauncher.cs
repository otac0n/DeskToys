using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using HidLibrary;

namespace DeskToys.Implementations
{
    internal class UsbMissileLauncher : IEdgeAwareLauncher
    {
        private static readonly Dictionary<Command, byte[]> commands = new Dictionary<Command, byte[]>
        {
            { Command.Down, new byte[] { 0x00, 0x01 } },
            { Command.Up, new byte[] { 0x00, 0x02 } },
            { Command.Left, new byte[] { 0x00, 0x04 } },
            { Command.Right, new byte[] { 0x00, 0x08 } },
            { Command.Fire, new byte[] { 0x00, 0x10 } },
            { Command.Stop, new byte[] { 0x00, 0x20 } },
        };

        private static readonly byte[] readStatusCommand = { 0x00, 0x40 };

        private readonly HidDevice device;
        private readonly Timer timer;
        private int edges;

        private UsbMissileLauncher(HidDevice device)
        {
            this.device = device;
            this.Tick(null);
            this.timer = new Timer(Tick, null, TimeSpan.FromMilliseconds(100), TimeSpan.FromMilliseconds(100));
        }

        public static IEnumerable<Service> Enumerate()
        {
            foreach (var device in HidDevices.Enumerate(0x0A81, 0x0701))
            {
                yield return new Service<UsbMissileLauncher>(() => new UsbMissileLauncher(device));
            }
        }

        public event EventHandler<EdgeChangeEventArgs> EdgeChange;

        public Edge Edges
        {
            get { return (Edge)this.edges; }
        }

        public void Dispose()
        {
            this.timer.Dispose();
            this.device.Dispose();
        }

        public async Task Send(Command command)
        {
            await this.device.WriteAsync(commands[command]);
        }

        public Task Reset(Edge edges)
        {
            return IEdgeAwareLauncherTraits.Reset(this, edges);
        }

        public async Task Fire()
        {
            await IEdgeAwareLauncherTraits.Fire(this);
            await Task.Delay(TimeSpan.FromSeconds(2));
        }

        private void Tick(object state)
        {
            var success = this.device.WriteAsync(readStatusCommand).Result;
            if (success)
            {
                var data = this.device.Read();
                if (data.Status == HidDeviceData.ReadStatus.Success)
                {
                    this.UpdateEdges((DeviceEdge)data.Data[1]);
                }
            }
        }

        private void UpdateEdges(DeviceEdge newEdges)
        {
            var edges = (Edge)(int)(byte)newEdges;
            var previousEdges = (Edge)this.edges;
            if (edges != previousEdges && Interlocked.CompareExchange(ref this.edges, (int)edges, (int)previousEdges) == (int)previousEdges)
            {
                var handler = this.EdgeChange;
                if (handler != null)
                {
                    handler(this, new EdgeChangeEventArgs(previousEdges, edges));
                }
            }
        }

        private enum DeviceEdge : byte
        {
            Bottom = 0x01,
            Top = 0x02,
            Left = 0x04,
            Right = 0x08,
            Fire = 0x10,
        }
    }
}
