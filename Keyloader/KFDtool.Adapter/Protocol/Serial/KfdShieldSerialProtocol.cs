// SPDX-License-Identifier: MIT
/*
* KFDtool Container (EKC) Editor
* MIT Open Source. Use is subject to license terms.
* DO NOT ALTER OR REMOVE COPYRIGHT NOTICES OR THIS FILE HEADER.
*
*/

using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Threading;

namespace KFDtool.Adapter.Protocol.Serial
{
    /// <summary>
    /// 
    /// </summary>
    public class KfdShieldSerialProtocol : KfdSerialProtocol
    {
        const byte SOM = 0x61;
        const byte SOM_PLACEHOLDER = 0x62;
        const byte EOM = 0x63;
        const byte EOM_PLACEHOLDER = 0x64;
        const byte ESC = 0x70;
        const byte ESC_PLACEHOLDER = 0x71;

        private static AutoResetEvent CancelRead = new AutoResetEvent(false);

        private List<byte> FrameBuffer;

        private bool FoundStart;

        private List<List<byte>> PacketBuffer;

        private static ManualResetEvent PacketReady = new ManualResetEvent(false);

        private SerialPort Port;

        /*
        ** Methods
        */

        /// <summary>
        /// 
        /// </summary>
        /// <param name="portName"></param>
        public KfdShieldSerialProtocol(string portName)
        {
            FrameBuffer = new List<byte>();

            FoundStart = false;

            PacketBuffer = new List<List<byte>>();

            Port = new SerialPort();

            Port.PortName = portName;
            Port.BaudRate = 115200;
            Port.Parity = Parity.None;
            Port.DataBits = 8;
            Port.StopBits = StopBits.One;
            
            Port.DataReceived += new SerialDataReceivedEventHandler(OnDataReceived);
        }

        /// <summary>
        /// 
        /// </summary>
        public void Open()
        {
            // don't open the port if it is open already
            if (!Port.IsOpen)
                Port.Open();
        }

        /// <summary>
        /// 
        /// </summary>
        public void Close()
        {
            Port.Close();
        }

        /// <summary>
        /// 
        /// </summary>
        public void Clear()
        {
            FrameBuffer.Clear();
            PacketBuffer.Clear();
            PacketReady.Reset();
            FoundStart = false;

            while (Port.BytesToRead > 0)
                Port.ReadByte();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        public void Send(List<byte> data)
        {
            List<byte> frameData = new List<byte>();

            frameData.Add(SOM);

            foreach (byte b in data)
            {
                if (b == ESC)
                {
                    frameData.Add(ESC);
                    frameData.Add(ESC_PLACEHOLDER);
                }
                else if (b == SOM)
                {
                    frameData.Add(ESC);
                    frameData.Add(SOM_PLACEHOLDER);
                }
                else if (b == EOM)
                {
                    frameData.Add(ESC);
                    frameData.Add(EOM_PLACEHOLDER);
                }
                else
                {
                    frameData.Add(b);
                }
            }

            frameData.Add(EOM);

            byte[] outData = frameData.ToArray();
            Port.Write(outData, 0, outData.Length);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        private byte[] ReadPacketFromPacketBuffer()
        {
            if (PacketBuffer.Count == 0)
                throw new Exception("no packet in packet buffer");

            byte[] packet = PacketBuffer[0].ToArray();

            PacketBuffer.RemoveAt(0);

            return packet;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="timeout"></param>
        /// <returns></returns>
        /// <exception cref="TimeoutException"></exception>
        /// <exception cref="OperationCanceledException"></exception>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public List<byte> Read(int timeout)
        {
            // if there are no packets in the buffer, wait until there is one
            if (PacketBuffer.Count == 0)
            {
                if (timeout > 0)
                {
                    if (!PacketReady.WaitOne(timeout))
                        throw new TimeoutException("timeout waiting for data");
                }
                else if (timeout == 0)
                {
                    WaitHandle[] handles = new WaitHandle[] { PacketReady, CancelRead };

                    if (WaitHandle.WaitAny(handles) == 1)
                        throw new OperationCanceledException("read was canceled");
                }
                else
                    throw new ArgumentOutOfRangeException("timeout can not be negative");
            }

            List<byte> data = new List<byte>();

            byte[] packet = ReadPacketFromPacketBuffer();

            data.AddRange(packet);

            PacketReady.Reset();

            return data;
        }

        /// <summary>
        /// 
        /// </summary>
        public void Cancel()
        {
            CancelRead.Set();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <exception cref="Exception"></exception>
        private void OnDataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            SerialPort sp = sender as SerialPort;

            int toRead = sp.BytesToRead;

            if (toRead == 0)
                return;

            byte[] inData = new byte[toRead];

            sp.Read(inData, 0, inData.Length);

            foreach (byte b in inData)
            {
                if (b == SOM)
                    FoundStart = true;
                else if (b == EOM)
                {
                    for (int i = 0; i < FrameBuffer.Count; i++)
                    {
                        if (FrameBuffer[i] == ESC)
                        {
                            FrameBuffer.RemoveAt(i);

                            if (i == FrameBuffer.Count)
                                throw new Exception("escape character at end");

                            if (FrameBuffer[i] == ESC_PLACEHOLDER)
                                FrameBuffer[i] = ESC;
                            else if (FrameBuffer[i] == SOM_PLACEHOLDER)
                                FrameBuffer[i] = SOM;
                            else if (FrameBuffer[i] == EOM_PLACEHOLDER)
                                FrameBuffer[i] = EOM;
                            else
                                throw new Exception("invalid character after escape character");
                        }
                    }

                    List<byte> packet = new List<byte>();

                    packet.AddRange(FrameBuffer);

                    PacketBuffer.Add(packet);

                    FrameBuffer.Clear();
                }
                else
                {
                    if (FoundStart)
                        FrameBuffer.Add(b);
                }
            }

            if (PacketBuffer.Count > 0)
                PacketReady.Set();
        }
    }
}
