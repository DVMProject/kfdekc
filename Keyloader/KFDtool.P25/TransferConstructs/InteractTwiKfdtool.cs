// SPDX-License-Identifier: MIT
/*
* KFDtool Container (EKC) Editor
* MIT Open Source. Use is subject to license terms.
* DO NOT ALTER OR REMOVE COPYRIGHT NOTICES OR THIS FILE HEADER.
*
*/

using System;
using System.Collections.Generic;
using System.Text;

using KFDtool.Adapter.Protocol.Adapter;
using KFDtool.P25.ManualRekey;
using KFDtool.P25.ThreeWire;

namespace KFDtool.P25.TransferConstructs
{
    /// <summary>
    /// 
    /// </summary>
    public class InteractTwiKfdtool
    {
        /*
        ** Methods
        */

        /// <summary>
        /// 
        /// </summary>
        /// <param name="device"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public static string ReadAdapterProtocolVersion(BaseDevice device)
        {
            string version = string.Empty;
            if (device.TwiKfdtoolDevice.ComPort == string.Empty)
                throw new ArgumentException("No device selected");

            AdapterProtocol ap = null;

            try
            {
                ap = new AdapterProtocol(device.TwiKfdtoolDevice.ComPort, device.KfdDeviceType);
                ap.Open();
                ap.Clear();

                Version ver = ap.ReadAdapterProtocolVersion();
                version = string.Format("{0}.{1}.{2}", ver.Major, ver.Minor, ver.Build);
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                try
                {
                    if (ap != null)
                        ap.Close();
                }
                catch (System.IO.IOException ex)
                {
                    /* stub */
                }
            }

            return version;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="device"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public static string ReadFirmwareVersion(BaseDevice device)
        {
            string version = string.Empty;
            if (device.TwiKfdtoolDevice.ComPort == string.Empty)
                throw new ArgumentException("No device selected");

            AdapterProtocol ap = null;

            try
            {
                ap = new AdapterProtocol(device.TwiKfdtoolDevice.ComPort, device.KfdDeviceType);
                ap.Open();
                ap.Clear();

                byte[] ver = ap.ReadFirmwareVersion();
                version = string.Format("{0}.{1}.{2}", ver[0], ver[1], ver[2]);
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                try
                {
                    if (ap != null)
                        ap.Close();
                }
                catch (System.IO.IOException ex)
                {
                    /* stub */
                }
            }

            return version;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="device"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public static string ReadUniqueId(BaseDevice device)
        {
            string uniqueId = string.Empty;
            if (device.TwiKfdtoolDevice.ComPort == string.Empty)
                throw new ArgumentException("No device selected");

            AdapterProtocol ap = null;

            try
            {
                ap = new AdapterProtocol(device.TwiKfdtoolDevice.ComPort, device.KfdDeviceType);
                ap.Open();
                ap.Clear();

                byte[] id = ap.ReadUniqueId();
                if (id.Length == 0)
                    uniqueId = "NONE";
                else
                    uniqueId = BitConverter.ToString(id).Replace("-", string.Empty);
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                try
                {
                    if (ap != null)
                        ap.Close();
                }
                catch (System.IO.IOException ex)
                {
                    /* stub */
                }
            }

            return uniqueId;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="device"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public static string ReadModel(BaseDevice device)
        {
            string model = string.Empty;
            if (device.TwiKfdtoolDevice.ComPort == string.Empty)
                throw new ArgumentException("No device selected");

            AdapterProtocol ap = null;

            try
            {
                ap = new AdapterProtocol(device.TwiKfdtoolDevice.ComPort, device.KfdDeviceType);
                ap.Open();
                ap.Clear();

                byte mod = ap.ReadModelId();
                switch (mod) {
                    case 0x00:
                        model = "NOT SET";
                        break;
                    case 0x01:
                        model = "KFD100";
                        break;
                    case 0x02:
                        model = "KFDshield";
                        break;
                    case 0x03:
                        model = "KFDmicro"; // @w3axl
                        break;
                    case 0x04:
                        model = "KFDpico"; // @alexhanyuan
                        break;
                    case 0x05:
                        model = "bblkey"; // @beepbooplabsltd
                        break;
                    case 0x06:
                        model = "KFDnano"; // @alexhanyuan and @rentfrow72
                        break;
                    case 0x07:
                        model = "RESERVED"; // @beepbooplabsltd
                        break;
                    default:
                        model = "UNKNOWN";
                        break;
                }
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                try
                {
                    if (ap != null)
                        ap.Close();
                }
                catch (System.IO.IOException ex)
                {
                    /* stub */
                }
            }

            return model;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="device"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public static string ReadHardwareRevision(BaseDevice device)
        {
            string version = string.Empty;
            if (device.TwiKfdtoolDevice.ComPort == string.Empty)
                throw new ArgumentException("No device selected");

            AdapterProtocol ap = null;

            try
            {
                ap = new AdapterProtocol(device.TwiKfdtoolDevice.ComPort, device.KfdDeviceType);
                ap.Open();
                ap.Clear();

                byte[] ver = ap.ReadHardwareRevision();
                if (ver[0] == 0x00 && ver[1] == 0x00)
                    version = "NOT SET";
                else
                    version = string.Format("{0}.{1}", ver[0], ver[1]);
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                try
                {
                    if (ap != null)
                        ap.Close();
                }
                catch (System.IO.IOException ex)
                {
                    /* stub */
                }
            }

            return version;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="device"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public static string ReadSerialNumber(BaseDevice device)
        {
            string serialNumber = string.Empty;
            if (device.TwiKfdtoolDevice.ComPort == string.Empty)
                throw new ArgumentException("No device selected");

            AdapterProtocol ap = null;

            try
            {
                ap = new AdapterProtocol(device.TwiKfdtoolDevice.ComPort, device.KfdDeviceType);
                ap.Open();
                ap.Clear();

                byte[] ser = ap.ReadSerialNumber();
                if (ser.Length == 0)
                    serialNumber = "NONE";
                else
                    serialNumber = Encoding.ASCII.GetString(ser);
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                try
                {
                    if (ap != null)
                        ap.Close();
                }
                catch (System.IO.IOException ex)
                {
                    /* stub */
                }
            }

            return serialNumber;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="device"></param>
        /// <exception cref="ArgumentException"></exception>
        public static void EnterBslMode(BaseDevice device)
        {
            if (device.TwiKfdtoolDevice.ComPort == string.Empty)
                throw new ArgumentException("No device selected");

            AdapterProtocol ap = null;

            try
            {
                ap = new AdapterProtocol(device.TwiKfdtoolDevice.ComPort, device.KfdDeviceType);
                ap.Open();
                ap.Clear();

                ap.EnterBslMode();
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                try
                {
                    if (ap != null)
                        ap.Close();
                }
                catch (System.IO.IOException ex)
                {
                    /* stub */
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="device"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public static string SelfTest(BaseDevice device)
        {
            string result = string.Empty;
            if (device.TwiKfdtoolDevice.ComPort == string.Empty)
                throw new ArgumentException("No device selected");

            AdapterProtocol ap = null;

            try
            {
                ap = new AdapterProtocol(device.TwiKfdtoolDevice.ComPort, device.KfdDeviceType);
                ap.Open();
                ap.Clear();

                byte res = ap.SelfTest();

                if (res == 0x01)
                    result = string.Format("Data shorted to ground (0x{0:X2})", res);
                else if (res == 0x02)
                    result = string.Format("Sense shorted to ground (0x{0:X2})", res);
                else if (res == 0x03)
                    result = string.Format("Data shorted to power (0x{0:X2})", res);
                else if (res == 0x04)
                    result = string.Format("Sense shorted to power (0x{0:X2})", res);
                else if (res == 0x05)
                    result = string.Format("Data and Sense shorted (0x{0:X2})", res);
                else if (res == 0x06)
                    result = string.Format("Sense and Data shorted (0x{0:X2})", res);
                else if (res != 0x00)
                    result = string.Format("Unknown self test result (0x{0:X2})", res);
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                try
                {
                    if (ap != null)
                        ap.Close();
                }
                catch (System.IO.IOException ex)
                {
                    /* stub */
                }
            }

            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="device"></param>
        /// <exception cref="ArgumentException"></exception>
        public static void CheckTargetMrConnection(BaseDevice device)
        {
            if (device.TwiKfdtoolDevice.ComPort == string.Empty)
                throw new ArgumentException("No device selected");

            AdapterProtocol ap = null;

            try
            {
                ap = new AdapterProtocol(device.TwiKfdtoolDevice.ComPort, device.KfdDeviceType);
                ap.Open();
                ap.Clear();

                ThreeWireProtocol twp = new ThreeWireProtocol(ap);
                twp.CheckTargetMrConnection();
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                try
                {
                    if (ap != null)
                        ap.Close();
                }
                catch (System.IO.IOException ex)
                {
                    /* stub */
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="device"></param>
        /// <param name="keys"></param>
        /// <param name="kek"></param>
        /// <exception cref="ArgumentException"></exception>
        public static void Keyload(BaseDevice device, List<CmdKeyItem> keys, CmdKeyItem kek = null)
        {
            if (device.TwiKfdtoolDevice.ComPort == string.Empty)
                throw new ArgumentException("No device selected");

            AdapterProtocol ap = null;

            try
            {
                ap = new AdapterProtocol(device.TwiKfdtoolDevice.ComPort, device.KfdDeviceType);
                ap.Open();
                ap.Clear();

                ManualRekeyApplication mra = new ManualRekeyApplication(ap);
                mra.Keyload(keys, kek);
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                try
                {
                    if (ap != null)
                        ap.Close();
                }
                catch (System.IO.IOException ex)
                {
                    /* stub */
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="device"></param>
        /// <param name="keys"></param>
        /// <exception cref="ArgumentException"></exception>
        public static void EraseKey(BaseDevice device, List<CmdKeyItem> keys)
        {
            if (device.TwiKfdtoolDevice.ComPort == string.Empty)
                throw new ArgumentException("No device selected");

            AdapterProtocol ap = null;

            try
            {
                ap = new AdapterProtocol(device.TwiKfdtoolDevice.ComPort, device.KfdDeviceType);
                ap.Open();
                ap.Clear();

                ManualRekeyApplication mra = new ManualRekeyApplication(ap);
                mra.EraseKeys(keys);
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                try
                {
                    if (ap != null)
                        ap.Close();
                }
                catch (System.IO.IOException ex)
                {
                    /* stub */
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="device"></param>
        /// <exception cref="ArgumentException"></exception>
        public static void EraseAllKeys(BaseDevice device)
        {
            if (device.TwiKfdtoolDevice.ComPort == string.Empty)
                throw new ArgumentException("No device selected");

            AdapterProtocol ap = null;

            try
            {
                ap = new AdapterProtocol(device.TwiKfdtoolDevice.ComPort, device.KfdDeviceType);
                ap.Open();
                ap.Clear();

                ManualRekeyApplication mra = new ManualRekeyApplication(ap);
                mra.EraseAllKeys();
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                try
                {
                    if (ap != null)
                        ap.Close();
                }
                catch (System.IO.IOException ex)
                {
                    /* stub */
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="device"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public static List<RspKeyInfo> ViewKeyInfo(BaseDevice device)
        {
            List<RspKeyInfo> result = new List<RspKeyInfo>();

            if (device.TwiKfdtoolDevice.ComPort == string.Empty)
                throw new ArgumentException("No device selected");

            AdapterProtocol ap = null;

            try
            {
                ap = new AdapterProtocol(device.TwiKfdtoolDevice.ComPort, device.KfdDeviceType);
                ap.Open();
                ap.Clear();

                ManualRekeyApplication mra = new ManualRekeyApplication(ap);
                result.AddRange(mra.ViewKeyInfo());
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                try
                {
                    if (ap != null)
                        ap.Close();
                }
                catch (System.IO.IOException ex)
                {
                    /* stub */
                }
            }

            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="device"></param>
        /// <param name="kmfRsi"></param>
        /// <param name="mnp"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public static RspRsiInfo LoadConfig(BaseDevice device, int kmfRsi, int mnp)
        {
            RspRsiInfo result = new RspRsiInfo();

            if (device.TwiKfdtoolDevice.ComPort == string.Empty)
                throw new ArgumentException("No device selected");

            AdapterProtocol ap = null;

            try
            {
                ap = new AdapterProtocol(device.TwiKfdtoolDevice.ComPort, device.KfdDeviceType);
                ap.Open();
                ap.Clear();

                ManualRekeyApplication mra = new ManualRekeyApplication(ap);
                result = mra.LoadConfig(kmfRsi, mnp);
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                try
                {
                    if (ap != null)
                        ap.Close();
                }
                catch (System.IO.IOException ex)
                {
                    /* stub */
                }
            }

            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="device"></param>
        /// <param name="rsiOld"></param>
        /// <param name="rsiNew"></param>
        /// <param name="mnp"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public static RspRsiInfo ChangeRsi(BaseDevice device, int rsiOld, int rsiNew, int mnp)
        {
            RspRsiInfo result = new RspRsiInfo();

            if (device.TwiKfdtoolDevice.ComPort == string.Empty)
                throw new ArgumentException("No device selected");

            AdapterProtocol ap = null;

            try
            {
                ap = new AdapterProtocol(device.TwiKfdtoolDevice.ComPort, device.KfdDeviceType);
                ap.Open();
                ap.Clear();

                ManualRekeyApplication mra = new ManualRekeyApplication(ap);
                result = mra.ChangeRsi(rsiOld, rsiNew, mnp);
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                try
                {
                    if (ap != null)
                        ap.Close();
                }
                catch (System.IO.IOException ex)
                {
                    /* stub */
                }
            }

            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="device"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public static List<RspRsiInfo> ViewRsiItems(BaseDevice device)
        {
            List<RspRsiInfo> result = new List<RspRsiInfo>();

            if (device.TwiKfdtoolDevice.ComPort == string.Empty)
                throw new ArgumentException("No device selected");

            AdapterProtocol ap = null;

            try
            {
                ap = new AdapterProtocol(device.TwiKfdtoolDevice.ComPort, device.KfdDeviceType);
                ap.Open();
                ap.Clear();

                ManualRekeyApplication mra = new ManualRekeyApplication(ap);
                result.AddRange(mra.ViewRsiItems());
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                try
                {
                    if (ap != null)
                        ap.Close();
                }
                catch (System.IO.IOException ex)
                {
                    /* stub */
                }
            }

            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="device"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public static int ViewMnp(BaseDevice device)
        {
            int result = new int();
            if (device.TwiKfdtoolDevice.ComPort == string.Empty)
                throw new ArgumentException("No device selected");

            AdapterProtocol ap = null;

            try
            {
                ap = new AdapterProtocol(device.TwiKfdtoolDevice.ComPort, device.KfdDeviceType);
                ap.Open();
                ap.Clear();

                ManualRekeyApplication mra = new ManualRekeyApplication(ap);
                result = mra.ViewMnp();
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                try
                {
                    if (ap != null)
                        ap.Close();
                }
                catch (System.IO.IOException ex)
                {
                    /* stub */
                }
            }

            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="device"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public static int ViewKmfRsi(BaseDevice device)
        {
            int result = new int();
            if (device.TwiKfdtoolDevice.ComPort == string.Empty)
                throw new ArgumentException("No device selected");

            AdapterProtocol ap = null;

            try
            {
                ap = new AdapterProtocol(device.TwiKfdtoolDevice.ComPort, device.KfdDeviceType);
                ap.Open();
                ap.Clear();

                ManualRekeyApplication mra = new ManualRekeyApplication(ap);
                result = mra.ViewKmfRsi();
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                try
                {
                    if (ap != null)
                        ap.Close();
                }
                catch (System.IO.IOException ex)
                {
                    /* stub */
                }
            }

            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="device"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public static List<RspKeysetInfo> ViewKeysetTaggingInfo(BaseDevice device)
        {
            List<RspKeysetInfo> result = new List<RspKeysetInfo>();
            if (device.TwiKfdtoolDevice.ComPort == string.Empty)
                throw new ArgumentException("No device selected");

            AdapterProtocol ap = null;

            try
            {
                ap = new AdapterProtocol(device.TwiKfdtoolDevice.ComPort, device.KfdDeviceType);
                ap.Open();
                ap.Clear();

                ManualRekeyApplication mra = new ManualRekeyApplication(ap);
                result = mra.ViewKeysetTaggingInfo();
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                try
                {
                    if (ap != null)
                        ap.Close();
                }
                catch (System.IO.IOException ex)
                {
                    /* stub */
                }
            }

            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="device"></param>
        /// <param name="keysetSuperseded"></param>
        /// <param name="keysetActivated"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public static RspChangeoverInfo ActivateKeyset(BaseDevice device, int keysetSuperseded, int keysetActivated)
        {
            RspChangeoverInfo result = new RspChangeoverInfo();
            if (device.TwiKfdtoolDevice.ComPort == string.Empty)
                throw new ArgumentException("No device selected");

            AdapterProtocol ap = null;

            try
            {
                ap = new AdapterProtocol(device.TwiKfdtoolDevice.ComPort, device.KfdDeviceType);
                ap.Open();
                ap.Clear();

                ManualRekeyApplication mra = new ManualRekeyApplication(ap);
                result = mra.ActivateKeyset(keysetSuperseded, keysetActivated);
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                try
                {
                    if (ap != null)
                        ap.Close();
                }
                catch (System.IO.IOException ex)
                {
                    /* stub */
                }
            }

            return result;
        }
    }
}
