// SPDX-License-Identifier: MIT
/*
* KFDtool Container (EKC) Editor
* MIT Open Source. Use is subject to license terms.
* DO NOT ALTER OR REMOVE COPYRIGHT NOTICES OR THIS FILE HEADER.
*
*/

using System;
using System.Collections.Generic;
using System.Linq;

using KFDtool.Adapter.Protocol.Adapter;
using KFDtool.P25.Constant;
using KFDtool.P25.DeviceProtocol;
using KFDtool.P25.Kmm;

using KFDEKC.Shared;

namespace KFDtool.P25.ThreeWire
{
    /// <summary>
    /// 
    /// </summary>
    public class ThreeWireProtocol : IDeviceProtocol
    {
        private const int TIMEOUT_NONE = 0; // no timeout
        private const int TIMEOUT_STD = 5000; // 5 second timeout

        private const byte OPCODE_READY_REQ = 0xC0;
        private const byte OPCODE_READY_GENERAL_MODE_MR = 0xD0;
        private const byte OPCODE_READY_GENERAL_MODE_KVL = 0xD1;
        private const byte OPCODE_TRANSFER_DONE = 0xC1;
        private const byte OPCODE_KMM = 0xC2;
        private const byte OPCODE_DISCONNECT_ACK = 0x90;
        private const byte OPCODE_DISCONNECT = 0x92;

        public event EventHandler StatusChanged;

        private string _status;

        public string Status
        {
            get { return _status; }
            set
            {
                if (_status != value)
                {
                    _status = value;
                    OnStatusChanged(new EventArgs());
                }
            }
        }

        private AdapterProtocol Protocol;

        /*
        ** Methods
        */

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ap"></param>
        public ThreeWireProtocol(AdapterProtocol ap)
        {
            Protocol = ap;
        }

        /// <summary>
        /// 
        /// </summary>
        public void SendKeySignature()
        {
            if (Protocol.FeatureAvailableSendKeySignatureAndReadyReq)
                Protocol.SendKeySignatureAndReadyReq();
            else
                Protocol.SendKeySignature();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public DeviceType InitSession()
        {
            if (!Protocol.FeatureAvailableSendKeySignatureAndReadyReq)
            {
                // send ready req opcode
                List<byte> cmd = new List<byte>();
                cmd.Add(OPCODE_READY_REQ);
                Protocol.SendData(cmd);
            }

            // receive ready general mode opcode
            byte rsp = Protocol.GetByte(TIMEOUT_STD);
            switch (rsp)
            {
                case OPCODE_READY_GENERAL_MODE_MR: return DeviceType.Mr;
                case OPCODE_READY_GENERAL_MODE_KVL: return DeviceType.Kvl;
                default: throw new Exception("mr: unexpected opcode");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void CheckTargetMrConnection()
        {
            SendKeySignature();
            InitSession();
            EndSession();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="kmm"></param>
        /// <returns></returns>
        private List<byte> CreateKmmFrame(List<byte> kmm)
        {
            // create body
            List<byte> body = new List<byte>();

            body.Add(0x00); // control
            body.Add(0xFF); // destination RSI high byte
            body.Add(0xFF); // destination RSI mid byte
            body.Add(0xFF); // destination RSI low byte
            body.AddRange(kmm); // kmm

            // calculate crc
            byte[] crc = CRC16.CalculateCrc(body.ToArray());

            // create frame
            List<byte> frame = new List<byte>();

            int length = body.Count + 2; // control + dest rsi + kmm + crc

            frame.Add(OPCODE_KMM); // kmm opcode

            frame.Add((byte)((length >> 8) & 0xFF)); // length high byte
            frame.Add((byte)(length & 0xFF)); // length low byte

            frame.AddRange(body); // kmm body

            frame.Add(crc[0]); // crc high byte
            frame.Add(crc[1]); // crc low byte

            return frame;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        private List<byte> ParseKmmFrame()
        {
            byte temp;

            int length = 0;

            // receive length high byte
            temp = Protocol.GetByte(TIMEOUT_STD);

            length |= (temp & 0xFF) << 8;

            // receive length low byte
            temp = Protocol.GetByte(TIMEOUT_STD);

            length |= temp & 0xFF;

            List<byte> toCrc = new List<byte>();

            // receive control
            temp = Protocol.GetByte(TIMEOUT_STD);
            toCrc.Add(temp);

            // receive dest rsi high byte
            temp = Protocol.GetByte(TIMEOUT_STD);
            toCrc.Add(temp);

            // receive dest rsi mid byte
            temp = Protocol.GetByte(TIMEOUT_STD);
            toCrc.Add(temp);

            // receive dest rsi low byte
            temp = Protocol.GetByte(TIMEOUT_STD);
            toCrc.Add(temp);

            int bodyLength = length - 6;

            List<byte> kmm = new List<byte>();

            for (int i = 0; i < bodyLength; i++)
            {
                temp = Protocol.GetByte(TIMEOUT_STD);
                kmm.Add(temp);
            }

            toCrc.AddRange(kmm);

            // calculate crc
            byte[] expectedCrc = CRC16.CalculateCrc(toCrc.ToArray());

            byte[] crc = new byte[2];

            // receive crc high byte
            crc[0] = Protocol.GetByte(TIMEOUT_STD);

            // receive crc low byte
            crc[1] = Protocol.GetByte(TIMEOUT_STD);

            if (expectedCrc[0] != crc[0])
                throw new Exception(string.Format("mr: crc high byte mismatch, expected: 0x{0:X2}, got: 0x{1:X2}", expectedCrc[0], crc[0]));

            if (expectedCrc[1] != crc[1])
                throw new Exception(string.Format("mr: crc low byte mismatch, expected: 0x{0:X2}, got: 0x{1:X2}", expectedCrc[1], crc[1]));

            return kmm;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <exception cref="Exception"></exception>
        public void EndSession()
        {
            // send transfer done opcode
            List<byte> cmd1 = new List<byte>();
            cmd1.Add(OPCODE_TRANSFER_DONE);
            Protocol.SendData(cmd1);

            // receive transfer done opcode
            byte rsp1 = Protocol.GetByte(TIMEOUT_STD);
            if (rsp1 != OPCODE_TRANSFER_DONE)
                throw new Exception("mr: unexpected opcode");

            // send disconnect opcode
            List<byte> cmd2 = new List<byte>();
            cmd2.Add(OPCODE_DISCONNECT);
            Protocol.SendData(cmd2);

            // receive disconnect ack opcode
            byte rsp2 = Protocol.GetByte(TIMEOUT_STD);
            if (rsp2 != OPCODE_DISCONNECT_ACK)
                throw new Exception("mr: unexpected opcode");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="inKmm"></param>
        /// <exception cref="Exception"></exception>
        private void SendKmm(byte[] inKmm)
        {
            if (inKmm.Length > 512)
                throw new Exception("kmm exceeds max size");

            List<byte> txFrame = CreateKmmFrame(inKmm.ToList());
            Protocol.SendData(txFrame);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="inKmm"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public byte[] PerformKmmTransfer(byte[] inKmm)
        {
            // send kmm frame
            SendKmm(inKmm);

            byte rx;

            // receive kmm opcode
            try
            {
                rx = Protocol.GetByte(TIMEOUT_STD);
            }
            catch (Exception)
            {
                string msg = string.Format("in: timed out waiting for kmm opcode");
                throw new Exception(msg);
            }

            if (rx != OPCODE_KMM)
            {
                string msg = string.Format("in: unexpected kmm opcode, expected ({0}) got ({1})", Utility.DataFormat(OPCODE_KMM), Utility.DataFormat(rx));
                throw new Exception(msg);
            }

            // receive kmm frame
            byte[] rxFrame = ParseKmmFrame().ToArray();

            return rxFrame;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        private void OnStatusChanged(EventArgs e)
        {
            EventHandler handler = StatusChanged;
            if (handler != null)
                handler(this, e);
        }

        /// <summary>
        /// 
        /// </summary>
        public void MrRunProducer()
        {
            try
            {
                while (true)
                {
                    byte rx;

                    /* RX: KEY SIGNATURE */

                    // currently there is no rx key signature function in the adapter
                    // however, the key signature will appear as a 0x00 byte

                    // the 5 second timeout should prevent most sync issues, however
                    // a rx key signature function should be added to the adapter
                    // to make this more robust and correct

                    rx = Protocol.GetByte(TIMEOUT_NONE);

                    byte sig = 0x00; // key signature

                    if (rx != sig)
                    {
                        string msg = string.Format("in: unexpected key signature opcode, expected ({0}) got ({1})", Utility.DataFormat(sig), Utility.DataFormat(rx));
                        continue;
                    }

                    /* RX: READY REQUEST */

                    try
                    {
                        rx = Protocol.GetByte(TIMEOUT_STD);
                    }
                    catch (Exception)
                    {
                        string msg = string.Format("in: timed out waiting for ready request opcode");
                        continue;
                    }

                    if (rx != OPCODE_READY_REQ)
                    {
                        string msg = string.Format("in: unexpected ready request opcode, expected ({0}) got ({1})", Utility.DataFormat(OPCODE_READY_REQ), Utility.DataFormat(rx));
                        continue;
                    }

                    /* TX: READY GENERAL MODE */

                    Protocol.SendByte(OPCODE_READY_GENERAL_MODE_MR);

                    while (true)
                    {
                        /* RX: FRAME TYPE */

                        try
                        {
                            rx = Protocol.GetByte(TIMEOUT_STD);
                        }
                        catch (Exception)
                        {
                            string msg = string.Format("in: timed out waiting for frame type opcode");
                            break;
                        }

                        if (rx == OPCODE_KMM)
                        {
                            List<byte> rxFrame;

                            try
                            {
                                rxFrame = ParseKmmFrame();
                            }
                            catch (Exception ex)
                            {
                                break;
                            }

                            KmmFrame kfdKmmFrame = null;

                            try
                            {
                                kfdKmmFrame = new KmmFrame(false, rxFrame.ToArray());
                            }
                            catch (Exception ex)
                            {
                                byte[] message = rxFrame.ToArray();

                                if (message.Length != 0)
                                {
                                    NegativeAcknowledgment kmm = new NegativeAcknowledgment();

                                    kmm.AcknowledgedMessageId = (MessageId)message[0];
                                    kmm.Status = OperationStatus.InvalidMessageId;

                                    KmmFrame frame = new KmmFrame(kmm);

                                    SendKmm(frame.ToBytes());
                                }

                                continue;
                            }

                            KmmBody kfdKmmBody = kfdKmmFrame.KmmBody;

                            if (kfdKmmBody is InventoryCommandListActiveKsetIds)
                            {
                                InventoryResponseListActiveKsetIds mrKmm = new InventoryResponseListActiveKsetIds();

                                // do not return any keysets, to match factory Motorola SU behavior

                                KmmFrame commandKmmFrame = new KmmFrame(mrKmm);

                                SendKmm(commandKmmFrame.ToBytes());
                            }
                            else if (kfdKmmBody is InventoryCommandListRsiItems)
                            {
                                InventoryResponseListRsiItems mrKmm = new InventoryResponseListRsiItems();

                                RsiItem item = new RsiItem();

                                // set RSI and message number to match factory Motorola SU behavior

                                item.RSI = 0x000001;
                                item.MessageNumber = 0x0000;

                                mrKmm.RsiItems.Add(item);

                                KmmFrame commandKmmFrame = new KmmFrame(mrKmm);

                                SendKmm(commandKmmFrame.ToBytes());
                            }
                            else if (kfdKmmBody is ModifyKeyCommand)
                            {
                                ModifyKeyCommand cmdKmm = kfdKmmBody as ModifyKeyCommand;

                                RekeyAcknowledgment rspKmm = new RekeyAcknowledgment();

                                rspKmm.MessageIdAcknowledged = MessageId.ModifyKeyCommand;
                                rspKmm.NumberOfItems = cmdKmm.KeyItems.Count;

                                for (int i = 0; i < cmdKmm.KeyItems.Count; i++)
                                {
                                    KeyItem item = cmdKmm.KeyItems[i];

                                    string algName = string.Empty;

                                    if (Enum.IsDefined(typeof(AlgorithmId), (byte)cmdKmm.AlgorithmId))
                                        algName = ((AlgorithmId)cmdKmm.AlgorithmId).ToString();
                                    else
                                        algName = "UNKNOWN";

                                    Status +=
                                        string.Format("Keyset ID: {0} (dec), {0:X} (hex)", cmdKmm.KeysetId) + Environment.NewLine +
                                        string.Format("SLN/CKR: {0} (dec), {0:X} (hex)", item.SLN) + Environment.NewLine +
                                        string.Format("Key ID: {0} (dec), {0:X} (hex)", item.KeyId) + Environment.NewLine +
                                        string.Format("Algorithm: {0} (dec), {0:X} (hex), {1}", cmdKmm.AlgorithmId, algName) + Environment.NewLine +
                                        string.Format("Key: {0}", BitConverter.ToString(item.Key).Replace("-", string.Empty)) + Environment.NewLine +
                                        "--" + Environment.NewLine;

                                    KeyStatus status = new KeyStatus();

                                    status.AlgorithmId = cmdKmm.AlgorithmId;
                                    status.KeyId = item.KeyId;
                                    status.Status = 0x00; // command was performed

                                    rspKmm.Keys.Add(status);
                                }

                                KmmFrame cmdKmmFrame = new KmmFrame(rspKmm);

                                SendKmm(cmdKmmFrame.ToBytes());
                            }
                        }
                        else if (rx == OPCODE_TRANSFER_DONE)
                        {
                            /* TX: TRANSFER DONE */

                            Protocol.SendByte(OPCODE_TRANSFER_DONE);

                            /* RX: DISCONNECT */

                            try
                            {
                                rx = Protocol.GetByte(TIMEOUT_STD);
                            }
                            catch (Exception)
                            {
                                string msg = string.Format("in: timed out waiting for disconnect opcode");
                                break;
                            }

                            if (rx != OPCODE_DISCONNECT)
                            {
                                string msg = string.Format("in: unexpected disconnect opcode, expected ({0}) got ({1})", Utility.DataFormat(OPCODE_DISCONNECT), Utility.DataFormat(rx));
                                break;
                            }

                            /* TX: DISCONNECT ACKNOWLEDGE */

                            Protocol.SendByte(OPCODE_DISCONNECT_ACK);
                            break;
                        }
                        else
                        {
                            string msg = string.Format("in: unexpected frame type opcode ({0})", Utility.DataFormat(rx));
                            break;
                        }
                    }
                }
            }
            catch (OperationCanceledException)
            {
                return;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
