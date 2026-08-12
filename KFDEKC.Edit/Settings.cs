// SPDX-License-Identifier: MIT
/*
* KFDtool Container (EKC) Editor
* MIT Open Source. Use is subject to license terms.
* DO NOT ALTER OR REMOVE COPYRIGHT NOTICES OR THIS FILE HEADER.
*
*/

using System;
using System.Diagnostics;
using System.Reflection;

using KFDEKC.Container.FileStructure.EKC;

using KFDtool.Adapter.Device;
using KFDtool.P25.TransferConstructs;

namespace KFDEKC.Edit
{
    /// <summary>
    /// 
    /// </summary>
    public class Settings
    {
        public const string ASSEMBLY_VERSION = "R01A00";

        /// <summary>
        /// 
        /// </summary>
        public enum ThemeMode
        {
            Dark = 1,
            Light = 2
        }

        /// <summary>
        /// 
        /// </summary>
        public static bool ContainerOpen { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public static bool ContainerSaved { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public static string ContainerPath { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public static byte[] ContainerKey { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public static OuterContainer ContainerOuter { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public static InnerContainer ContainerInner { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public static ThemeMode SelectedTheme { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public static BaseDevice SelectedDevice { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public static string LastDVMFNEHostname { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public static int LastDVMFNEPort { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public static uint LastDVMFNEPeerId { get; set; }

        /*
        ** Methods
        */

        /// <summary>
        /// 
        /// </summary>
        static Settings()
        {
            ContainerOpen = false;
            ContainerSaved = false;
            ContainerPath = string.Empty;
            ContainerKey = null;
            ContainerInner = null;
            ContainerOuter = null;

            SelectedDevice = new BaseDevice();

            SelectedDevice.TwiKfdtoolDevice = new TwiKfdtoolDevice();
            SelectedDevice.DliIpDevice = new DliIpDevice();
            SelectedDevice.DliIpDevice.Protocol = DliIpDevice.ProtocolOptions.UDP;

            LastDVMFNEHostname = string.Empty;
            LastDVMFNEPort = 0;
            LastDVMFNEPeerId = 0;

            SelectedTheme = ThemeMode.Dark;

            //LoadSettings();
        }

        /// <summary>
        /// 
        /// </summary>
        public static void InitSettings()
        {
            Properties.Settings.Default.TwiComPort = "";
            Properties.Settings.Default.DliHostname = "192.168.128.1";
            Properties.Settings.Default.DliPort = 49644;
            Properties.Settings.Default.DliVariant = "Motorola";
            Properties.Settings.Default.DeviceType = "TwiKfdDevice";
            Properties.Settings.Default.KfdDeviceType = "KfdShield";

            Properties.Settings.Default.LastDVMHostname = string.Empty;
            Properties.Settings.Default.LastDVMPort = 0;
            Properties.Settings.Default.LastDVMPeerID = 0;

            Properties.Settings.Default.SelectedTheme = ThemeMode.Dark.ToString();
            Properties.Settings.Default.Save();
        }

        /// <summary>
        /// 
        /// </summary>
        public static void SaveSettings()
        {
            Properties.Settings.Default.TwiComPort = SelectedDevice.TwiKfdtoolDevice.ComPort;
            Properties.Settings.Default.DliHostname = SelectedDevice.DliIpDevice.Hostname;
            Properties.Settings.Default.DliPort = SelectedDevice.DliIpDevice.Port;
            Properties.Settings.Default.DliVariant = SelectedDevice.DliIpDevice.Variant.ToString();
            Properties.Settings.Default.DeviceType = SelectedDevice.DeviceType.ToString();
            Properties.Settings.Default.KfdDeviceType = SelectedDevice.KfdDeviceType.ToString();

            Properties.Settings.Default.LastDVMHostname = LastDVMFNEHostname;
            Properties.Settings.Default.LastDVMPort = LastDVMFNEPort;
            Properties.Settings.Default.LastDVMPeerID = LastDVMFNEPeerId;

            Properties.Settings.Default.SelectedTheme = SelectedTheme.ToString();
            Properties.Settings.Default.Save();
        }

        /// <summary>
        /// 
        /// </summary>
        public static void LoadSettings()
        {
            SelectedDevice.TwiKfdtoolDevice.ComPort = Properties.Settings.Default.TwiComPort;
            SelectedDevice.DliIpDevice.Hostname = Properties.Settings.Default.DliHostname;
            SelectedDevice.DliIpDevice.Port = Properties.Settings.Default.DliPort;
            SelectedDevice.DliIpDevice.Variant = (DliIpDevice.VariantOptions)Enum.Parse(typeof(DliIpDevice.VariantOptions), Properties.Settings.Default.DliVariant);
            SelectedDevice.DeviceType = (BaseDevice.DeviceTypeOptions)Enum.Parse(typeof(BaseDevice.DeviceTypeOptions), Properties.Settings.Default.DeviceType);
            SelectedDevice.KfdDeviceType = (TwiKfdDevice)Enum.Parse(typeof(TwiKfdDevice), Properties.Settings.Default.KfdDeviceType);

            LastDVMFNEHostname = Properties.Settings.Default.LastDVMHostname;
            LastDVMFNEPort = Properties.Settings.Default.LastDVMPort;
            LastDVMFNEPeerId = Properties.Settings.Default.LastDVMPeerID;

            SelectedTheme = LoadSelectedTheme();
        }

        /// <summary>
        /// 
        /// </summary>
        private static ThemeMode LoadSelectedTheme()
        {
            if (Enum.TryParse(Properties.Settings.Default.SelectedTheme, true, out ThemeMode selectedTheme) &&
                Enum.IsDefined(typeof(ThemeMode), selectedTheme))
            {
                return selectedTheme;
            }

            Properties.Settings.Default.SelectedTheme = ThemeMode.Dark.ToString();
            Properties.Settings.Default.Save();
            return ThemeMode.Dark;
        }
    }
}
