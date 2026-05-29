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
    public class RspRsiInfo
    {
        private int rsi;
        private int mn;
        private int status;

        /// <summary>
        /// 
        /// </summary>
        public int RSI
        {
            get { return rsi; }
            set
            {
                if (value < 0x00 || value > 0xFFFFFF)
                    throw new ArgumentOutOfRangeException();

                rsi = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public int MN
        {
            get { return mn; }
            set
            {
                if (value < 0x00 || value > 0xFFFF)
                    throw new ArgumentOutOfRangeException();

                mn = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public int Status
        {
            get { return status; }
            set
            {
                if (value < 0x00 || value > 0xFF)
                    throw new ArgumentOutOfRangeException();

                status = value;
            }
        }
    }
}
