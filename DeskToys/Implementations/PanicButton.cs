﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using HidLibrary;

namespace DeskToys
{
    public class PanicButton : IButton
    {
        private readonly HidDevice device;
        private readonly Timer timer;

        private PanicButton(HidDevice device)
        {
            this.device = device;
            this.Tick(null);
            this.timer = new Timer(Tick, null, TimeSpan.Zero, TimeSpan.FromMilliseconds(10));
        }

        internal IEnumerable<Service> Enumerate()
        {
            foreach (var device in HidDevices.Enumerate(0x1130, 0x202).Where(d => d.Capabilities.NumberFeatureButtonCaps > 0))
            {
                yield return new Service<PanicButton>(() => new PanicButton(device));
            }
        }

        public event EventHandler<EventArgs> Click;

        public void Dispose()
        {
            this.timer.Dispose();
        }

        private void Tick(object state)
        {
            byte[] data;
            if (this.device.ReadFeatureData(out data, 0))
            {
                for (var i = 0; i < data[1]; i++)
                {
                    this.RaiseClick();
                }
            }
        }

        private void RaiseClick()
        {
            var handler = this.Click;
            if (handler != null)
            {
                handler(this, new EventArgs());
            }
        }
    }
}