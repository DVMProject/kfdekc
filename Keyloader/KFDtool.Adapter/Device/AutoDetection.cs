// SPDX-License-Identifier: MIT
/*
* KFDtool Container (EKC) Editor
* MIT Open Source. Use is subject to license terms.
* DO NOT ALTER OR REMOVE COPYRIGHT NOTICES OR THIS FILE HEADER.
*
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;

namespace KFDtool.Adapter.Device
{
    /// <summary>
    /// 
    /// </summary>
    public class AutoDetection
    {
        private ManagementEventWatcher Watcher;
        private bool FirstRun;

        public List<string> Devices { get; private set; }
        public event EventHandler DevicesChanged;

        /*
        ** Methods
        */

        /// <summary>
        /// 
        /// </summary>
        public AutoDetection()
        {
            FirstRun = false;

            Devices = new List<string>();

            Watcher = new ManagementEventWatcher();
            WqlEventQuery query = new WqlEventQuery("SELECT * FROM Win32_DeviceChangeEvent");
            Watcher.EventArrived += new EventArrivedEventHandler(USBChangedEvent);
            Watcher.Query = query;
        }

        /// <summary>
        /// 
        /// </summary>
        public void Start()
        {
            FirstRun = true;

            UpdateDevices();
            Watcher.Start();
        }

        /// <summary>
        /// 
        /// </summary>
        public void Stop()
        {
            Watcher.Stop();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void USBChangedEvent(object sender, EventArrivedEventArgs e)
        {
            UpdateDevices();
        }

        /// <summary>
        /// 
        /// </summary>
        private void UpdateDevices()
        {
            List<string> detDev = ManualDetection.DetectConnectedAppDevices();

            detDev.Sort();

            if (FirstRun || !detDev.SequenceEqual(Devices))
            {
                FirstRun = false;

                Devices.Clear();
                Devices.AddRange(detDev);
                Devices.Sort();
                OnDevicesChanged(new EventArgs());
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        private void OnDevicesChanged(EventArgs e)
        {
            EventHandler handler = DevicesChanged;
            if (handler != null)
            {
                handler(this, e);
            }
        }
    }
}
