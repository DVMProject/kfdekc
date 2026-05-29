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
    public class RspKeyInfo
    {
        private int keysetId;
        private int sln;
        private int algorithmId;
        private int keyId;

        /// <summary>
        /// 
        /// </summary>
        public int KeysetId
        {
            get { return keysetId; }
            set
            {
                if (value < 0x00 || value > 0xFF)
                    throw new ArgumentOutOfRangeException();

                keysetId = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public int Sln
        {
            get { return sln; }
            set
            {
                if (value < 0x0000 || value > 0xFFFF)
                    throw new ArgumentOutOfRangeException();

                sln = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public int AlgorithmId
        {
            get { return algorithmId; }
            set
            {
                if (value < 0x00 || value > 0xFF)
                    throw new ArgumentOutOfRangeException();

                algorithmId = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public int KeyId
        {
            get { return keyId; }
            set
            {
                if (value < 0x0000 || value > 0xFFFF)
                    throw new ArgumentOutOfRangeException();

                keyId = value;
            }
        }
    }
}
