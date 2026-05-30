// SPDX-License-Identifier: MIT
/*
* KFDtool Container (EKC) Editor
* MIT Open Source. Use is subject to license terms.
* DO NOT ALTER OR REMOVE COPYRIGHT NOTICES OR THIS FILE HEADER.
*
*/

using KFDEKC.Edit.Dialog;
using KFDtool.P25.TransferConstructs;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace KFDEKC.Edit.Control
{
    /// <summary>
    /// Interaction logic for P25ViewKeysetInfo.xaml
    /// </summary>
    public partial class P25ViewKeysetInfo : UserControl
    {
        private Window parent;

        /// <summary>
        /// 
        /// </summary>
        public P25ViewKeysetInfo(Window parent)
        {
            InitializeComponent();

            this.parent = parent;

            parent.Cursor = Cursors.Wait;
            RetrieveKeysetInfo();
            parent.Cursor = Cursors.Arrow;
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
        private void RetrieveKeysetInfo()
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
                KeysetCount.Text = $"{keyset.Count} keyset(s)";
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Changeover_Click(object sender, RoutedEventArgs e)
        {
            parent.Cursor = Cursors.Wait;
            Window.GetWindow(this).Cursor = Cursors.Wait;

            changeOverStatus.Text = $"Changing Keyset from {txtKsIdOldDec.Text} to {txtKsIdNewDec.Text}...please, wait.";
            UserControlDialog.RefreshUi();

            RspChangeoverInfo changeoverResult = new RspChangeoverInfo();
            try
            {
                changeoverResult = Interact.ActivateKeyset(Settings.SelectedDevice, int.Parse(txtKsIdOldDec.Text), int.Parse(txtKsIdNewDec.Text));
                changeOverStatus.Text = $"Keyset {changeoverResult.KeysetIdActivated} activated, Keyset {changeoverResult.KeysetIdSuperseded} superseded.";
            }
            catch (Exception ex)
            {
                MessageBox.Show(string.Format("Error -- {0}", ex.Message), "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                changeOverStatus.Text = $"ERROR: Keyset changover failed! Keyset not actived!";
                parent.Cursor = Cursors.Arrow;
                Window.GetWindow(this).Cursor = Cursors.Arrow;
                return;
            }

            parent.Cursor = Cursors.Arrow;
            Window.GetWindow(this).Cursor = Cursors.Arrow;
        }
    }
}
