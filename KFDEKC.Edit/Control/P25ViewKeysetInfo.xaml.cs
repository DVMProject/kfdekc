// SPDX-License-Identifier: MIT
/*
* KFDtool Container (EKC) Editor
* MIT Open Source. Use is subject to license terms.
* DO NOT ALTER OR REMOVE COPYRIGHT NOTICES OR THIS FILE HEADER.
*
*/

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;

using KFDtool.P25.TransferConstructs;

namespace KFDEKC.Edit.Control
{
    /// <summary>
    /// Interaction logic for P25ViewKeysetInfo.xaml
    /// </summary>
    public partial class P25ViewKeysetInfo : UserControl
    {
        /// <summary>
        /// 
        /// </summary>
        public P25ViewKeysetInfo()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ksIdOldDec_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (txtKsIdOldDec.IsFocused)
            {
                int num;
                if (int.TryParse(txtKsIdOldDec.Text, out num))
                    txtKsIdOldHex.Text = string.Format("{0:X}", num);
                else
                    txtKsIdOldHex.Text = string.Empty;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ksIdOldHex_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (txtKsIdOldHex.IsFocused)
            {
                int num;
                if (int.TryParse(txtKsIdOldHex.Text, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out num))
                    txtKsIdOldDec.Text = num.ToString();
                else
                    txtKsIdOldDec.Text = string.Empty;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ksIdNewDec_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (txtKsIdNewDec.IsFocused)
            {
                int num;
                if (int.TryParse(txtKsIdNewDec.Text, out num))
                    txtKsIdNewHex.Text = string.Format("{0:X}", num);
                else
                    txtKsIdNewHex.Text = string.Empty;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ksIdNewHex_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (txtKsIdNewHex.IsFocused)
            {
                int num;
                if (int.TryParse(txtKsIdNewHex.Text, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out num))
                    txtKsIdNewDec.Text = num.ToString();
                else
                    txtKsIdNewDec.Text = string.Empty;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void View_KeysetInfo_Click(object sender, RoutedEventArgs e)
        {
            KeysetItems.ItemsSource = null; // clear table

            List<RspKeysetInfo> keyset = null;

            try
            {
                keyset = Interact.ViewKeysetTaggingInfo(Settings.SelectedDevice);
            }
            catch (Exception ex)
            {
                MessageBox.Show(string.Format("Error -- {0}", ex.Message), "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (keyset != null)
            {
                KeysetItems.ItemsSource = keyset;
                KeysetItems.Items.SortDescriptions.Add(new SortDescription("KeysetId", ListSortDirection.Ascending));
                MessageBox.Show(string.Format("{0} keyset(s) returned", keyset.Count), "Information", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Changeover_Click(object sender, RoutedEventArgs e)
        {
            RspChangeoverInfo changeoverResult = new RspChangeoverInfo();
            try
            {
                changeoverResult = Interact.ActivateKeyset(Settings.SelectedDevice, int.Parse(txtKsIdOldDec.Text), int.Parse(txtKsIdNewDec.Text));
                MessageBox.Show("Keyset " + changeoverResult.KeysetIdActivated + " activated, Keyset " + changeoverResult.KeysetIdSuperseded + " superseded", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(string.Format("Error -- {0}", ex.Message), "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
        }
    }
}
