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

            try
            {
                IPEndPoint.Parse(txtDVMFNEIP.Text);
            }
            catch (FormatException)
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
            DVMFNEIP = txtDVMFNEIP.Text;
            DVMFNEPeerID = peerId;
            DVMFNEPeerPassword = txtDVMPeerPassword.Password;
            DVMFNERemoteAccessPassword = txtDVMRemoteAccessPassword.Password;

            Close();
        }
    }
}
