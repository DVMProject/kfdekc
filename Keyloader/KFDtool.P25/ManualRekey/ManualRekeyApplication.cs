// SPDX-License-Identifier: MIT
/*
* KFDtool Container (EKC) Editor
* MIT Open Source. Use is subject to license terms.
* DO NOT ALTER OR REMOVE COPYRIGHT NOTICES OR THIS FILE HEADER.
*
*/

using System;
using System.Collections.Generic;

using KFDtool.Adapter.Protocol.Adapter;
using KFDtool.P25.DataLinkIndependent;
using KFDtool.P25.DeviceProtocol;
using KFDtool.P25.Kmm;
using KFDtool.P25.NetworkProtocol;
using KFDtool.P25.Partition;
using KFDtool.P25.ThreeWire;
using KFDtool.P25.TransferConstructs;

namespace KFDtool.P25.ManualRekey
{
    public class ManualRekeyApplication
    {
        private IDeviceProtocol DeviceProtocol;

        /// <summary>
        /// 
        /// </summary>
        private bool WithPreamble { get; set; }

        /// <summary>
        /// 
        /// </summary>
        private byte Mfid { get; set; }

        /*
        ** Methods
        */

        /// <summary>
        /// 
        /// </summary>
        /// <param name="adapterProtocol"></param>
        public ManualRekeyApplication(AdapterProtocol adapterProtocol)
        {
            DeviceProtocol = new ThreeWireProtocol(adapterProtocol);
            WithPreamble = false;
            Mfid = 0x00;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="udpProtocol"></param>
        /// <param name="motVariant"></param>
        public ManualRekeyApplication(UdpProtocol udpProtocol, bool motVariant)
        {
            DeviceProtocol = new DataLinkIndependentProtocol(udpProtocol, motVariant);
            WithPreamble = true;
            Mfid = motVariant ? (byte)0x90 : (byte)0x00;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private DeviceType Begin()
        {
            DeviceProtocol.SendKeySignature();
            return DeviceProtocol.InitSession();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="commandKmmBody"></param>
        /// <returns></returns>
        private KmmBody TxRxKmm(KmmBody commandKmmBody)
        {
            KmmFrame commandKmmFrame = new KmmFrame(commandKmmBody);

            byte[] toRadio = WithPreamble ? commandKmmFrame.ToBytesWithPreamble(Mfid) : commandKmmFrame.ToBytes();

            byte[] fromRadio = DeviceProtocol.PerformKmmTransfer(toRadio);

            KmmFrame responseKmmFrame = new KmmFrame(WithPreamble, fromRadio);

            return responseKmmFrame.KmmBody;
        }

        /// <summary>
        /// 
        /// </summary>
        private void End()
        {
            DeviceProtocol.EndSession();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="keyItems"></param>
        /// <param name="kek"></param>
        public void Keyload(List<CmdKeyItem> keyItems, CmdKeyItem kek = null)
        {
            DeviceType deviceType = Begin();

            int maxBytes = KeyPartitioner.MAX_BYTES;
            if (deviceType == DeviceType.Kvl)
            {
                // Adjusted to definitely fit KMM entirely within a single CMD_SEND_BYTES because
                // the KVL 4000 doesn't like when a KMM has any timing delays in the middle of it
                maxBytes = 250;
            }
            List<List<CmdKeyItem>> keyGroups = KeyPartitioner.PartitionKeys(keyItems, maxBytes);

            try
            {
                // KVL does not support inventorying the active keyset; use keyset 1 as the default
                int activeKeysetId = 1;
             
                if (deviceType == DeviceType.Mr)
                {
                    InventoryCommandListActiveKsetIds cmdKmmBody1 = new InventoryCommandListActiveKsetIds();

                    KmmBody rspKmmBody1 = TxRxKmm(cmdKmmBody1);

                    if (rspKmmBody1 is InventoryResponseListActiveKsetIds)
                    {
                        InventoryResponseListActiveKsetIds kmm = rspKmmBody1 as InventoryResponseListActiveKsetIds;

                        // TODO support more than one crypto group
                        if (kmm.KsetIds.Count > 0)
                            activeKeysetId = kmm.KsetIds[0];
                        else
                            activeKeysetId = 1; // to match KVL3000+ R3.53.03 behavior
                    }
                    else if (rspKmmBody1 is NegativeAcknowledgment)
                    {
                        NegativeAcknowledgment kmm = rspKmmBody1 as NegativeAcknowledgment;

                        string statusDescr = OperationStatusExtensions.ToStatusString(kmm.Status);
                        string statusReason = OperationStatusExtensions.ToReasonString(kmm.Status);
                        throw new Exception(string.Format("received negative acknowledgment{0}status: {1} (0x{2:X2}){0}{3}", Environment.NewLine, statusDescr, kmm.Status, statusReason));
                    }
                    else
                        throw new Exception("unexpected kmm");
                }

                foreach (List<CmdKeyItem> keyGroup in keyGroups)
                {
                    ModifyKeyCommand modifyKeyCommand = new ModifyKeyCommand();

                    // TODO support more than one crypto group
                    if (keyGroup[0].UseActiveKeyset && !keyGroup[0].IsKek)
                        modifyKeyCommand.KeysetId = activeKeysetId;
                    else if (keyGroup[0].UseActiveKeyset && keyGroup[0].IsKek)
                        modifyKeyCommand.KeysetId = 0xFF; // to match KVL3000+ R3.53.03 behavior
                    else
                        modifyKeyCommand.KeysetId = keyGroup[0].KeysetId;

                    modifyKeyCommand.AlgorithmId = keyGroup[0].AlgorithmId;

                    if (kek != null)
                        modifyKeyCommand.Kek = kek;

                    foreach (CmdKeyItem key in keyGroup)
                    {
                        KeyItem keyItem = new KeyItem();
                        keyItem.SLN = key.Sln;
                        keyItem.KeyId = key.KeyId;
                        keyItem.Key = key.Key.ToArray();
                        keyItem.KEK = key.IsKek;
                        keyItem.Erase = false;

                        modifyKeyCommand.KeyItems.Add(keyItem);
                    }

                    KmmBody rspKmmBody2 = TxRxKmm(modifyKeyCommand);

                    if (rspKmmBody2 is RekeyAcknowledgment)
                    {
                        RekeyAcknowledgment kmm = rspKmmBody2 as RekeyAcknowledgment;

                        for (int i = 0; i < kmm.Keys.Count; i++)
                        {
                            KeyStatus status = kmm.Keys[i];

                            if (status.Status != 0)
                            {
                                string statusDescr = OperationStatusExtensions.ToStatusString((OperationStatus)status.Status);
                                string statusReason = OperationStatusExtensions.ToReasonString((OperationStatus)status.Status);
                                throw new Exception(
                                    string.Format(
                                        "received unexpected key status{0}" +
                                        "algorithm id: {1} (0x{1:X}){0}" +
                                        "key id: {2} (0x{2:X}){0}" +
                                        "status: {3} (0x{3:X}){0}" +
                                        "status description: {4}{0}" +
                                        "status reason: {5}",
                                        Environment.NewLine, status.AlgorithmId, status.KeyId, status.Status, statusDescr, statusReason
                                    )
                                );
                            }
                        }
                    }
                    else if (rspKmmBody2 is NegativeAcknowledgment)
                    {
                        NegativeAcknowledgment kmm = rspKmmBody2 as NegativeAcknowledgment;

                        string statusDescr = OperationStatusExtensions.ToStatusString(kmm.Status);
                        string statusReason = OperationStatusExtensions.ToReasonString(kmm.Status);
                        throw new Exception(string.Format("received negative acknowledgment{0}status: {1} (0x{2:X2}){0}{3}", Environment.NewLine, statusDescr, kmm.Status, statusReason));
                    }
                    else
                        throw new Exception("received unexpected kmm");
                }
            }
            catch
            {
                End();
                throw;
            }

            End();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="keyItems"></param>
        public void EraseKeys(List<CmdKeyItem> keyItems)
        {
            List<List<CmdKeyItem>> keyGroups = KeyPartitioner.PartitionKeys(keyItems);

            Begin();

            try
            {
                InventoryCommandListActiveKsetIds cmdKmmBody1 = new InventoryCommandListActiveKsetIds();

                KmmBody rspKmmBody1 = TxRxKmm(cmdKmmBody1);

                int activeKeysetId = 0;

                if (rspKmmBody1 is InventoryResponseListActiveKsetIds)
                {
                    InventoryResponseListActiveKsetIds kmm = rspKmmBody1 as InventoryResponseListActiveKsetIds;

                    // TODO support more than one crypto group
                    if (kmm.KsetIds.Count > 0)
                        activeKeysetId = kmm.KsetIds[0];
                    else
                        activeKeysetId = 1; // to match KVL3000+ R3.53.03 behavior
                }
                else if (rspKmmBody1 is NegativeAcknowledgment)
                {
                    NegativeAcknowledgment kmm = rspKmmBody1 as NegativeAcknowledgment;

                    string statusDescr = OperationStatusExtensions.ToStatusString(kmm.Status);
                    string statusReason = OperationStatusExtensions.ToReasonString(kmm.Status);
                    throw new Exception(string.Format("received negative acknowledgment{0}status: {1} (0x{2:X2}){0}{3}", Environment.NewLine, statusDescr, kmm.Status, statusReason));
                }
                else
                    throw new Exception("unexpected kmm");

                foreach (List<CmdKeyItem> keyGroup in keyGroups)
                {
                    ModifyKeyCommand modifyKeyCommand = new ModifyKeyCommand();

                    // TODO support more than one crypto group
                    if (keyGroup[0].UseActiveKeyset && !keyGroup[0].IsKek)
                        modifyKeyCommand.KeysetId = activeKeysetId;
                    else if (keyGroup[0].UseActiveKeyset && keyGroup[0].IsKek)
                        modifyKeyCommand.KeysetId = 0xFF; // to match KVL3000+ R3.53.03 behavior
                    else
                        modifyKeyCommand.KeysetId = keyGroup[0].KeysetId;

                    modifyKeyCommand.AlgorithmId = 0x81; // to match KVL3000+ R3.53.03 behavior

                    foreach (CmdKeyItem key in keyGroup)
                    {
                        KeyItem keyItem = new KeyItem();
                        keyItem.SLN = key.Sln;
                        keyItem.KeyId = 65535; // to match KVL3000+ R3.53.03 behavior
                        keyItem.Key = new byte[] { 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF }; // to match KVL3000+ R3.53.03 behavior
                        keyItem.KEK = key.IsKek;
                        keyItem.Erase = true;

                        modifyKeyCommand.KeyItems.Add(keyItem);
                    }

                    KmmBody rspKmmBody2 = TxRxKmm(modifyKeyCommand);

                    if (rspKmmBody2 is RekeyAcknowledgment)
                    {
                        RekeyAcknowledgment kmm = rspKmmBody2 as RekeyAcknowledgment;

                        for (int i = 0; i < kmm.Keys.Count; i++)
                        {
                            KeyStatus status = kmm.Keys[i];

                            if (status.Status != 0)
                            {
                                string statusDescr = OperationStatusExtensions.ToStatusString((OperationStatus)status.Status);
                                string statusReason = OperationStatusExtensions.ToReasonString((OperationStatus)status.Status);
                                throw new Exception(
                                    string.Format(
                                        "received unexpected key status{0}" +
                                        "algorithm id: {1} (0x{1:X}){0}" +
                                        "key id: {2} (0x{2:X}){0}" +
                                        "status: {3} (0x{3:X}){0}" +
                                        "status description: {4}{0}" +
                                        "status reason: {5}",
                                        Environment.NewLine, status.AlgorithmId, status.KeyId, status.Status, statusDescr, statusReason
                                    )
                                );
                            }
                        }
                    }
                    else if (rspKmmBody2 is NegativeAcknowledgment)
                    {
                        NegativeAcknowledgment kmm = rspKmmBody2 as NegativeAcknowledgment;

                        string statusDescr = OperationStatusExtensions.ToStatusString(kmm.Status);
                        string statusReason = OperationStatusExtensions.ToReasonString(kmm.Status);
                        throw new Exception(string.Format("received negative acknowledgment{0}status: {1} (0x{2:X2}){0}{3}", Environment.NewLine, statusDescr, kmm.Status, statusReason));
                    }
                    else
                        throw new Exception("received unexpected kmm");
                }
            }
            catch
            {
                End();
                throw;
            }

            End();
        }

        /// <summary>
        /// 
        /// </summary>
        public void EraseAllKeys()
        {
            Begin();

            try
            {
                ZeroizeCommand commandKmmBody = new ZeroizeCommand();

                KmmBody responseKmmBody = TxRxKmm(commandKmmBody);

                if (responseKmmBody is ZeroizeResponse)
                    End();
                else if (responseKmmBody is NegativeAcknowledgment)
                {
                    NegativeAcknowledgment kmm = responseKmmBody as NegativeAcknowledgment;

                    string statusDescr = OperationStatusExtensions.ToStatusString(kmm.Status);
                    string statusReason = OperationStatusExtensions.ToReasonString(kmm.Status);
                    throw new Exception(string.Format("received negative acknowledgment{0}status: {1} (0x{2:X2}){0}{3}", Environment.NewLine, statusDescr, kmm.Status, statusReason));
                }
                else
                {
                    throw new Exception("unexpected kmm");
                }
            }
            catch
            {
                End();
                throw;
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        public List<RspKeyInfo> ViewKeyInfo()
        {
            List<RspKeyInfo> result = new List<RspKeyInfo>();

            Begin();

            try
            {
                bool more = true;
                int marker = 0;

                while (more)
                {
                    InventoryCommandListActiveKeys commandKmmBody = new InventoryCommandListActiveKeys();
                    commandKmmBody.InventoryMarker = marker;

                    if (WithPreamble)
                        commandKmmBody.MaxKeysRequested = 75; // for DLI, to match KVL4000 1.3.5000.243 behavior
                    else
                        commandKmmBody.MaxKeysRequested = 78; // for TWI, per TIA 102.AACD-A 3.9.2.11

                    KmmBody responseKmmBody = TxRxKmm(commandKmmBody);

                    if (responseKmmBody is InventoryResponseListActiveKeys)
                    {
                        InventoryResponseListActiveKeys kmm = responseKmmBody as InventoryResponseListActiveKeys;

                        marker = kmm.InventoryMarker;

                        if (marker == 0)
                            more = false;

                        for (int i = 0; i < kmm.Keys.Count; i++)
                        {
                            KeyInfo info = kmm.Keys[i];

                            RspKeyInfo res = new RspKeyInfo();

                            res.KeysetId = info.KeySetId;
                            res.Sln = info.SLN;
                            res.AlgorithmId = info.AlgorithmId;
                            res.KeyId = info.KeyId;

                            result.Add(res);
                        }
                    }
                    else if (responseKmmBody is NegativeAcknowledgment)
                    {
                        NegativeAcknowledgment kmm = responseKmmBody as NegativeAcknowledgment;

                        string statusDescr = OperationStatusExtensions.ToStatusString(kmm.Status);
                        string statusReason = OperationStatusExtensions.ToReasonString(kmm.Status);
                        throw new Exception(string.Format("received negative acknowledgment{0}status: {1} (0x{2:X2}){0}{3}", Environment.NewLine, statusDescr, kmm.Status, statusReason));
                    }
                    else
                        throw new Exception("unexpected kmm");
                }
            }
            catch
            {
                End();
                throw;
            }

            End();

            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public int ViewKmfRsi()
        {
            int result = new int();

            Begin();

            try
            {
                InventoryCommandListKmfRsi commandKmmBody = new InventoryCommandListKmfRsi();

                KmmBody responseKmmBody = TxRxKmm(commandKmmBody);

                if (responseKmmBody is InventoryResponseListKmfRsi)
                {
                    InventoryResponseListKmfRsi kmm = responseKmmBody as InventoryResponseListKmfRsi;

                    result = kmm.KmfRsi;
                }
                else if (responseKmmBody is NegativeAcknowledgment)
                {
                    NegativeAcknowledgment kmm = responseKmmBody as NegativeAcknowledgment;

                    string statusDescr = OperationStatusExtensions.ToStatusString(kmm.Status);
                    string statusReason = OperationStatusExtensions.ToReasonString(kmm.Status);
                    throw new Exception(string.Format("received negative acknowledgment{0}status: {1} (0x{2:X2}){0}{3}", Environment.NewLine, statusDescr, kmm.Status, statusReason));
                }
                else
                    throw new Exception("unexpected kmm");
            }
            catch
            {
                End();
                throw;
            }

            End();
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public int ViewMnp()
        {
            int result = new int();

            Begin();

            try
            {
                InventoryCommandListMnp commandKmmBody = new InventoryCommandListMnp();

                KmmBody responseKmmBody = TxRxKmm(commandKmmBody);

                if (responseKmmBody is InventoryResponseListMnp)
                {
                    InventoryResponseListMnp kmm = responseKmmBody as InventoryResponseListMnp;

                    result = kmm.MessageNumberPeriod;
                }
                else if (responseKmmBody is NegativeAcknowledgment)
                {
                    NegativeAcknowledgment kmm = responseKmmBody as NegativeAcknowledgment;

                    string statusDescr = OperationStatusExtensions.ToStatusString(kmm.Status);
                    string statusReason = OperationStatusExtensions.ToReasonString(kmm.Status);
                    throw new Exception(string.Format("received negative acknowledgment{0}status: {1} (0x{2:X2}){0}{3}", Environment.NewLine, statusDescr, kmm.Status, statusReason));
                }
                else
                    throw new Exception("unexpected kmm");
            }
            catch
            {
                End();
                throw;
            }

            End();
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="kmfRsi"></param>
        /// <param name="mnp"></param>
        /// <returns></returns>
        public RspRsiInfo LoadConfig(int kmfRsi, int mnp)
        {
            RspRsiInfo result = new RspRsiInfo();

            Begin();

            try
            {
                LoadConfigCommand cmdKmmBody = new LoadConfigCommand();
                cmdKmmBody.KmfRsi = kmfRsi;
                cmdKmmBody.MessageNumberPeriod = mnp;
                KmmBody rspKmmBody = TxRxKmm(cmdKmmBody);
                if (rspKmmBody is LoadConfigResponse)
                {
                    LoadConfigResponse kmm = rspKmmBody as LoadConfigResponse;
                    result.RSI = kmm.RSI;
                    result.MN = kmm.MN;
                    result.Status = kmm.Status;
                }
                else if (rspKmmBody is NegativeAcknowledgment)
                {
                    NegativeAcknowledgment kmm = rspKmmBody as NegativeAcknowledgment;

                    string statusDescr = OperationStatusExtensions.ToStatusString(kmm.Status);
                    string statusReason = OperationStatusExtensions.ToReasonString(kmm.Status);
                    throw new Exception(string.Format("received negative acknowledgment{0}status: {1} (0x{2:X2}){0}{3}", Environment.NewLine, statusDescr, kmm.Status, statusReason));
                }
                else
                    throw new Exception("unexpected kmm");
            }
            catch
            {
                End();
                throw;
            }

            End();
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="rsiOld"></param>
        /// <param name="rsiNew"></param>
        /// <param name="mnp"></param>
        /// <returns></returns>
        public RspRsiInfo ChangeRsi(int rsiOld, int rsiNew, int mnp)
        {
            RspRsiInfo result = new RspRsiInfo();

            Begin();

            try
            {
                ChangeRsiCommand cmdKmmBody = new ChangeRsiCommand();
                cmdKmmBody.RsiOld = rsiOld;
                cmdKmmBody.RsiNew = rsiNew;
                cmdKmmBody.MessageNumber = mnp;

                KmmBody rspKmmBody = TxRxKmm(cmdKmmBody);

                if (rspKmmBody is ChangeRsiResponse)
                {
                    ChangeRsiResponse kmm = rspKmmBody as ChangeRsiResponse;
                    result.RSI = rsiNew;
                    result.MN = mnp;
                    result.Status = kmm.Status;
                }
                else if (rspKmmBody is NegativeAcknowledgment)
                {
                    NegativeAcknowledgment kmm = rspKmmBody as NegativeAcknowledgment;

                    string statusDescr = OperationStatusExtensions.ToStatusString(kmm.Status);
                    string statusReason = OperationStatusExtensions.ToReasonString(kmm.Status);
                    throw new Exception(string.Format("received negative acknowledgment{0}status: {1} (0x{2:X2}){0}{3}", Environment.NewLine, statusDescr, kmm.Status, statusReason));
                }
                else
                    throw new Exception("unexpected kmm");
            }
            catch
            {
                End();
                throw;
            }

            End();
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public List<RspRsiInfo> ViewRsiItems()
        {
            List<RspRsiInfo> result = new List<RspRsiInfo>();

            Begin();

            try
            {
                bool more = true;
                int marker = 0;

                while (more)
                {
                    InventoryCommandListRsiItems commandKmmBody = new InventoryCommandListRsiItems();

                    KmmBody responseKmmBody = TxRxKmm(commandKmmBody);

                    if (responseKmmBody is InventoryResponseListRsiItems)
                    {
                        InventoryResponseListRsiItems kmm = responseKmmBody as InventoryResponseListRsiItems;

                        if (marker == 0)
                            more = false;

                        for (int i = 0; i < kmm.RsiItems.Count; i++)
                        {
                            RsiItem item = kmm.RsiItems[i];

                            RspRsiInfo res = new RspRsiInfo();

                            res.RSI = (int)item.RSI;
                            res.MN = item.MessageNumber;

                            result.Add(res);
                        }
                    }
                    else if (responseKmmBody is NegativeAcknowledgment)
                    {
                        NegativeAcknowledgment kmm = responseKmmBody as NegativeAcknowledgment;

                        string statusDescr = OperationStatusExtensions.ToStatusString(kmm.Status);
                        string statusReason = OperationStatusExtensions.ToReasonString(kmm.Status);
                        throw new Exception(string.Format("received negative acknowledgment{0}status: {1} (0x{2:X2}){0}{3}", Environment.NewLine, statusDescr, kmm.Status, statusReason));
                    }
                    else
                        throw new Exception("unexpected kmm");
                }
            }
            catch
            {
                End();
                throw;
            }

            End();
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public List<RspKeysetInfo> ViewKeysetTaggingInfo()
        {
            List<RspKeysetInfo> result = new List<RspKeysetInfo>();

            Begin();

            try
            {
                InventoryCommandListKeysetTaggingInfo commandKmmBody = new InventoryCommandListKeysetTaggingInfo();

                KmmBody responseKmmBody = TxRxKmm(commandKmmBody);

                if (responseKmmBody is InventoryResponseListKeysetTaggingInfo)
                {
                    InventoryResponseListKeysetTaggingInfo kmm = responseKmmBody as InventoryResponseListKeysetTaggingInfo;

                    for (int i = 0; i < kmm.KeysetItems.Count; i++)
                    {
                        KeysetItem item = kmm.KeysetItems[i];

                        RspKeysetInfo res = new RspKeysetInfo();

                        res.KeysetId = item.KeysetId;
                        res.KeysetName = item.KeysetName;
                        res.KeysetType = item.KeysetType;
                        res.ActivationDateTime = item.ActivationDateTime;
                        res.ReservedField = item.ReservedField;

                        result.Add(res);
                    }
                }
                else if (responseKmmBody is NegativeAcknowledgment)
                {
                    NegativeAcknowledgment kmm = responseKmmBody as NegativeAcknowledgment;

                    string statusDescr = OperationStatusExtensions.ToStatusString(kmm.Status);
                    string statusReason = OperationStatusExtensions.ToReasonString(kmm.Status);
                    throw new Exception(string.Format("received negative acknowledgment{0}status: {1} (0x{2:X2}){0}{3}", Environment.NewLine, statusDescr, kmm.Status, statusReason));
                }
                else
                    throw new Exception("unexpected kmm");
            }
            catch
            {
                End();
                throw;
            }

            End();
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="keysetSuperseded"></param>
        /// <param name="keysetActivated"></param>
        /// <returns></returns>
        public RspChangeoverInfo ActivateKeyset(int keysetSuperseded, int keysetActivated)
        {
            RspChangeoverInfo result = new RspChangeoverInfo();

            Begin();

            try
            {
                ChangeoverCommand cmdKmmBody = new ChangeoverCommand();
                cmdKmmBody.KeysetIdSuperseded = keysetSuperseded;
                cmdKmmBody.KeysetIdActivated = keysetActivated;
                KmmBody rspKmmBody = TxRxKmm(cmdKmmBody);

                if (rspKmmBody is ChangeoverResponse)
                {
                    ChangeoverResponse kmm = rspKmmBody as ChangeoverResponse;

                    result.KeysetIdSuperseded = kmm.KeysetIdSuperseded;
                    result.KeysetIdActivated = kmm.KeysetIdActivated;
                }
                else if (rspKmmBody is NegativeAcknowledgment)
                {
                    NegativeAcknowledgment kmm = rspKmmBody as NegativeAcknowledgment;

                    string statusDescr = OperationStatusExtensions.ToStatusString(kmm.Status);
                    string statusReason = OperationStatusExtensions.ToReasonString(kmm.Status);
                    throw new Exception(string.Format("received negative acknowledgment{0}status: {1} (0x{2:X2}){0}{3}", Environment.NewLine, statusDescr, kmm.Status, statusReason));
                }
                else
                    throw new Exception("unexpected kmm");
            }
            catch
            {
                End();
                throw;
            }

            End();
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <exception cref="NotImplementedException"></exception>
        public void LoadAuthenticationKey()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <exception cref="NotImplementedException"></exception>
        public void DeleteAuthenticationKey()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <exception cref="NotImplementedException"></exception>
        public void ViewSuidInfo()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <exception cref="NotImplementedException"></exception>
        public void ViewActiveSuidInfo()
        {
            throw new NotImplementedException();
        }
    }
}
