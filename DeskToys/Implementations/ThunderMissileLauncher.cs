using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using HidLibrary;

namespace DeskToys.Implementations
{
    internal class ThunderMissileLauncher : IResettableLauncher
    {
        private static readonly Dictionary<Command, byte[]> commands = new Dictionary<Command, byte[]>
        {
            { Command.Down,  new byte[] { 0, 0x02, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 } },
            { Command.Up,    new byte[] { 0, 0x02, 0x02, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 } },
            { Command.Left,  new byte[] { 0, 0x02, 0x04, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 } },
            { Command.Right, new byte[] { 0, 0x02, 0x08, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 } },
            { Command.Fire,  new byte[] { 0, 0x02, 0x10, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 } },
            { Command.Stop,  new byte[] { 0, 0x02, 0x20, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 } },
        };

        private readonly HidDevice device;

        private ThunderMissileLauncher(HidDevice device)
        {
            this.device = device;
        }

        public int MissileCount
        {
            get { return 4; }
        }

        public static IEnumerable<Service> Enumerate()
        {
            foreach (var device in HidDevices.Enumerate(0x2123, 0x1010))
            {
                yield return new Service<ThunderMissileLauncher>("Dream Cheeky Thunder Missile Launcher", () => new ThunderMissileLauncher(device));
            }
        }

        public void Dispose()
        {
            this.device.Dispose();
        }

        public async Task Send(Command command)
        {
            await this.device.WriteAsync(commands[command]);
        }

        public async Task Reset(Edge edges)
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
                await this.Send(Command.Left);
                await Task.Delay(TimeSpan.FromSeconds(6.5));
            }
            else if (right)
            {
                await this.Send(Command.Right);
                await Task.Delay(TimeSpan.FromSeconds(6.5));
            }

            if (top)
            {
                await this.Send(Command.Up);
                await Task.Delay(TimeSpan.FromSeconds(1.5));
            }
            else if (bottom)
            {
                await this.Send(Command.Down);
                await Task.Delay(TimeSpan.FromSeconds(1.5));
            }

            await this.Send(Command.Stop);
        }

        public async Task Fire()
        {
            await this.Send(Command.Fire);
            await Task.Delay(TimeSpan.FromSeconds(4));
        }
    }
}
