using System;
using System.Threading.Tasks;

namespace DeskToys
{
    public static class LauncherExtensions
    {
        public static Task Down(this ILauncher launcher, double millisecondsTime)
        {
            return launcher.Down(TimeSpan.FromMilliseconds(millisecondsTime));
        }

        public static async Task Down(this ILauncher launcher, TimeSpan time)
        {
            await launcher.Send(Command.Down);
            await Task.Delay(time);
            await launcher.Send(Command.Stop);
        }

        public static Task Up(this ILauncher launcher, double millisecondsTime)
        {
            return launcher.Up(TimeSpan.FromMilliseconds(millisecondsTime));
        }

        public static async Task Up(this ILauncher launcher, TimeSpan time)
        {
            await launcher.Send(Command.Up);
            await Task.Delay(time);
            await launcher.Send(Command.Stop);
        }

        public static Task Left(this ILauncher launcher, double millisecondsTime)
        {
            return launcher.Left(TimeSpan.FromMilliseconds(millisecondsTime));
        }

        public static async Task Left(this ILauncher launcher, TimeSpan time)
        {
            await launcher.Send(Command.Left);
            await Task.Delay(time);
            await launcher.Send(Command.Stop);
        }

        public static Task Right(this ILauncher launcher, double millisecondsTime)
        {
            return launcher.Right(TimeSpan.FromMilliseconds(millisecondsTime));
        }

        public static async Task Right(this ILauncher launcher, TimeSpan time)
        {
            await launcher.Send(Command.Right);
            await Task.Delay(time);
            await launcher.Send(Command.Stop);
        }

        public static async Task FireAll(this ILauncher launcher)
        {
            for (int i = 0; i < launcher.MissileCount; i++)
            {
                await launcher.Fire();
            }
        }
    }
}
