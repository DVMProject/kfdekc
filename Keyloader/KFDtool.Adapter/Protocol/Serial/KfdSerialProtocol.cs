// SPDX-License-Identifier: MIT
/*
* KFDtool Container (EKC) Editor
* MIT Open Source. Use is subject to license terms.
* DO NOT ALTER OR REMOVE COPYRIGHT NOTICES OR THIS FILE HEADER.
*
*/

using System.Collections.Generic;

namespace KFDtool.Adapter.Protocol.Serial
{
    /// <summary>
    /// 
    /// </summary>
    internal interface KfdSerialProtocol
    {
        /*
        ** Methods
        */

        /// <summary>
        /// 
        /// </summary>
        void Open();

        /// <summary>
        /// 
        /// </summary>
        void Close();

        /// <summary>
        /// 
        /// </summary>
        void Clear();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="date"></param>
        void Send(List<byte> date);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="timeout"></param>
        /// <returns></returns>
        List<byte> Read(int timeout);

        /// <summary>
        /// 
        /// </summary>
        void Cancel();
    }
}
