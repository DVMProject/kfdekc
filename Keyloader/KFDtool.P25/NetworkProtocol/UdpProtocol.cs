// SPDX-License-Identifier: MIT
/*
* KFDtool Container (EKC) Editor
* MIT Open Source. Use is subject to license terms.
* DO NOT ALTER OR REMOVE COPYRIGHT NOTICES OR THIS FILE HEADER.
*
*/

using System.Net;
using System.Net.Sockets;

namespace KFDtool.P25.NetworkProtocol
{
    /// <summary>
    /// 
    /// </summary>
    public class UdpProtocol
    {
        private string IpAddress;

        private int PortNumber;

        private int Timeout;

        /*
        ** Methods
        */

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ipAddress"></param>
        /// <param name="portNumber"></param>
        /// <param name="timeout"></param>
        public UdpProtocol(string ipAddress, int portNumber, int timeout)
        {
            IpAddress = ipAddress;
            PortNumber = portNumber;
            Timeout = timeout;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="toRadio"></param>
        /// <returns></returns>
        public byte[] TxRx(byte[] toRadio)
        {
            using (UdpClient udpClient = new UdpClient(IpAddress, PortNumber))
            {
                udpClient.Client.SendTimeout = Timeout;
                udpClient.Client.ReceiveTimeout = Timeout;
                udpClient.Send(toRadio, toRadio.Length);
                IPEndPoint remoteEndPoint = new IPEndPoint(IPAddress.Any, 0);
                byte[] fromRadio = udpClient.Receive(ref remoteEndPoint);
                return fromRadio;
            }
        }
    }
}
