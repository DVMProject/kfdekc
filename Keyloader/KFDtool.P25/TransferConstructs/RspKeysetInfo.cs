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
    public class RspKeysetInfo
    {
        private int keysetId;
        private int reservedField;

        /// <summary>
        /// 
        /// </summary>
        public string KeysetName { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string KeysetType { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public DateTime ActivationDateTime { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int KeysetId
        {
            get
            { return keysetId; }
            set
            {
                if (value < 0 || value > 0xFF)
                    throw new ArgumentOutOfRangeException();

                keysetId = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public int ReservedField
        {
            get { return reservedField; }
            set
            {
                if (value < 0 || value > 0xFFFFFF)
                    throw new ArgumentOutOfRangeException();

                reservedField = value;
            }
        }
    }
}
