// SPDX-License-Identifier: MIT
/*
* KFDtool Container (EKC) Editor
* MIT Open Source. Use is subject to license terms.
* DO NOT ALTER OR REMOVE COPYRIGHT NOTICES OR THIS FILE HEADER.
*
*/

using Microsoft.Win32;
using System;
using System.IO;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Threading.Tasks;

using KFDEKC.Container.FileStructure.EKC;
using KFDEKC.Shared;

using KFDtool.P25;
using KFDtool.P25.TransferConstructs;
using System.ComponentModel;
using System.Windows.Data;

namespace KFDEKC.Edit.Control
{
    /// <summary>
    /// Interaction logic for P25MultipleKeyload.xaml
    /// </summary>
    public partial class P25MultipleKeyload : UserControl
    {
        private List<int> Keys;
        private List<int> Groups;

        private Dictionary<int, string> KeysAvailable;
        private Dictionary<int, string> KeysSelected;
        private Dictionary<int, string> KeksAvailable;

        private Dictionary<int, string> GroupsAvailable;
        private Dictionary<int, string> GroupsSelected;

        /// <summary>
        /// 
        /// </summary>
        public P25MultipleKeyload()
        {
            InitializeComponent();

            Keys = new List<int>();
            Groups = new List<int>();

            KeysAvailable = new Dictionary<int, string>();
            KeysSelected = new Dictionary<int, string>();
            KeksAvailable = new Dictionary<int, string>();

            GroupsAvailable = new Dictionary<int, string>();
            GroupsSelected = new Dictionary<int, string>();

            lbKeysAvailable.ItemsSource = KeysAvailable;
            lbKeysSelected.ItemsSource = KeysSelected;

            dropKeksAvailable.ItemsSource = KeksAvailable;

            lbGroupsAvailable.ItemsSource = GroupsAvailable;
            lbGroupsSelected.ItemsSource = GroupsSelected;

            UpdateKeysColumns();
            UpdateKeksDropdown();
            UpdateGroupsColumns();
        }

        /// <summary>
        /// 
        /// </summary>
        private void UpdateKeysColumns()
        {
            KeysAvailable.Clear();
            foreach (KeyItem keyItem in Settings.ContainerInner.Keys)
                KeysAvailable.Add(keyItem.Id, keyItem.Name);

            KeysSelected.Clear();
            foreach (int key in Keys)
                KeysSelected.Add(key, KeysAvailable[key]);

            foreach (KeyValuePair<int, string> selected in KeysSelected)
                KeysAvailable.Remove(selected.Key);

            ICollectionView keysAvailableCollection = CollectionViewSource.GetDefaultView(lbKeysAvailable.ItemsSource);
            keysAvailableCollection.Refresh();
            ICollectionView keysSelectedCollection = CollectionViewSource.GetDefaultView(lbKeysSelected.ItemsSource);
            keysSelectedCollection.Refresh();
        }

        /// <summary>
        /// 
        /// </summary>
        private void UpdateKeksDropdown()
        {
            KeksAvailable.Clear();

            KeksAvailable.Add(-1, "Clear Keyload");
            dropKeksAvailable.SelectedIndex = 0;

            foreach (KeyItem keyItem in Settings.ContainerInner.Keys)
            {
                // AACA-A 6.1/Fig. 5
                if (keyItem.Sln >= 61440)
                    KeksAvailable.Add(keyItem.Id, keyItem.Name);
            }

            ICollectionView dropKeksAvailableCollection = CollectionViewSource.GetDefaultView(dropKeksAvailable.ItemsSource);
            dropKeksAvailableCollection.Refresh();
        }

        /// <summary>
        /// 
        /// </summary>
        private void UpdateGroupsColumns()
        {
            GroupsAvailable.Clear();
            foreach (Container.FileStructure.EKC.GroupItem groupItem in Settings.ContainerInner.Groups)
                GroupsAvailable.Add(groupItem.Id, groupItem.Name);

            GroupsSelected.Clear();
            foreach (int group in Groups)
                GroupsSelected.Add(group, GroupsAvailable[group]);

            foreach (KeyValuePair<int, string> selected in GroupsSelected)
                GroupsAvailable.Remove(selected.Key);

            ICollectionView groupsAvailableCollection = CollectionViewSource.GetDefaultView(lbGroupsAvailable.ItemsSource);
            groupsAvailableCollection.Refresh();
            ICollectionView groupsSelectedCollection = CollectionViewSource.GetDefaultView(lbGroupsSelected.ItemsSource);
            groupsSelectedCollection.Refresh();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Keys_Add_Click(object sender, RoutedEventArgs e)
        {
            if (lbKeysAvailable.SelectedItem != null)
            {
                foreach(var item in lbKeysAvailable.SelectedItems)
                {
                    int key = ((KeyValuePair<int, string>)item).Key;
                    Keys.Add(key);
                }
 
                UpdateKeysColumns();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Keys_Remove_Click(object sender, RoutedEventArgs e)
        {
            if (lbKeysSelected.SelectedItem != null)
            {
                foreach(var item in lbKeysSelected.SelectedItems)
                {
                    int key = ((KeyValuePair<int, string>)item).Key;
                    Keys.Remove(key);
                }
                
                UpdateKeysColumns();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Groups_Add_Click(object sender, RoutedEventArgs e)
        {
            if (lbGroupsAvailable.SelectedItem != null)
            {
                foreach (var item in lbGroupsAvailable.SelectedItems)
                {
                    int key = ((KeyValuePair<int, string>)item).Key;
                    Groups.Add(key);
                }

                UpdateGroupsColumns();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Groups_Remove_Click(object sender, RoutedEventArgs e)
        {
            if (lbGroupsSelected.SelectedItem != null)
            {
                foreach (var item in lbGroupsSelected.SelectedItems)
                {
                    int key = ((KeyValuePair<int, string>)item).Key;
                    Groups.Remove(key);
                }
                
                UpdateGroupsColumns();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <exception cref="Exception"></exception>
        private void Load()
        {
            List<int> combinedKeys = new List<int>();

            combinedKeys.AddRange(Keys);

            foreach (int groupItemId in Groups)
            {
                bool found = false;
                foreach (Container.FileStructure.EKC.GroupItem containerGroupItem in Settings.ContainerInner.Groups)
                {
                    if (groupItemId == containerGroupItem.Id)
                    {
                        found = true;
                        combinedKeys.AddRange(containerGroupItem.Keys);
                        break;
                    }
                }

                if (!found)
                    throw new Exception(string.Format("group with id {0} not found in container", groupItemId));
            }

            if (combinedKeys.Count == 0)
                throw new Exception("no keys/groups selected");

            List<CmdKeyItem> keys = new List<CmdKeyItem>();
            foreach (int keyId in combinedKeys)
            {
                bool found = false;

                foreach (KeyItem containerKeyItem in Settings.ContainerInner.Keys)
                {
                    if (keyId == containerKeyItem.Id)
                    {
                        found = true;

                        CmdKeyItem cmdKeyItem = new CmdKeyItem();

                        cmdKeyItem.UseActiveKeyset = containerKeyItem.ActiveKeyset;
                        cmdKeyItem.KeysetId = containerKeyItem.KeysetId;
                        cmdKeyItem.Sln = containerKeyItem.Sln;

                        if (containerKeyItem.KeyTypeAuto)
                        {
                            if (cmdKeyItem.Sln >= 0 && cmdKeyItem.Sln <= 61439)
                                cmdKeyItem.IsKek = false;
                            else if (cmdKeyItem.Sln >= 61440 && cmdKeyItem.Sln <= 65535)
                                cmdKeyItem.IsKek = true;
                            else
                                throw new Exception(string.Format("invalid Sln and KeyTypeAuto set: {0}", cmdKeyItem.Sln));
                        }
                        else if (containerKeyItem.KeyTypeTek)
                            cmdKeyItem.IsKek = false;
                        else if (containerKeyItem.KeyTypeKek)
                            cmdKeyItem.IsKek = true;
                        else
                            throw new Exception("KeyTypeAuto, KeyTypeTek, and KeyTypeKek all false");

                        cmdKeyItem.KeyId = containerKeyItem.KeyId;
                        cmdKeyItem.AlgorithmId = containerKeyItem.AlgorithmId;
                        cmdKeyItem.Key = Utility.ByteStringToByteList(containerKeyItem.Key);

                        keys.Add(cmdKeyItem);

                        break;
                    }
                }

                if (!found)
                    throw new Exception(string.Format("key with id {0} not found in container", keyId));
            }

            // if the combo box isn't set to Clear, then keyload with the kek
            int selKekContainerIndex = ((KeyValuePair<int, string>)dropKeksAvailable.Items[dropKeksAvailable.SelectedIndex]).Key;
            if (selKekContainerIndex > -1)
            {
                CmdKeyItem selectedKek = new CmdKeyItem();
                foreach (KeyItem containerKeyItem in Settings.ContainerInner.Keys)
                {
                    if (selKekContainerIndex == containerKeyItem.Id)
                    {
                        selectedKek.Sln = containerKeyItem.Sln;
                        selectedKek.IsKek = true;
                        selectedKek.KeyId = containerKeyItem.KeyId;
                        selectedKek.AlgorithmId = containerKeyItem.AlgorithmId;
                        selectedKek.Key = Utility.ByteStringToByteList(containerKeyItem.Key);
                    }
                }
                Interact.Keyload(Settings.SelectedDevice, keys, selectedKek);
            }
            else
                Interact.Keyload(Settings.SelectedDevice, keys);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Load_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Load();
            }
            catch (Exception ex)
            {
                MessageBox.Show(string.Format("Error -- {0}", ex.Message), "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            MessageBox.Show("Key(s) Loaded Successfully", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}
