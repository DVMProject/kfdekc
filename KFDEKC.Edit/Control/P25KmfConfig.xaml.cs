// SPDX-License-Identifier: MIT
/*
* KFDtool Container (EKC) Editor
* MIT Open Source. Use is subject to license terms.
* DO NOT ALTER OR REMOVE COPYRIGHT NOTICES OR THIS FILE HEADER.
*
*/

using KFDtool.P25.TransferConstructs;
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace KFDEKC.Edit.Control
{
    /// <summary>
    /// Interaction logic for P25KmfConfig.xaml
    /// </summary>
    public partial class P25KmfConfig : UserControl
    {
        private Window parent;

        /// <summary>
        /// 
        /// </summary>
        public P25KmfConfig(Window parent)
        {
            InitializeComponent();

            this.parent = parent;

            View_KmfRsi_Click(this, new RoutedEventArgs());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void KmfRsiDec_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (txtKmfRsiDec.IsFocused)
            {
                int num;
                if (int.TryParse(txtKmfRsiDec.Text, out num))
                    txtKmfRsiHex.Text = string.Format("{0:X}", num);
                else
                    txtKmfRsiHex.Text = string.Empty;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void KmfRsiHex_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (txtKmfRsiHex.IsFocused)
            {
                int num;
                if (int.TryParse(txtKmfRsiHex.Text, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out num))
                    txtKmfRsiDec.Text = num.ToString();
                else
                    txtKmfRsiDec.Text = string.Empty;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MnpDec_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (txtMnpDec.IsFocused)
            {
                int num;
                if (int.TryParse(txtMnpDec.Text, out num))
                    txtMnpHex.Text = string.Format("{0:X}", num);
                else
                    txtMnpHex.Text = string.Empty;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MnpHex_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (txtMnpHex.IsFocused)
            {
                int num;
                if (int.TryParse(txtMnpHex.Text, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out num))
                    txtMnpDec.Text = num.ToString();
                else
                    txtMnpDec.Text = string.Empty;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void View_MNP_Click(object sender, RoutedEventArgs e)
        {
            int mnp = -1;

            parent.Cursor = Cursors.Wait;

            try
            {
                mnp = Interact.ViewMnp(Settings.SelectedDevice);
            }
            catch (Exception ex)
            {
                MessageBox.Show(string.Format("Error -- {0}", ex.Message), "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                parent.Cursor = Cursors.Arrow;
                return;
            }

            MessageBox.Show("Message Number Period: " + mnp, "Information", MessageBoxButton.OK, MessageBoxImage.Information);
            parent.Cursor = Cursors.Arrow;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void View_KmfRsi_Click(object sender, RoutedEventArgs e)
        {
            parent.Cursor = Cursors.Wait;

            // First get KMF RSI
            try
            {
                int rsi = new int();
                rsi = Interact.ViewKmfRsi(Settings.SelectedDevice);

                txtKmfRsiDec.Text = rsi.ToString();
                txtKmfRsiHex.Text = string.Format("{0:X}", rsi);

                // Next get MNP for KMF
                int mnp = -1;
                try
                {
                    mnp = Interact.ViewMnp(Settings.SelectedDevice);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(string.Format("Error -- {0}", ex.Message), "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    parent.Cursor = Cursors.Arrow;
                    return;
                }

                txtMnpDec.Text = mnp.ToString();
                txtMnpHex.Text = string.Format("{0:X}", mnp);
            }
            catch (Exception ex)
            {
                MessageBox.Show(string.Format("Error -- {0}", ex.Message), "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                parent.Cursor = Cursors.Arrow;
                return;
            }

            parent.Cursor = Cursors.Arrow;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Load_Config_Click(object sender, RoutedEventArgs e)
        {
            int kmfRsi = 0;
            int mnp = 0;

            try
            {
                kmfRsi = Convert.ToInt32(txtKmfRsiHex.Text, 16);
            }
            catch (Exception)
            {
                MessageBox.Show("Error Parsing KMF RSI", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            try
            {
                mnp = Convert.ToInt32(txtMnpHex.Text, 16);
            }
            catch (Exception)
            {
                MessageBox.Show("Error Parsing MNP", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Ensure valid KMF RSI value
            if ((kmfRsi > 9999999) || (kmfRsi < 1))
            {
                MessageBox.Show("Invalid KMF RSI - must be between 1 and 9,999,999", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Ensure valid MNP value
            if ((mnp > 65535) || (mnp < 0))
            {
                MessageBox.Show("Invalid MNP - must be between 0 and 65,535", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            parent.Cursor = Cursors.Wait;

            try
            {
                RspRsiInfo temp = new RspRsiInfo();
                temp = Interact.LoadConfig(Settings.SelectedDevice, kmfRsi, mnp);
                MessageBox.Show("Config Loaded Successfully - RSI: " + temp.RSI + ", Message Number: " + temp.MN + ", Status: " + temp.Status, "Information", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(string.Format("Error -- {0}", ex.Message), "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                parent.Cursor = Cursors.Arrow;
                return;
            }

            parent.Cursor = Cursors.Arrow;
        }
    }
}
