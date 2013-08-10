using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;
using HidLibrary;

namespace DeskToys.Implementations
{
    public class StressButton : IButton
    {
        private HotKeyForm form;

        private StressButton()
        {
            this.form = HotKeyForm.Run(Keys.Shift | Keys.Alt | Keys.P);
            this.form.Press += (o, e) =>
            {
                var handler = this.Press;
                if (handler != null)
                {
                    handler(this, e);
                }
            };
        }

        public event EventHandler<EventArgs> Press;

        public static IEnumerable<Service> Enumerate()
        {
            if (HidDevices.Enumerate(0x04F3, 0x04A0).Any())
            {
                yield return new Service<StressButton>("Dream Cheeky Stress Button", () => new StressButton());
            }
        }

        public void Dispose()
        {
            this.form.Dispose();
        }

        private class HotKeyForm : Form
        {
            private const int WM_HOTKEY = 0x312;
            private static int autoId;

            public HotKeyForm(Keys keys)
            {
                var modifiers = keys & Keys.Modifiers;

                var key = (uint)(keys & ~Keys.Modifiers);
                var modifier =
                    (modifiers.HasFlag(Keys.Alt) ? 0x01u : 0) +
                    (modifiers.HasFlag(Keys.Control) ? 0x02u : 0) +
                    (modifiers.HasFlag(Keys.Shift) ? 0x04u : 0);

                if (!NativeMethods.RegisterHotKey(this.Handle, AutoId, modifier, key))
                {
                    throw new Win32Exception(Marshal.GetLastWin32Error());
                }
            }

            public event EventHandler<EventArgs> Press;

            private static int AutoId
            {
                get { return Interlocked.Increment(ref autoId); }
            }

            public static HotKeyForm Run(Keys keys)
            {
                HotKeyForm form = null;
                var initialized = new ManualResetEvent(false);

                var thread = new Thread(() =>
                {
                    form = new HotKeyForm(keys);
                    initialized.Set();
                    Application.Run(form);
                });
                thread.IsBackground = true;
                thread.Start();

                initialized.WaitOne();
                return form;
            }

            protected override void SetVisibleCore(bool value)
            {
                base.SetVisibleCore(false);
            }

            protected override void WndProc(ref Message m)
            {
                if (m.Msg == WM_HOTKEY)
                {
                    var handler = this.Press;
                    if (handler != null)
                    {
                        handler(this, new EventArgs());
                    }
                }

                base.WndProc(ref m);
            }

            protected override void Dispose(bool disposing)
            {
                if (disposing)
                {
                    this.Close();
                }

                base.Dispose(disposing);
            }
        }
    }
}
