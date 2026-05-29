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
using System.Windows.Input;

using KFDEKC.Container;
using KFDEKC.Container.FileStructure.EKC;
using KFDEKC.P25.Generator;

namespace KFDEKC.Edit.Control
{
    /// <summary>
    /// Interaction logic for ContainerEdit.xaml
    /// </summary>
    public partial class ContainerEdit : UserControl
    {
        private string OriginalContainer;
        public static RoutedCommand InsertCommand = new RoutedCommand();
        public static RoutedCommand DeleteCommand = new RoutedCommand();

        /*
        ** Methods
        */

        /// <summary>
        /// 
        /// </summary>
        public ContainerEdit()
        {
            InitializeComponent();

            InsertCommand.InputGestures.Add(new KeyGesture(Key.Insert));
            DeleteCommand.InputGestures.Add(new KeyGesture(Key.Delete));
        }

        /// <summary>
        /// 
        /// </summary>
        public void Refresh()
        {
            OriginalContainer = ContainerUtilities.SerializeInnerContainer(Settings.ContainerInner).OuterXml;
            keysListView.ItemsSource = Settings.ContainerInner.Keys;
            keysListView.SelectionChanged += KeysListView_SelectionChanged;
            groupsListView.ItemsSource = Settings.ContainerInner.Groups;
            groupsListView.SelectionChanged += GroupsListView_SelectionChanged;
            ukekListView.ItemsSource = Settings.ContainerInner.UKEKs;
            ukekListView.SelectionChanged += UkekListView_SelectionChanged;
            llaListView.ItemsSource = Settings.ContainerInner.LLAs;
            llaListView.SelectionChanged += LlaListView_SelectionChanged;
            UpdateTabSelection();
        }

        /// <summary>
        /// 
        /// </summary>
        public void Clear()
        {
            OriginalContainer = null;
            keysListView.ItemsSource = null;
            keysListView.SelectionChanged -= KeysListView_SelectionChanged;
            groupsListView.ItemsSource = null;
            groupsListView.SelectionChanged -= GroupsListView_SelectionChanged;
            ukekListView.ItemsSource = null;
            ukekListView.SelectionChanged -= UkekListView_SelectionChanged;
            llaListView.ItemsSource = null;
            llaListView.SelectionChanged -= LlaListView_SelectionChanged;
        }

        /// <summary>
        /// 
        /// </summary>
        private void UpdateTabSelection()
        {
            keysListView.SelectedItem = null;
            groupsListView.SelectedItem = null;
            ukekListView.SelectedItem = null;
            llaListView.SelectedItem = null;

            if (Settings.ContainerInner != null)
            {
                if (containerTabControl.SelectedItem == keysTabItem)
                    keysListView.SelectedItem = Settings.ContainerInner.Keys.Count > 0 ? Settings.ContainerInner.Keys[0] : null;
                else if (containerTabControl.SelectedItem == groupsTabItem)
                    groupsListView.SelectedItem = Settings.ContainerInner.Groups.Count > 0 ? Settings.ContainerInner.Groups[0] : null;
                else if (containerTabControl.SelectedItem == ukekTabItem)
                    ukekListView.SelectedItem = Settings.ContainerInner.UKEKs.Count > 0 ? Settings.ContainerInner.UKEKs[0] : null;
                else if (containerTabControl.SelectedItem == llaTabItem)
                    llaListView.SelectedItem = Settings.ContainerInner.LLAs.Count > 0 ? Settings.ContainerInner.LLAs[0] : null;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Tab_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.Source is TabControl)
                UpdateTabSelection();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void KeysListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (keysListView.SelectedItem != null)
            {
                ContainerEditKeyControl keyEdit = new ContainerEditKeyControl((KeyItem)keysListView.SelectedItem);
                ItemView.Content = keyEdit;
            }
            else
                ItemView.Content = null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GroupsListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (groupsListView.SelectedItem != null)
            {
                ContainerEditGroupControl keyEdit = new ContainerEditGroupControl((Container.FileStructure.EKC.GroupItem)groupsListView.SelectedItem);
                ItemView.Content = keyEdit;
            }
            else
                ItemView.Content = null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UkekListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ukekListView.SelectedItem != null)
            {
                ContainerEditRSIKeyControl keyEdit = new ContainerEditRSIKeyControl((Container.FileStructure.EKC.RSIKeyItem)ukekListView.SelectedItem, true);
                ItemView.Content = keyEdit;
            }
            else
                ItemView.Content = null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LlaListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (llaListView.SelectedItem != null)
            {
                ContainerEditRSIKeyControl keyEdit = new ContainerEditRSIKeyControl((Container.FileStructure.EKC.RSIKeyItem)llaListView.SelectedItem, false);
                ItemView.Content = keyEdit;
            }
            else
                ItemView.Content = null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void New_Click(object sender, RoutedEventArgs e)
        {
            if (containerTabControl.SelectedItem == keysTabItem)
            {
                KeyItem key = new KeyItem();
                key.Id = Settings.ContainerInner.NextKeyNumber;
                key.Name = string.Format("Key {0}", Settings.ContainerInner.NextKeyNumber);
                Settings.ContainerInner.NextKeyNumber++;
                key.ActiveKeyset = true;
                key.KeysetId = 1;
                key.Sln = 1;
                key.KeyTypeAuto = true;
                key.KeyTypeTek = false;
                key.KeyTypeKek = false;
                key.KeyId = 1;
                key.AlgorithmId = 0x84;
                key.Key = BitConverter.ToString(KeyGenerator.GenerateVarKey(32).ToArray()).Replace("-", string.Empty);
                Settings.ContainerInner.Keys.Add(key);
                keysListView.SelectedItem = key;
            }
            else if (containerTabControl.SelectedItem == groupsTabItem)
            {
                Container.FileStructure.EKC.GroupItem group = new Container.FileStructure.EKC.GroupItem();
                group.Id = Settings.ContainerInner.NextGroupNumber;
                group.Name = string.Format("Group {0}", Settings.ContainerInner.NextGroupNumber);
                Settings.ContainerInner.NextGroupNumber++;
                group.Keys = new List<int>();
                Settings.ContainerInner.Groups.Add(group);
                groupsListView.SelectedItem = group;
            }
            else if (containerTabControl.SelectedItem == ukekTabItem)
            {
                RSIKeyItem key = new RSIKeyItem();
                key.Id = Settings.ContainerInner.NextUKEKNumber;
                key.RsiId = 0;
                key.Name = "0";
                Settings.ContainerInner.NextUKEKNumber++;
                key.ActiveKeyset = true;
                key.KeysetId = 255;
                key.Sln = 61440;
                key.KeyTypeAuto = false;
                key.KeyTypeTek = false;
                key.KeyTypeKek = true;
                key.KeyId = 62440;
                key.AlgorithmId = 0x84;
                key.Key = BitConverter.ToString(KeyGenerator.GenerateVarKey(32).ToArray()).Replace("-", string.Empty);
                Settings.ContainerInner.UKEKs.Add(key);
                ukekListView.SelectedItem = key;
            }
            else if (containerTabControl.SelectedItem == llaTabItem)
            {
                RSIKeyItem key = new RSIKeyItem();
                key.Id = Settings.ContainerInner.NextLLANumber;
                key.RsiId = 0;
                key.Name = "0";
                Settings.ContainerInner.NextLLANumber++;
                key.ActiveKeyset = true;
                key.KeysetId = 255;
                key.Sln = 0;
                key.KeyTypeAuto = false;
                key.KeyTypeTek = false;
                key.KeyTypeKek = true;
                key.KeyId = 1;
                key.AlgorithmId = 0x85;
                key.Key = BitConverter.ToString(KeyGenerator.GenerateVarKey(16).ToArray()).Replace("-", string.Empty);
                Settings.ContainerInner.LLAs.Add(key);
                llaListView.SelectedItem = key;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Up_Click(object sender, RoutedEventArgs e)
        {
            if (containerTabControl.SelectedItem == keysTabItem)
            {
                if (keysListView.SelectedItem != null)
                {
                    int index = keysListView.SelectedIndex;
                    if (index - 1 >= 0)
                        Settings.ContainerInner.Keys.Move(index, index - 1);
                }
            }
            else if (containerTabControl.SelectedItem == groupsTabItem)
            {
                if (groupsListView.SelectedItem != null)
                {
                    int index = groupsListView.SelectedIndex;
                    if (index - 1 >= 0)
                        Settings.ContainerInner.Groups.Move(index, index - 1);
                }
            }
            else if (containerTabControl.SelectedItem == ukekTabItem)
            {
                if (ukekListView.SelectedItem != null)
                {
                    int index = ukekListView.SelectedIndex;
                    if (index - 1 >= 0)
                        Settings.ContainerInner.UKEKs.Move(index, index - 1);
                }
            }
            else if (containerTabControl.SelectedItem == llaTabItem)
            {
                if (llaListView.SelectedItem != null)
                {
                    int index = llaListView.SelectedIndex;
                    if (index - 1 >= 0)
                        Settings.ContainerInner.LLAs.Move(index, index - 1);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Down_Click(object sender, RoutedEventArgs e)
        {
            if (containerTabControl.SelectedItem == keysTabItem)
            {
                if (keysListView.SelectedItem != null)
                {
                    int index = keysListView.SelectedIndex;
                    if (index + 1 < Settings.ContainerInner.Keys.Count)
                        Settings.ContainerInner.Keys.Move(index, index + 1);
                }
            }
            else if (containerTabControl.SelectedItem == groupsTabItem)
            {
                if (groupsListView.SelectedItem != null)
                {
                    int index = groupsListView.SelectedIndex;
                    if (index + 1 < Settings.ContainerInner.Groups.Count)
                        Settings.ContainerInner.Groups.Move(index, index + 1);
                }
            }
            else if (containerTabControl.SelectedItem == ukekTabItem)
            {
                if (ukekListView.SelectedItem != null)
                {
                    int index = ukekListView.SelectedIndex;
                    if (index + 1 < Settings.ContainerInner.UKEKs.Count)
                        Settings.ContainerInner.UKEKs.Move(index, index + 1);
                }
            }
            else if (containerTabControl.SelectedItem == llaTabItem)
            {
                if (llaListView.SelectedItem != null)
                {
                    int index = llaListView.SelectedIndex;
                    if (index + 1 < Settings.ContainerInner.LLAs.Count)
                        Settings.ContainerInner.LLAs.Move(index, index + 1);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            if (containerTabControl.SelectedItem == keysTabItem)
            {
                if (keysListView.SelectedItem != null)
                {
                    int index = keysListView.SelectedIndex;
                    int id = Settings.ContainerInner.Keys[index].Id;

                    // remove key reference from groups
                    foreach (Container.FileStructure.EKC.GroupItem groupItem in Settings.ContainerInner.Groups)
                    {
                        if (groupItem.Keys.Contains(id))
                            groupItem.Keys.Remove(id);
                    }

                    // remove key item
                    Settings.ContainerInner.Keys.RemoveAt(index);
                }
            }
            else if (containerTabControl.SelectedItem == groupsTabItem)
            {
                if (groupsListView.SelectedItem != null)
                {
                    int index = groupsListView.SelectedIndex;
                    Settings.ContainerInner.Groups.RemoveAt(index);
                }
            }
            else if (containerTabControl.SelectedItem == ukekTabItem)
            {
                if (ukekListView.SelectedItem != null)
                {
                    int index = ukekListView.SelectedIndex;
                    int id = Settings.ContainerInner.UKEKs[index].Id;

                    // remove key item
                    Settings.ContainerInner.UKEKs.RemoveAt(index);
                }
            }
            else if (containerTabControl.SelectedItem == llaTabItem)
            {
                if (ukekListView.SelectedItem != null)
                {
                    int index = ukekListView.SelectedIndex;
                    int id = Settings.ContainerInner.LLAs[index].Id;

                    // remove key item
                    Settings.ContainerInner.LLAs.RemoveAt(index);
                }
            }
        }
    }
}
