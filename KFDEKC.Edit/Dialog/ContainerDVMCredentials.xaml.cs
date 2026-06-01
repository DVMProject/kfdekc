// SPDX-License-Identifier: MIT
/*
* KFDtool Container (EKC) Editor
* MIT Open Source. Use is subject to license terms.
* DO NOT ALTER OR REMOVE COPYRIGHT NOTICES OR THIS FILE HEADER.
*
*/

using System.Net;
using System.Windows;

namespace KFDEKC.Edit.Dialog
{
    /// <summary>
    /// Interaction logic for ContainerDVMCredentials.xaml
    /// </summary>
    public partial class ContainerDVMCredentials : Window
    {
        /// <summary>
        /// 
        /// </summary>
        public bool DataSet { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string DVMFNEIP { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public uint DVMFNEPeerID { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string DVMFNEPeerPassword { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string DVMFNERemoteAccessPassword { get; set; }

        /*
        ** Methods
        */

        /// <summary>
        /// 
        /// </summary>
        public ContainerDVMCredentials()
        {
            InitializeComponent();

            DataSet = false;

            DVMFNEIP = string.Empty;
            DVMFNEPeerID = 0;
            DVMFNEPeerPassword = string.Empty;
            DVMFNERemoteAccessPassword = string.Empty;

            txtDVMFNEIP.Focus();

            if (Settings.LastDVMFNEHostname != string.Empty)
            {
                txtDVMFNEIP.Text = $"{Settings.LastDVMFNEHostname}:{Settings.LastDVMFNEPort}";
                txtDVMPeerID.Focus();
            }

            if (Settings.LastDVMFNEPeerId > 0)
            {
                txtDVMPeerID.Text = Settings.LastDVMFNEPeerId.ToString();
                txtDVMPeerPassword.Focus();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Submit_Click(object sender, RoutedEventArgs e)
        {
            if (txtDVMFNEIP.Text.Length == 0)
            {
                MessageBox.Show("DVM FNE IP and port is required", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            string rawUri = txtDVMFNEIP.Text;
            if (!rawUri.Contains("://"))
                rawUri = $"udp://{rawUri}";

            string resultFNEIP = string.Empty;
            if (Uri.TryCreate(rawUri, UriKind.Absolute, out Uri uri))
            {
                try
                {
                    string host = uri.Host;
                    try
                    {
                        IPAddress[] addresses = Dns.GetHostAddresses(host);
                        if (addresses.Length > 0)
                            host = addresses[0].ToString();
                    }
                    catch (Exception)
                    {
                        /* stub */
                    }

                    int port = uri.Port;

                    IPEndPoint.Parse($"{host}:{port}");
                    Settings.LastDVMFNEHostname = host;
                    Settings.LastDVMFNEPort = port;
                    resultFNEIP = host + ":" + port;
                }
                catch (FormatException)
                {
                    MessageBox.Show("DVM FNE IP and port must be a properly formatted as: IP:PORT", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
            }
            else
            {
                MessageBox.Show("DVM FNE IP and port must be a properly formatted as: IP:PORT", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (resultFNEIP.Length == 0)
            {
                MessageBox.Show("DVM FNE IP and port must be a properly formatted as: IP:PORT", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (txtDVMPeerID.Text.Length == 0)
            {
                MessageBox.Show("DVM FNE peer ID is required", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            uint peerId = 0;
            if (!uint.TryParse(txtDVMPeerID.Text, out peerId))
            {
                MessageBox.Show("DVM FNE peer ID must be a number", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            Settings.LastDVMFNEPeerId = peerId;

            if (txtDVMPeerPassword.Password.Length == 0)
            {
                MessageBox.Show("DVM FNE peer password is required", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (txtDVMRemoteAccessPassword.Password.Length == 0)
            {
                MessageBox.Show("DVM FNE remote access password is required", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            DataSet = true;
            DVMFNEIP = resultFNEIP;
            DVMFNEPeerID = peerId;
            DVMFNEPeerPassword = txtDVMPeerPassword.Password;
            DVMFNERemoteAccessPassword = txtDVMRemoteAccessPassword.Password;

            Close();
        }
    }
}
