// SPDX-License-Identifier: MIT
/*
* KFDtool Container (EKC) Editor
* MIT Open Source. Use is subject to license terms.
* DO NOT ALTER OR REMOVE COPYRIGHT NOTICES OR THIS FILE HEADER.
*
*/

using KFDEKC.Edit.Dialog;
using KFDEKC.P25;
using KFDEKC.P25.Generator;
using KFDEKC.Shared;
using KFDtool.P25;
using KFDtool.P25.TransferConstructs;
using Microsoft.Win32;
using System;
using System.Globalization;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace KFDEKC.Edit.Control
{
    /// <summary>
    /// Interaction logic for P25SingleKeyload.xaml
    /// </summary>
    public partial class P25SingleKeyload : UserControl
    {
        private Window parent;

        /// <summary>
        /// 
        /// </summary>
        private bool IsKek { get; set; }

        /*
        ** Methods
        */

        /// <summary>
        /// 
        /// </summary>
        public P25SingleKeyload(Window parent)
        {
            InitializeComponent();

            this.parent = parent;

            IsKek = false;

            cbActiveKeyset.IsChecked = true; // check here to trigger the cb/txt logic on load
            cboType.SelectedIndex = 0; // set to the first item here to trigger the cbo/lbl logic on load
            cboAlgo.SelectedIndex = 0; // set to the first item here to trigger the cbo/txt logic on load
            cbHide.IsChecked = true; // check here to trigger the cb/txt logic on load
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
        private void KeyIdDec_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (txtKeyIdDec.IsFocused)
            {
                int num;
                if (int.TryParse(txtKeyIdDec.Text, out num))
                    txtKeyIdHex.Text = string.Format("{0:X}", num);
                else
                    txtKeyIdHex.Text = string.Empty;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void KeyIdHex_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (txtKeyIdHex.IsFocused)
            {
                int num;
                if (int.TryParse(txtKeyIdHex.Text, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out num))
                    txtKeyIdDec.Text = num.ToString();
                else
                    txtKeyIdDec.Text = string.Empty;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AlgoDec_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (txtAlgoDec.IsFocused)
            {
                int num;
                if (int.TryParse(txtAlgoDec.Text, out num))
                    txtAlgoHex.Text = string.Format("{0:X}", num);
                else
                    txtAlgoHex.Text = string.Empty;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AlgoHex_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (txtAlgoHex.IsFocused)
                UpdateAlgoHexToDec();
        }

        /// <summary>
        /// 
        /// </summary>
        private void UpdateAlgoHexToDec()
        {
            int num;
            if (int.TryParse(txtAlgoHex.Text, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out num))
                txtAlgoDec.Text = num.ToString();
            else
                txtAlgoDec.Text = string.Empty;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnAlgoChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cboAlgo.SelectedItem != null)
            {
                string name = ((ComboBoxItem)cboAlgo.SelectedItem).Name as string;

                if (name == "AES256")
                {
                    txtAlgoHex.Text = "84";
                    UpdateAlgoHexToDec();
                    txtAlgoDec.IsEnabled = false;
                    txtAlgoHex.IsEnabled = false;
                }
                else if (name == "DESOFB")
                {
                    txtAlgoHex.Text = "81";
                    UpdateAlgoHexToDec();
                    txtAlgoDec.IsEnabled = false;
                    txtAlgoHex.IsEnabled = false;
                }
                else if (name == "DESXL")
                {
                    txtAlgoHex.Text = "9F";
                    UpdateAlgoHexToDec();
                    txtAlgoDec.IsEnabled = false;
                    txtAlgoHex.IsEnabled = false;
                }
                else if (name == "ADP")
                {
                    txtAlgoHex.Text = "AA";
                    UpdateAlgoHexToDec();
                    txtAlgoDec.IsEnabled = false;
                    txtAlgoHex.IsEnabled = false;
                }
                else
                {
                    txtAlgoDec.Text = string.Empty;
                    txtAlgoHex.Text = string.Empty;
                    txtAlgoDec.IsEnabled = true;
                    txtAlgoHex.IsEnabled = true;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Generate_Button_Click(object sender, RoutedEventArgs e)
        {
            int algId = 0;

            try
            {
                algId = Convert.ToInt32(txtAlgoHex.Text, 16);
            }
            catch (Exception)
            {
                MessageBox.Show("Error Parsing Algorithm ID", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (!FieldValidator.IsValidAlgorithmId(algId))
            {
                MessageBox.Show("Algorithm ID invalid - valid range 0 to 255 (dec), 0x00 to 0xFF (hex)", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            List<byte> key = new List<byte>();

            if (algId == (byte)AlgorithmId.AES256)
                key = KeyGenerator.GenerateVarKey(32);
            else if (algId == (byte)AlgorithmId.DESOFB || algId == (byte)AlgorithmId.DESXL)
                key = KeyGenerator.GenerateSingleDesKey();
            else if (algId == (byte)AlgorithmId.ADP)
                key = KeyGenerator.GenerateVarKey(5);
            else
            {
                MessageBox.Show(string.Format("No key generator exists for algorithm ID 0x{0:X2}", algId), "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            SetKey(BitConverter.ToString(key.ToArray()).Replace("-", string.Empty));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnHideChecked(object sender, RoutedEventArgs e)
        {
            txtKeyHidden.Password = txtKeyVisible.Text;
            txtKeyVisible.Text = string.Empty;
            txtKeyVisible.Visibility = Visibility.Hidden;
            txtKeyHidden.Visibility = Visibility.Visible;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnHideUnchecked(object sender, RoutedEventArgs e)
        {
            txtKeyVisible.Text = txtKeyHidden.Password;
            txtKeyHidden.Password = null;
            txtKeyVisible.Visibility = Visibility.Visible;
            txtKeyHidden.Visibility = Visibility.Hidden;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private string GetKey()
        {
            if (cbHide.IsChecked == true)
                return txtKeyHidden.Password;
            else
                return txtKeyVisible.Text;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="str"></param>
        private void SetKey(string str)
        {
            if (cbHide.IsChecked == true)
                txtKeyHidden.Password = str;
            else
                txtKeyVisible.Text = str;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Load_Button_Click(object sender, RoutedEventArgs e)
        {
            int keysetId = 0, sln = 0, keyId = 0, algId = 0;
            List<byte> key = new List<byte>();

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

            try
            {
                keyId = Convert.ToInt32(txtKeyIdHex.Text, 16);
            }
            catch (Exception)
            {
                MessageBox.Show("Error Parsing Key ID", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            try
            {
                algId = Convert.ToInt32(txtAlgoHex.Text, 16);
            }
            catch (Exception)
            {
                MessageBox.Show("Error Parsing Algorithm ID", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            try
            {
                key = Utility.ByteStringToByteList(GetKey());
            }
            catch (Exception)
            {
                MessageBox.Show("Error Parsing Key", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            Tuple<ValidateResult, string> validateResult = FieldValidator.KeyloadValidate(keysetId, sln, IsKek, keyId, algId, key);
            if (validateResult.Item1 == ValidateResult.Warning)
            {
                if (MessageBox.Show(string.Format("{1}{0}{0}Continue?", Environment.NewLine, validateResult.Item2), "Warning", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.No)
                    return;
            }
            else if (validateResult.Item1 == ValidateResult.Error)
            {
                MessageBox.Show(validateResult.Item2, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            parent.Cursor = Cursors.Wait;
            Window.GetWindow(this).Cursor = Cursors.Wait;

            loadStatus.Text = $"Loading {txtSlnHex.Text} / {txtKeyIdHex.Text} key into device...please, wait.";
            this.IsEnabled = false;
            UserControlDialog.RefreshUi();

            List<CmdKeyItem> keys = new List<CmdKeyItem>();

            CmdKeyItem keyItem = new CmdKeyItem();

            keyItem.UseActiveKeyset = useActiveKeyset;
            keyItem.KeysetId = keysetId;
            keyItem.Sln = sln;
            keyItem.IsKek = IsKek;
            keyItem.KeyId = keyId;
            keyItem.AlgorithmId = algId;
            keyItem.Key = key;

            keys.Add(keyItem);

            try
            {
                Interact.Keyload(Settings.SelectedDevice, keys);
            }
            catch (Exception ex)
            {
                MessageBox.Show(string.Format("Error -- {0}", ex.Message), "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                loadStatus.Text = "ERROR: Key was not loaded!";
                this.IsEnabled = true;
                parent.Cursor = Cursors.Arrow;
                Window.GetWindow(this).Cursor = Cursors.Arrow;
                return;
            }

            this.IsEnabled = true;

            loadStatus.Text = "Key loaded successfully.";

            parent.Cursor = Cursors.Arrow;
            Window.GetWindow(this).Cursor = Cursors.Arrow;
        }
    }
}
