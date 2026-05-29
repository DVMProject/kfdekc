// SPDX-License-Identifier: MIT
/*
* KFDtool Container (EKC) Editor
* MIT Open Source. Use is subject to license terms.
* DO NOT ALTER OR REMOVE COPYRIGHT NOTICES OR THIS FILE HEADER.
*
*/

using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

using KFDEKC.Container;
using KFDEKC.Container.FileStructure.EKC;

namespace KFDEKC.Edit.Control
{
    /// <summary>
    /// Interaction logic for ContainerEditGroupControl.xaml
    /// </summary>
    public partial class ContainerEditGroupControl : UserControl
    {
        private Container.FileStructure.EKC.GroupItem localGroup;
        private List<int> keys;
        private Dictionary<int, string> available;
        private Dictionary<int, string> selected;

        /*
        ** Methods
        */

        /// <summary>
        /// 
        /// </summary>
        /// <param name="groupItem"></param>
        public ContainerEditGroupControl(Container.FileStructure.EKC.GroupItem groupItem)
        {
            InitializeComponent();

            localGroup = groupItem;

            keys = new List<int>();
            keys.AddRange(groupItem.Keys);

            available = new Dictionary<int, string>();

            selected = new Dictionary<int, string>();

            txtName.Text = groupItem.Name;

            lbAvailable.ItemsSource = available;

            lbSelected.ItemsSource = selected;

            UpdateColumns();
        }

        /// <summary>
        /// 
        /// </summary>
        private void UpdateColumns()
        {
            available.Clear();
            foreach (KeyItem keyItem in Settings.ContainerInner.Keys)
                available.Add(keyItem.Id, keyItem.Name);

            selected.Clear();
            foreach (int key in keys)
                selected.Add(key, available[key]);

            foreach (KeyValuePair<int, string> selected in selected)
                available.Remove(selected.Key);

            lbAvailable.Items.Refresh();
            lbSelected.Items.Refresh();
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Add_Button_Click(object sender, RoutedEventArgs e)
        {
            if (lbAvailable.SelectedItem != null)
            {
                foreach( KeyValuePair<int, string> selectedKey in lbAvailable.SelectedItems)
                    keys.Add(selectedKey.Key);

                UpdateColumns();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Remove_Button_Click(object sender, RoutedEventArgs e)
        {
            if (lbSelected.SelectedItem != null)
            {
                foreach( KeyValuePair<int, string> selectedKey in lbSelected.SelectedItems)
                    keys.Remove(selectedKey.Key);

                UpdateColumns();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Save_Button_Click(object sender, RoutedEventArgs e)
        {
            if (txtName.Text.Length == 0)
            {
                MessageBox.Show("Group name required", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (txtName.Text != localGroup.Name)
            {
                foreach (Container.FileStructure.EKC.GroupItem groupItem in Settings.ContainerInner.Groups)
                {
                    if (txtName.Text == groupItem.Name)
                    {
                        MessageBox.Show("Group name must be unique", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }
                }
            }

            localGroup.Name = txtName.Text;
            localGroup.Keys.Clear();
            localGroup.Keys.AddRange(keys);
        }
    }
}
