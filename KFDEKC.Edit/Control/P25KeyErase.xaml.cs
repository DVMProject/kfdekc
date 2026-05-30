// SPDX-License-Identifier: MIT
/*
* KFDtool Container (EKC) Editor
* MIT Open Source. Use is subject to license terms.
* DO NOT ALTER OR REMOVE COPYRIGHT NOTICES OR THIS FILE HEADER.
*
*/

using KFDEKC.Edit.Dialog;
using KFDEKC.P25;
using KFDtool.P25.TransferConstructs;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace KFDEKC.Edit.Control
{
    /// <summary>
    /// Interaction logic for P25KeyErase.xaml
    /// </summary>
    public partial class P25KeyErase : UserControl
    {
        private Window parent;
        private bool IsKek { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public P25KeyErase(Window parent)
        {
            InitializeComponent();

            this.parent = parent;

            IsKek = false;

            cbActiveKeyset.IsChecked = true; // check here to trigger the cb/txt logic on load
            cboType.SelectedIndex = 0; // set to the first item here to trigger the cbo/lbl logic on load
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void KeysetIdDec_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (txtKeysetIdDec.IsFocused)
            {
                int num;
                if (int.TryParse(txtKeysetIdDec.Text, out num))
                    txtKeysetIdHex.Text = string.Format("{0:X}", num);
                else
                    txtKeysetIdHex.Text = string.Empty;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void KeysetIdHex_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (txtKeysetIdHex.IsFocused)
            {
                int num;
                if (int.TryParse(txtKeysetIdHex.Text, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out num))
                    txtKeysetIdDec.Text = num.ToString();
                else
                    txtKeysetIdDec.Text = string.Empty;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnActiveKeysetChecked(object sender, RoutedEventArgs e)
        {
            txtKeysetIdDec.Text = string.Empty;
            txtKeysetIdHex.Text = string.Empty;
            txtKeysetIdDec.IsEnabled = false;
            txtKeysetIdHex.IsEnabled = false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnActiveKeysetUnchecked(object sender, RoutedEventArgs e)
        {
            txtKeysetIdDec.IsEnabled = true;
            txtKeysetIdHex.IsEnabled = true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SlnDec_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (txtSlnDec.IsFocused)
            {
                int num;
                if (int.TryParse(txtSlnDec.Text, out num))
                    txtSlnHex.Text = string.Format("{0:X}", num);
                else
                    txtSlnHex.Text = string.Empty;

                UpdateType();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SlnHex_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (txtSlnHex.IsFocused)
            {
                int num;
                if (int.TryParse(txtSlnHex.Text, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out num))
                    txtSlnDec.Text = num.ToString();
                else
                    txtSlnDec.Text = string.Empty;

                UpdateType();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void UpdateType()
        {
            if (cboType.SelectedItem != null)
            {
                string name = ((ComboBoxItem)cboType.SelectedItem).Name as string;

                if (name == "AUTO")
                {
                    int num;
                    if (int.TryParse(txtSlnHex.Text, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out num))
                    {
                        if (num >= 0 && num <= 61439)
                        {
                            lblType.Content = "TEK";
                            IsKek = false;
                        }
                        else if (num >= 61440 && num <= 65535)
                        {
                            lblType.Content = "KEK";
                            IsKek = true;
                        }
                        else
                        {
                            lblType.Content = "Auto";
                        }
                    }
                    else
                        lblType.Content = "Auto";
                }
                else if (name == "TEK")
                {
                    lblType.Content = "TEK";
                    IsKek = false;
                }
                else if (name == "KEK")
                {
                    lblType.Content = "KEK";
                    IsKek = true;
                }
                else
                {
                    // error
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnTypeChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateType();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Erase_Button_Click(object sender, RoutedEventArgs e)
        {
            int keysetId = 0;
            int sln = 0;

            bool useActiveKeyset = cbActiveKeyset.IsChecked == true;

            if (useActiveKeyset)
                keysetId = 2; // to pass validation, will not get used
            else
            {
                try
                {
                    keysetId = Convert.ToInt32(txtKeysetIdHex.Text, 16);
                }
                catch (Exception)
                {
                    MessageBox.Show("Error Parsing Keyset ID", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
            }

            try
            {
                sln = Convert.ToInt32(txtSlnHex.Text, 16);
            }
            catch (Exception)
            {
                MessageBox.Show("Error Parsing SLN", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            bool keysetValidateResult = FieldValidator.IsValidKeysetId(keysetId);

            if (!keysetValidateResult)
            {
                MessageBox.Show("Keyset ID invalid - valid range 1 to 255 (dec), 0x01 to 0xFF (hex)", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            bool slnValidateResult = FieldValidator.IsValidSln(sln);

            if (!slnValidateResult)
            {
                MessageBox.Show("SLN invalid - valid range 0 to 65535 (dec), 0x0000 to 0xFFFF (hex)", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            List<CmdKeyItem> keys = new List<CmdKeyItem>();

            CmdKeyItem keyItem = new CmdKeyItem();

            keyItem.UseActiveKeyset = useActiveKeyset;
            keyItem.KeysetId = keysetId;
            keyItem.Sln = sln;
            keyItem.IsKek = IsKek;

            keys.Add(keyItem);

            parent.Cursor = Cursors.Wait;
            Window.GetWindow(this).Cursor = Cursors.Wait;

            eraseStatus.Text = $"Erasing key {txtSlnHex.Text} from device...please, wait.";
            this.IsEnabled = false;
            UserControlDialog.RefreshUi();

            try
            {
                Interact.EraseKey(Settings.SelectedDevice, keys);
            }
            catch (Exception ex)
            {
                MessageBox.Show(string.Format("Error -- {0}", ex.Message), "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                eraseStatus.Text = "ERROR: Key was not erased!";
                this.IsEnabled = true;
                parent.Cursor = Cursors.Arrow;
                Window.GetWindow(this).Cursor = Cursors.Arrow;
                return;
            }

            this.IsEnabled = true;

            eraseStatus.Text = "Key erased successfully.";
            parent.Cursor = Cursors.Arrow;
            Window.GetWindow(this).Cursor = Cursors.Arrow;
        }
    }
}
