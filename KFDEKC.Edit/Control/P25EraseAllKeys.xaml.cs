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
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace KFDEKC.Edit.Control
{
    /// <summary>
    /// Interaction logic for P25EraseAllKeys.xaml
    /// </summary>
    public partial class P25EraseAllKeys : UserControl
    {
        private Window parent;

        /// <summary>
        /// 
        /// </summary>
        public P25EraseAllKeys(Window parent)
        {
            InitializeComponent();

            this.parent = parent;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Erase_Click(object sender, RoutedEventArgs e)
        {
            parent.Cursor = Cursors.Wait;
            Window.GetWindow(this).Cursor = Cursors.Wait;

            this.IsEnabled = false;
            UserControlDialog.RefreshUi();

            try
            {
                Interact.EraseAllKeys(Settings.SelectedDevice);
            }
            catch (Exception ex)
            {
                MessageBox.Show(string.Format("Error -- {0}", ex.Message), "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                this.IsEnabled = true;
                parent.Cursor = Cursors.Arrow;
                Window.GetWindow(this).Cursor = Cursors.Arrow;
                return;
            }

            MessageBox.Show("All Keys Erased Successfully", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
            parent.Cursor = Cursors.Arrow;
            Window.GetWindow(this).Cursor = Cursors.Arrow;
            Window.GetWindow(this).Close();
        }
    }
}
