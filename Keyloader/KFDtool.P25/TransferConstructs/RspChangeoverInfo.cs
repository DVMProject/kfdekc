// SPDX-License-Identifier: MIT
/*
* KFDtool Container (EKC) Editor
* MIT Open Source. Use is subject to license terms.
* DO NOT ALTER OR REMOVE COPYRIGHT NOTICES OR THIS FILE HEADER.
*
*/

using System;

namespace KFDtool.P25.TransferConstructs
{
    /// <summary>
    /// 
    /// </summary>
    public class RspChangeoverInfo
    {
        private int keysetIdSuperseded;
        private int keysetIdActivated;

        /// <summary>
        /// 
        /// </summary>
        public int KeysetIdSuperseded
        {
            get { return keysetIdSuperseded; }
            set
            {
                if (value < 0x00 || value > 0xFF)
                    throw new ArgumentOutOfRangeException();

                keysetIdSuperseded = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public int KeysetIdActivated
        {
            get { return keysetIdActivated; }
            set
            {
                if (value < 0x00 || value > 0xFF)
                    throw new ArgumentOutOfRangeException();

                keysetIdActivated = value;
            }
        }
    }
}
