namespace DreamCheeky.MissileLauncher
{
    public static class Launchers
    {
        public static ILauncher Create()
        {
            return Create<ILauncher>();
        }

        public static T Create<T>() where T : ILauncher
        {
            return (T)(object)new LauncherDevice();
        }
    }
}
