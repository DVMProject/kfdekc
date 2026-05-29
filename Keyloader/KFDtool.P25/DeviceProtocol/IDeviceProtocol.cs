// SPDX-License-Identifier: MIT
/*
* KFDtool Container (EKC) Editor
* MIT Open Source. Use is subject to license terms.
* DO NOT ALTER OR REMOVE COPYRIGHT NOTICES OR THIS FILE HEADER.
*
*/

namespace KFDtool.P25.DeviceProtocol
{
    /// <summary>
    /// 
    /// </summary>
    public interface IDeviceProtocol
    {
        /*
        ** Methods
        */

        /// <summary>
        /// 
        /// </summary>
        void SendKeySignature();

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        DeviceType InitSession();

        /// <summary>
        /// 
        /// </summary>
        void CheckTargetMrConnection();

        /// <summary>
        /// 
        /// </summary>
        void EndSession();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="kmm"></param>
        /// <returns></returns>
        byte[] PerformKmmTransfer(byte[] kmm);
    }
}
