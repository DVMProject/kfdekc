// SPDX-License-Identifier: MIT
/*
* KFDtool Container (EKC) Editor
* MIT Open Source. Use is subject to license terms.
* DO NOT ALTER OR REMOVE COPYRIGHT NOTICES OR THIS FILE HEADER.
*
*/

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;

using KFDEKC.Container;
using KFDEKC.Container.FileStructure.EKC;
using KFDEKC.P25;
using KFDEKC.P25.Generator;
using KFDEKC.Shared;

namespace KFDEKC.Edit.Control
{
    /// <summary>
    /// Interaction logic for ContainerEditRSIKeyControl.xaml
    /// </summary>
    public partial class ContainerEditRSIKeyControl : UserControl
    {
        private RSIKeyItem localKey;
        private bool isKek;
        private bool isUKEK;

        /*
        ** Methods
        */

        /// <summary>
        /// 
        /// </summary>
        /// <param name="keyItem"></param>
        /// <exception cref="Exception"></exception>
        public ContainerEditRSIKeyControl(RSIKeyItem keyItem, bool isUKEK = false)
        {
            InitializeComponent();

            isKek = true;
            this.isUKEK = isUKEK;

            if (isUKEK)
            {
                txtSlnDec.IsEnabled = false;
                txtSlnHex.IsEnabled = false;
                txtKeyIdDec.IsEnabled = false;
                txtKeyIdHex.IsEnabled = false;
            }

            localKey = keyItem;

            txtRSI.Text = keyItem.RsiId.ToString();

            keyItem.ActiveKeyset = true;
            cbActiveKeyset.IsChecked = true;
            keyItem.KeysetId = 255;
            txtKeysetIdDec.Text = keyItem.KeysetId.ToString();
            UpdateKeysetIdDec();
            keyItem.KeyId = 62440;
            keyItem.Sln = 61440;

            keyItem.KeyTypeAuto = false;
            keyItem.KeyTypeTek = false;
            keyItem.KeyTypeKek = true;

            cboType.SelectedIndex = 2;

            txtSlnDec.Text = keyItem.Sln.ToString();
            UpdateSlnDec();

            txtKeyIdDec.Text = keyItem.KeyId.ToString();
            UpdateKeyIdDec();

            txtAlgoDec.Text = keyItem.AlgorithmId.ToString();
            UpdateAlgoDec();

            cboAlgo.SelectedIndex = 0;

            cbHide.IsChecked = true;
            txtKeyHidden.Password = keyItem.Key;
        }

        /// <summary>
        /// 
        /// </summary>
        private void UpdateKeysetIdDec()
        {
            int num;
            if (int.TryParse(txtKeysetIdDec.Text, out num))
                txtKeysetIdHex.Text = string.Format("{0:X}", num);
            else
                txtKeysetIdHex.Text = string.Empty;
        }

        /// <summary>
        /// 
        /// </summary>
        private void UpdateSlnDec()
        {
            int num;
            if (int.TryParse(txtSlnDec.Text, out num))
                txtSlnHex.Text = string.Format("{0:X}", num);
            else
                txtSlnHex.Text = string.Empty;

            UpdateType();
        }

        /// <summary>
        /// 
        /// </summary>
        private void UpdateSlnHex()
        {
            int num;
            if (int.TryParse(txtSlnHex.Text, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out num))
                txtSlnDec.Text = num.ToString();
            else
                txtSlnDec.Text = string.Empty;

            UpdateType();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SlnDec_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (txtSlnDec.IsFocused)
                UpdateSlnDec();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SlnHex_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (txtSlnHex.IsFocused)
                UpdateSlnHex();
        }

        /// <summary>
        /// 
        /// </summary>
        private void UpdateType()
        {
            if (cboType.SelectedItem != null)
            {
                string name = ((ComboBoxItem)cboType.SelectedItem).Name as string;
                lblType.Content = "KEK";
                isKek = true;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void UpdateKeyIdDec()
        {
            int num;
            if (int.TryParse(txtKeyIdDec.Text, out num))
                txtKeyIdHex.Text = string.Format("{0:X}", num);
            else
                txtKeyIdHex.Text = string.Empty;
        }

        /// <summary>
        /// 
        /// </summary>
        private void UpdateKeyIdHex()
        {
            int num;
            if (int.TryParse(txtKeyIdHex.Text, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out num))
                txtKeyIdDec.Text = num.ToString();
            else
                txtKeyIdDec.Text = string.Empty;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void KeyIdDec_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (txtKeyIdDec.IsFocused)
                UpdateKeyIdDec();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void KeyIdHex_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (txtKeyIdHex.IsFocused)
                UpdateKeyIdHex();
        }

        /// <summary>
        /// 
        /// </summary>
        private void UpdateAlgoDec()
        {
            int num;
            if (int.TryParse(txtAlgoDec.Text, out num))
                txtAlgoHex.Text = string.Format("{0:X}", num);
            else
                txtAlgoHex.Text = string.Empty;
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
            if (isUKEK)
                key = KeyGenerator.GenerateVarKey(32);
            else
                key = KeyGenerator.GenerateVarKey(16);

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
        /// <exception cref="Exception"></exception>
        private void Save_Button_Click(object sender, RoutedEventArgs e)
        {
            int keysetId, sln, keyId, algId, rsiId;
            List<byte> key;

            bool useActiveKeyset = cbActiveKeyset.IsChecked == true;

            if (useActiveKeyset)
                keysetId = 1; // to pass validation, will not get used
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

            Tuple<ValidateResult, string> validateResult = FieldValidator.KeyloadValidate(keysetId, sln, isKek, keyId, algId, key);
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

            if (txtRSI.Text.Length == 0)
            {
                MessageBox.Show("Key RSI required", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (txtRSI.Text != localKey.Name)
            {
                foreach (KeyItem keyItem in Settings.ContainerInner.Keys)
                {
                    if (txtRSI.Text == keyItem.Name)
                    {
                        MessageBox.Show("Key RSI must be unique", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }
                }
            }

            try
            {
                rsiId = Convert.ToInt32(txtRSI.Text);
            }
            catch (Exception)
            {
                MessageBox.Show("Error Parsing RSI ID", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            localKey.Name = txtRSI.Text;
            localKey.RsiId = rsiId;
            localKey.ActiveKeyset = true;
            localKey.KeysetId = 255;
            localKey.Sln = sln;

            localKey.KeyTypeAuto = false;
            localKey.KeyTypeTek = false;
            localKey.KeyTypeKek = true;

            localKey.KeyId = keyId;
            localKey.AlgorithmId = algId;
            localKey.Key = BitConverter.ToString(key.ToArray()).Replace("-", string.Empty);
        }
    }
}
