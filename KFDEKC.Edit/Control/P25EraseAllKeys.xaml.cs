// SPDX-License-Identifier: MIT
/*
* KFDtool Container (EKC) Editor
* MIT Open Source. Use is subject to license terms.
* DO NOT ALTER OR REMOVE COPYRIGHT NOTICES OR THIS FILE HEADER.
*
*/

using System;
using System.Windows;
using System.Windows.Controls;

using KFDtool.P25.TransferConstructs;

namespace KFDEKC.Edit.Control
{
    /// <summary>
    /// Interaction logic for P25EraseAllKeys.xaml
    /// </summary>
    public partial class P25EraseAllKeys : UserControl
    {
        /// <summary>
        /// 
        /// </summary>
        public P25EraseAllKeys()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Erase_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Interact.EraseAllKeys(Settings.SelectedDevice);
            }
            catch (Exception ex)
            {
                MessageBox.Show(string.Format("Error -- {0}", ex.Message), "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            MessageBox.Show("All Keys Erased Successfully", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}
