using System;

namespace DreamCheeky.MissileLauncher
{
    [Flags]
    public enum Edge
    {
        None = 0,
        Bottom = 1,
        Top = 2,
        Left = 4,
        Right = 8,
        Fire = 16,
    }
}
