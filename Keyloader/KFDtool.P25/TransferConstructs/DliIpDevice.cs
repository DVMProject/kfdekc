// SPDX-License-Identifier: MIT
/*
* KFDtool Container (EKC) Editor
* MIT Open Source. Use is subject to license terms.
* DO NOT ALTER OR REMOVE COPYRIGHT NOTICES OR THIS FILE HEADER.
*
*/

namespace KFDtool.P25.TransferConstructs
{
    /// <summary>
    /// 
    /// </summary>
    public class DliIpDevice
    {
        /// <summary>
        /// 
        /// </summary>
        public enum ProtocolOptions
        {
            UDP
        }

        /// <summary>
        /// 
        /// </summary>
        public enum VariantOptions
        {
            Standard,
            Motorola
        }

        /// <summary>
        /// 
        /// </summary>
        public ProtocolOptions Protocol { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Hostname { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int Port { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public VariantOptions Variant { get; set; }
    }
}
