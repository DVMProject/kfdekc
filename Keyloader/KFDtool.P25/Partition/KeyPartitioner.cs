// SPDX-License-Identifier: MIT
/*
* KFDtool Container (EKC) Editor
* MIT Open Source. Use is subject to license terms.
* DO NOT ALTER OR REMOVE COPYRIGHT NOTICES OR THIS FILE HEADER.
*
*/

using System;
using System.Collections.Generic;

using KFDtool.P25.TransferConstructs;

namespace KFDtool.P25.Partition
{
    public class KeyPartitioner
    {
        public const int MAX_BYTES = 512;

        /*
        ** Methods
        */

        /// <summary>
        /// 
        /// </summary>
        /// <param name="inKeys"></param>
        /// <exception cref="Exception"></exception>
        private static void CheckForDifferentKeyLengths(List<CmdKeyItem> inKeys)
        {
            Dictionary<int, int> len = new Dictionary<int, int>();

            foreach (CmdKeyItem key in inKeys)
            {
                if (!len.ContainsKey(key.AlgorithmId))
                    len.Add(key.AlgorithmId, key.Key.Count);
                else
                {
                    if (len[key.AlgorithmId] != key.Key.Count)
                        throw new Exception("more than one length of key per algorithm id");
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="keyLength"></param>
        /// <param name="maxBytes"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        private static int CalcMaxKeysPerKmm(int keyLength, int maxBytes)
        {
            // TODO make this calc more dynamic
            int availBytes = maxBytes - 27;
            int keyItemBytes = 5 + keyLength;
            int maxKeys = availBytes / keyItemBytes;

            if (maxKeys < 1)
                throw new Exception("key too large for kmm");

            return maxKeys;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="inKeys"></param>
        /// <param name="outKeys"></param>
        /// <param name="maxBytes"></param>
        private static void PartitionByAlg(List<CmdKeyItem> inKeys, List<List<CmdKeyItem>> outKeys, int maxBytes)
        {
            Dictionary<int, List<CmdKeyItem>> alg = new Dictionary<int, List<CmdKeyItem>>();

            foreach (CmdKeyItem keyItem in inKeys)
            {
                if (!alg.ContainsKey(keyItem.AlgorithmId))
                    alg.Add(keyItem.AlgorithmId, new List<CmdKeyItem>());

                alg[keyItem.AlgorithmId].Add(keyItem);
            }

            foreach (KeyValuePair<int, List<CmdKeyItem>> algGroup in alg)
            {
                int maxKeys = CalcMaxKeysPerKmm(algGroup.Value[0].Key.Count, maxBytes);
                PartitionByType(maxKeys, algGroup.Value, outKeys);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="maxKeys"></param>
        /// <param name="inKeys"></param>
        /// <param name="outKeys"></param>
        private static void PartitionByType(int maxKeys, List<CmdKeyItem> inKeys, List<List<CmdKeyItem>> outKeys)
        {
            List<CmdKeyItem> tek = new List<CmdKeyItem>();
            List<CmdKeyItem> kek = new List<CmdKeyItem>();

            foreach (CmdKeyItem keyItem in inKeys)
            {
                if (keyItem.IsKek)
                    kek.Add(keyItem);
                else
                    tek.Add(keyItem);
            }

            PartitionByActive(maxKeys, tek, outKeys);
            PartitionByActive(maxKeys, kek, outKeys);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="maxKeys"></param>
        /// <param name="inKeys"></param>
        /// <param name="outKeys"></param>
        private static void PartitionByActive(int maxKeys, List<CmdKeyItem> inKeys, List<List<CmdKeyItem>> outKeys)
        {
            List<CmdKeyItem> act = new List<CmdKeyItem>();
            List<CmdKeyItem> def = new List<CmdKeyItem>();

            foreach (CmdKeyItem keyItem in inKeys)
            {
                if (keyItem.UseActiveKeyset)
                    act.Add(keyItem);
                else
                    def.Add(keyItem);
            }

            PartitionByLength(maxKeys, act, outKeys);
            PartitionByKeyset(maxKeys, def, outKeys);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="maxKeys"></param>
        /// <param name="inKeys"></param>
        /// <param name="outKeys"></param>
        private static void PartitionByKeyset(int maxKeys, List<CmdKeyItem> inKeys, List<List<CmdKeyItem>> outKeys)
        {
            Dictionary<int, List<CmdKeyItem>> kset = new Dictionary<int, List<CmdKeyItem>>();

            foreach (CmdKeyItem keyItem in inKeys)
            {
                if (!kset.ContainsKey(keyItem.KeysetId))
                    kset.Add(keyItem.KeysetId, new List<CmdKeyItem>());

                kset[keyItem.KeysetId].Add(keyItem);
            }

            foreach (KeyValuePair<int, List<CmdKeyItem>> ksetGroup in kset)
                PartitionByLength(maxKeys, ksetGroup.Value, outKeys);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="maxKeys"></param>
        /// <param name="inKeys"></param>
        /// <param name="outKeys"></param>
        private static void PartitionByLength(int maxKeys, List<CmdKeyItem> inKeys, List<List<CmdKeyItem>> outKeys)
        {
            for (int i = 0; i < inKeys.Count; i += maxKeys)
                outKeys.Add(inKeys.GetRange(i, Math.Min(maxKeys, inKeys.Count - i)));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="inKeys"></param>
        /// <param name="maxBytes"></param>
        /// <returns></returns>
        public static List<List<CmdKeyItem>> PartitionKeys(List<CmdKeyItem> inKeys, int maxBytes = MAX_BYTES)
        {
            CheckForDifferentKeyLengths(inKeys);
            List<List<CmdKeyItem>> outKeys = new List<List<CmdKeyItem>>();
            PartitionByAlg(inKeys, outKeys, maxBytes);
            return outKeys;
        }
    }
}
