// SPDX-License-Identifier: MIT
/*
* KFDtool Container (EKC) Editor
* MIT Open Source. Use is subject to license terms.
* DO NOT ALTER OR REMOVE COPYRIGHT NOTICES OR THIS FILE HEADER.
*
*/

using System;
using System.Windows;

using KFDtool.P25.TransferConstructs;

namespace KFDEKC.Edit.Dialog
{
    /// <summary>
    /// Interaction logic for DliIpDeviceEdit.xaml
    /// </summary>
    public partial class DliIpDeviceEdit : Window
    {
        /// <summary>
        /// 
        /// </summary>
        /// <exception cref="Exception"></exception>
        public DliIpDeviceEdit()
        {
            InitializeComponent();

            // protocol

            if (Settings.SelectedDevice.DliIpDevice.Protocol == DliIpDevice.ProtocolOptions.UDP)
                PcbProtocol.SelectedItem = PcbiProtocolUdp;
            else
                throw new Exception("unknown DliIpProtocol setting");

            // hostname

            TbHostname.Text = Settings.SelectedDevice.DliIpDevice.Hostname;

            // port

            TbPort.Text = Settings.SelectedDevice.DliIpDevice.Port.ToString();

            // variant

            if (Settings.SelectedDevice.DliIpDevice.Variant == DliIpDevice.VariantOptions.Standard)
                PcbVariant.SelectedItem = PcbiVariantStandard;
            else if (Settings.SelectedDevice.DliIpDevice.Variant == DliIpDevice.VariantOptions.Motorola)
                PcbVariant.SelectedItem = PcbiVariantMotorola;
            else
                throw new Exception("unknown DliIpVariant setting");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <exception cref="Exception"></exception>
        private void Save_Button_Click(object sender, RoutedEventArgs e)
        {
            // protocol

            if (PcbProtocol.SelectedItem == PcbiProtocolUdp)
                Settings.SelectedDevice.DliIpDevice.Protocol = DliIpDevice.ProtocolOptions.UDP;
            else
                throw new Exception("unknown PcbProtocol selection");

            // hostname

            Settings.SelectedDevice.DliIpDevice.Hostname = TbHostname.Text;

            // port

            int port;

            if (int.TryParse(TbPort.Text, out port))
            {
                if (port >= 0 && port <= 65535)
                    Settings.SelectedDevice.DliIpDevice.Port = port;
                else
                {
                    MessageBox.Show("Valid port range is 0-65535", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
            }
            else
            {
                MessageBox.Show("Could not parse port", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // variant

            if (PcbVariant.SelectedItem == PcbiVariantStandard)
                Settings.SelectedDevice.DliIpDevice.Variant = DliIpDevice.VariantOptions.Standard;
            else if (PcbVariant.SelectedItem == PcbiVariantMotorola)
                Settings.SelectedDevice.DliIpDevice.Variant = DliIpDevice.VariantOptions.Motorola;
            else
                throw new Exception("unknown PcbVariant selection");

            this.Close();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Cancel_Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
