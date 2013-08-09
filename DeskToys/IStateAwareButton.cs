using System;

namespace DeskToys
{
    public interface IStateAwareButton : IButton
    {
        event EventHandler<EventArgs> Down;

        event EventHandler<EventArgs> Up;

        bool State { get; }
    }
}
