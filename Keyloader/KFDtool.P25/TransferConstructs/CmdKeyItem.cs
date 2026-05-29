// SPDX-License-Identifier: MIT
/*
* KFDtool Container (EKC) Editor
* MIT Open Source. Use is subject to license terms.
* DO NOT ALTER OR REMOVE COPYRIGHT NOTICES OR THIS FILE HEADER.
*
*/

using System;
using System.Collections.Generic;

namespace KFDtool.P25.TransferConstructs
{
    /// <summary>
    /// 
    /// </summary>
    public class CmdKeyItem
    {
        private int keysetId;
        private int sln;
        private int algorithmId;
        private int keyId;
        private List<byte> key;

        /// <summary>
        /// 
        /// </summary>
        public bool UseActiveKeyset { get; set; }

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
        public bool IsKek { get; set; }

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
        public List<byte> Key
        {
            get { return key; }
            set
            {
                if (value == null)
                    throw new ArgumentNullException();
                key = value;
            }
        }

        /*
        ** Methods
        */

        /// <summary>
        /// 
        /// </summary>
        public CmdKeyItem()
        {
            Key = new List<byte>();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="useActiveKeyset"></param>
        /// <param name="keysetId"></param>
        /// <param name="sln"></param>
        /// <param name="isKek"></param>
        /// <param name="keyId"></param>
        /// <param name="algorithmId"></param>
        /// <param name="key"></param>
        public CmdKeyItem(bool useActiveKeyset, int keysetId, int sln, bool isKek, int keyId, int algorithmId, List<byte> key)
        {
            UseActiveKeyset = useActiveKeyset;
            KeysetId = keysetId;
            Sln = sln;
            IsKek = isKek;
            KeyId = keyId;
            AlgorithmId = algorithmId;
            Key = key;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format("UseActiveKeyset: {0}, KeysetId: {1}, Sln: {2}, IsKek: {3}, KeyId: {4}, AlgorithmId: {5}, Key: {6}", UseActiveKeyset, KeysetId, Sln, IsKek, KeyId, AlgorithmId, BitConverter.ToString(Key.ToArray()));
        }
    }
}
