// SPDX-License-Identifier: MIT
/*
* KFDtool Container (EKC) Editor
* MIT Open Source. Use is subject to license terms.
* DO NOT ALTER OR REMOVE COPYRIGHT NOTICES OR THIS FILE HEADER.
*
*/

using System;
using System.Collections.Generic;

using KFDtool.P25.DataLinkIndependent;
using KFDtool.P25.ManualRekey;
using KFDtool.P25.NetworkProtocol;

namespace KFDtool.P25.TransferConstructs
{
    /// <summary>
    /// 
    /// </summary>
    public class InteractDliIp
    {
        /*
        ** Methods
        */

        /// <summary>
        /// 
        /// </summary>
        /// <param name="device"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        private static DataLinkIndependentProtocol GetDli(BaseDevice device)
        {
            if (device.DliIpDevice.Protocol == DliIpDevice.ProtocolOptions.UDP)
            {
                int timeout = 5000;

                UdpProtocol udpProtocol = new UdpProtocol(device.DliIpDevice.Hostname, device.DliIpDevice.Port, timeout);

                bool motVariant;

                if (device.DliIpDevice.Variant == DliIpDevice.VariantOptions.Standard)
                    motVariant = false;
                else if (device.DliIpDevice.Variant == DliIpDevice.VariantOptions.Motorola)
                    motVariant = true;
                else
                    throw new ArgumentOutOfRangeException("Variant");

                return new DataLinkIndependentProtocol(udpProtocol, motVariant);
            }
            else
                throw new ArgumentOutOfRangeException("Protocol");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="device"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        private static ManualRekeyApplication GetMra(BaseDevice device)
        {
            if (device.DliIpDevice.Protocol == DliIpDevice.ProtocolOptions.UDP)
            {
                int timeout = 5000;

                UdpProtocol udpProtocol = new UdpProtocol(device.DliIpDevice.Hostname, device.DliIpDevice.Port, timeout);

                bool motVariant;

                if (device.DliIpDevice.Variant == DliIpDevice.VariantOptions.Standard)
                    motVariant = false;
                else if (device.DliIpDevice.Variant == DliIpDevice.VariantOptions.Motorola)
                    motVariant = true;
                else
                    throw new ArgumentOutOfRangeException("Variant");

                return new ManualRekeyApplication(udpProtocol, motVariant);
            }
            else
                throw new ArgumentOutOfRangeException("Protocol");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="device"></param>
        public static void CheckTargetMrConnection(BaseDevice device)
        {
            try
            {
                DataLinkIndependentProtocol dli = GetDli(device);
                dli.CheckTargetMrConnection();
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="device"></param>
        /// <param name="keys"></param>
        public static void Keyload(BaseDevice device, List<CmdKeyItem> keys)
        {
            try
            {
                ManualRekeyApplication mra = GetMra(device);
                mra.Keyload(keys);
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="device"></param>
        /// <param name="keys"></param>
        public static void EraseKey(BaseDevice device, List<CmdKeyItem> keys)
        {
            try
            {
                ManualRekeyApplication mra = GetMra(device);
                mra.EraseKeys(keys);
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="device"></param>
        public static void EraseAllKeys(BaseDevice device)
        {
            try
            {
                ManualRekeyApplication mra = GetMra(device);
                mra.EraseAllKeys();
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="device"></param>
        /// <returns></returns>
        public static List<RspKeyInfo> ViewKeyInfo(BaseDevice device)
        {
            try
            {
                ManualRekeyApplication mra = GetMra(device);
                return mra.ViewKeyInfo();
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="device"></param>
        /// <param name="kmfRsi"></param>
        /// <param name="mnp"></param>
        /// <returns></returns>
        public static RspRsiInfo LoadConfig(BaseDevice device, int kmfRsi, int mnp)
        {
            try
            {
                ManualRekeyApplication mra = GetMra(device);
                return mra.LoadConfig(kmfRsi, mnp);
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="device"></param>
        /// <param name="rsiOld"></param>
        /// <param name="rsiNew"></param>
        /// <param name="mnp"></param>
        /// <returns></returns>
        public static RspRsiInfo ChangeRsi(BaseDevice device, int rsiOld, int rsiNew, int mnp)
        {
            try
            {
                ManualRekeyApplication mra = GetMra(device);
                return mra.ChangeRsi(rsiOld, rsiNew, mnp);
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="device"></param>
        /// <returns></returns>
        public static List<RspRsiInfo> ViewRsiItems(BaseDevice device)
        {
            try
            {
                ManualRekeyApplication mra = GetMra(device);
                return mra.ViewRsiItems();
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="device"></param>
        /// <returns></returns>
        public static int ViewMnp(BaseDevice device)
        {
            try
            {
                ManualRekeyApplication mra = GetMra(device);
                return mra.ViewMnp();
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="device"></param>
        /// <returns></returns>
        public static int ViewKmfRsi(BaseDevice device)
        {
            try
            {
                ManualRekeyApplication mra = GetMra(device);
                return mra.ViewKmfRsi();
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="device"></param>
        /// <returns></returns>
        public static List<RspKeysetInfo> ViewKeysetTaggingInfo(BaseDevice device)
        {
            try
            {
                ManualRekeyApplication mra = GetMra(device);

                return mra.ViewKeysetTaggingInfo();
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="device"></param>
        /// <param name="keysetSuperseded"></param>
        /// <param name="keysetActivated"></param>
        /// <returns></returns>
        public static RspChangeoverInfo ActivateKeyset(BaseDevice device, int keysetSuperseded, int keysetActivated)
        {
            try
            {
                ManualRekeyApplication mra = GetMra(device);
                return mra.ActivateKeyset(keysetSuperseded, keysetActivated);
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
