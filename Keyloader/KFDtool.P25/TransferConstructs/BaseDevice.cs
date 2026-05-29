// SPDX-License-Identifier: MIT
/*
* KFDtool Container (EKC) Editor
* MIT Open Source. Use is subject to license terms.
* DO NOT ALTER OR REMOVE COPYRIGHT NOTICES OR THIS FILE HEADER.
*
*/

using KFDtool.Adapter.Device;

namespace KFDtool.P25.TransferConstructs
{
    /// <summary>
    /// 
    /// </summary>
    public class BaseDevice
    {
        /// <summary>
        /// 
        /// </summary>
        public enum DeviceTypeOptions
        {
            TwiKfdDevice,
            DliIp
        }

        /// <summary>
        /// 
        /// </summary>
        public DeviceTypeOptions DeviceType { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public TwiKfdDevice KfdDeviceType { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public TwiKfdtoolDevice TwiKfdtoolDevice { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public DliIpDevice DliIpDevice { get; set; }
    }
}
