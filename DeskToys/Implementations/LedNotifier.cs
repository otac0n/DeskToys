﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using HidLibrary;

namespace DeskToys.Implementations
{
    public class LedNotifier : ILedNotifier
    {
        private static readonly byte[] colorData = { 0, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x1F, 0x05 };

        private static readonly byte[][] initData =
        {
            new byte[] { 0, 0x1F, 0x02, 0x00, 0x5F, 0x00, 0x00, 0x1F, 0x03 },
            new byte[] { 0, 0x00, 0x02, 0x00, 0x5F, 0x00, 0x00, 0x1F, 0x04 },
            new byte[] { 0, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x01 },
        };

        private readonly HidDevice device;
        private bool initialized;
        private float redRange = 1.0f;
        private float greenRange = 1.0f;
        private float blueRange = 1.0f;

        private LedNotifier(HidDevice device)
        {
            this.device = device;
        }

        public static IEnumerable<Service> Enumerate()
        {
            foreach (var device in HidDevices.Enumerate(0x1D34, 0x000A))
            {
                yield return new Service<LedNotifier>("Dream Cheeky Mailbox Friends Alert", () => new LedNotifier(device));
            }

            foreach (var device in HidDevices.Enumerate(0x1D34, 0x0004))
            {
                yield return new Service<LedNotifier>("Dream Cheeky WebMail Notifier", () => new LedNotifier(device));
            }
        }

        public float RedIntensity
        {
            get
            {
                return this.redRange;
            }

            set
            {
                if (value > 1.0 || value < 0.0)
                {
                    throw new ArgumentOutOfRangeException("value");
                }

                this.redRange = value;
            }
        }

        public float GreenIntensity
        {
            get
            {
                return this.greenRange;
            }

            set
            {
                if (value > 1.0 || value < 0.0)
                {
                    throw new ArgumentOutOfRangeException("value");
                }

                this.greenRange = value;
            }
        }

        public float BlueIntensity
        {
            get
            {
                return this.blueRange;
            }

            set
            {
                if (value > 1.0 || value < 0.0)
                {
                    throw new ArgumentOutOfRangeException("value");
                }

                this.blueRange = value;
            }
        }

        public static int GetDeviceCount(int vendorId, int productId)
        {
            return HidDevices.Enumerate(vendorId, productId).Count();
        }

        public async Task SetColor(Color color)
        {
            var data = new byte[colorData.Length];
            Array.Copy(colorData, data, colorData.Length);
            data[1] = (byte)Math.Floor(60 * this.redRange * color.R / 255);
            data[2] = (byte)Math.Floor(60 * this.greenRange * color.G / 255);
            data[3] = (byte)Math.Floor(60 * this.blueRange * color.B / 255);

            if (await this.Initialize())
            {
                await this.device.WriteAsync(data);
            }
        }

        private async Task<bool> Initialize()
        {
            if (this.initialized)
            {
                return true;
            }

            var success = true;
            for (int i = 0; i < initData.Length; i++)
            {
                success = success && await this.device.WriteAsync(initData[i]);
            }

            return this.initialized = success;
        }

        public void Dispose()
        {
            this.device.Dispose();
        }
    }
}
