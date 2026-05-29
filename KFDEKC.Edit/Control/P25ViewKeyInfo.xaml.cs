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
using System.Windows;
using System.Windows.Controls;

using KFDtool.P25.TransferConstructs;

namespace KFDEKC.Edit.Control
{
    /// <summary>
    /// Interaction logic for P25ViewKeyInfo.xaml
    /// </summary>
    public partial class P25ViewKeyInfo : UserControl
    {
        /// <summary>
        /// 
        /// </summary>
        public P25ViewKeyInfo()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Update_Click(object sender, RoutedEventArgs e)
        {
            KeyItems.ItemsSource = null; // clear table

            List<RspKeyInfo> keys = null;

            try
            {
                keys = Interact.ViewKeyInfo(Settings.SelectedDevice);
            }
            catch (Exception ex)
            {
                MessageBox.Show(string.Format("Error -- {0}", ex.Message), "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (keys != null)
            {
                KeyItems.ItemsSource = keys;

                KeyItems.Items.SortDescriptions.Add(new SortDescription("KeysetId", ListSortDirection.Ascending));
                KeyItems.Items.SortDescriptions.Add(new SortDescription("Sln", ListSortDirection.Ascending));

                MessageBox.Show(string.Format("{0} key(s) returned", keys.Count), "Information", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
    }
}
