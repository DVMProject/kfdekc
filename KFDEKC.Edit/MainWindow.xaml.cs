// SPDX-License-Identifier: MIT
/*
* KFDtool Container (EKC) Editor
* MIT Open Source. Use is subject to license terms.
* DO NOT ALTER OR REMOVE COPYRIGHT NOTICES OR THIS FILE HEADER.
*
*/

using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

using fnecore;

using FramePFX.Themes;

using KFDEKC.Container;
using KFDEKC.Container.FileStructure.EKC;
using KFDEKC.Edit.Dialog;

using KFDtool.Adapter.Device;
using KFDtool.P25.TransferConstructs;

namespace KFDEKC.Edit
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private AutoDetection AppDet;

        /*
        ** Methods
        */

        /// <summary>
        /// 
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();

            UpdateContainerText();
            try
            {
                Settings.LoadSettings();
            }
            catch
            {
                MessageBox.Show("Saved settings invalid, resetting to default", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                Settings.InitSettings();
                Settings.LoadSettings();
            }

            // initialize device detection
            AppDet = new AutoDetection();
            AppDet.DevicesChanged += CheckConnectedDevices;

            // Load selected theme
            UpdateWindowTheme();
#if DEBUG
            this.Title = string.Format("KFD EKC Editor/Keyloader {0} DEBUG", Settings.ASSEMBLY_VERSION);
#else
            this.Title = string.Format("KFD EKC Editor/Keyloader {0}", Settings.ASSEMBLY_VERSION);
#endif
            SetMenuStates(false);

            // on load select the type from settings
            switch (Settings.SelectedDevice.DeviceType)
            {
                case BaseDevice.DeviceTypeOptions.DliIp:
                    {
                        SwitchType(TypeDliIp);
                        break;
                    }

                case BaseDevice.DeviceTypeOptions.TwiKfdDevice:
                default:
                    {
                        // Select the appropriate device type based on loaded settings
                        switch (Settings.SelectedDevice.KfdDeviceType)
                        {
                            case TwiKfdDevice.KfdShield:
                            default:
                                {
                                    SwitchType(TypeTwiKfdShield);
                                    break;
                                }
                        }
                        // Select proper com port from loaded settings
                        foreach (MenuItem port in DeviceMenu.Items)
                            if (port.Name == Settings.SelectedDevice.TwiKfdtoolDevice.ComPort)
                                SelectDevice(port);
                        break;
                    }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MainWindow_Closing(object sender, CancelEventArgs e)
        {
            // ask user if container should be saved before exiting
            if (Settings.ContainerOpen)
            {
                if (!Settings.ContainerSaved)
                {
                    MessageBoxResult res = MessageBox.Show("Container is unsaved - save before closing?", "Warning", MessageBoxButton.YesNoCancel, MessageBoxImage.Warning);

                    if (res == MessageBoxResult.Yes)
                    {
                        ContainerSave();
                        ContainerClose();
                    }
                    else if (res == MessageBoxResult.Cancel)
                    {
                        e.Cancel = true;
                        return;
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="enabled"></param>
        private void SetMenuStates(bool enabled)
        {
            containerEdit.IsEnabled = enabled;

            navigateP25MultipleKeyload.IsEnabled = enabled;

            miContainerChangePassword.IsEnabled = enabled;
            miContainerExportDKF.IsEnabled = enabled;
            miContainerExportFNE.IsEnabled = enabled;
            miContainerSave.IsEnabled = enabled;
            miConatinerSaveAs.IsEnabled = enabled;
            miContainerClose.IsEnabled = enabled;
        }

        /// <summary>
        /// 
        /// </summary>
        public void UpdateContainerText()
        {
            if (Settings.ContainerOpen)
                lblSelectedContainer.Text = string.Format("{0}{1}", Settings.ContainerSaved ? string.Empty : "[UNSAVED] ", Settings.ContainerPath);
            else
                lblSelectedContainer.Text = "Not Opened";
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static bool IsSystemLightTheme()
        {
            var key = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Themes\Personalize");
            var value = key?.GetValue("AppsUseLightTheme");
            return value is int i && i > 0;
        }

        /// <summary>
        /// 
        /// </summary>
        private void UpdateWindowTheme()
        {
            // Reset checks
            NavigateUtilityChangeThemeDark.IsChecked = false;
            NavigateUtilityChangeThemeLight.IsChecked = false;
            NavigateUtilityChangeThemeSystem.IsChecked = false;
            // Change theme
            switch (Settings.SelectedTheme)
            {
                // System theme, detect light/dark mode
                case Settings.ThemeMode.System:
                    NavigateUtilityChangeThemeSystem.IsChecked = true;
                    if (IsSystemLightTheme())
                    {
                        //ThemesController.SetTheme(ThemeType.LightTheme);
                        ThemesController.ClearTheme();
                        this.Style = new Style();
                    }
                    else
                    {
                        ThemesController.SetTheme(ThemeType.SoftDark);
                    }
                    break;

                // Light theme
                case Settings.ThemeMode.Light:
                    NavigateUtilityChangeThemeLight.IsChecked = true;
                    //ThemesController.SetTheme(ThemeType.LightTheme);
                    ThemesController.ClearTheme();
                    this.Style = new Style();
                    break;

                // Dark Theme
                case Settings.ThemeMode.Dark:
                    NavigateUtilityChangeThemeDark.IsChecked = true;
                    ThemesController.SetTheme(ThemeType.SoftDark);
                    this.Style = (Style)FindResource("CustomWindowStyle");
                    break;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="path"></param>
        /// <param name="password"></param>
        private void ContainerWriteDKF(string path, string password)
        {
            byte[] contents;

            try
            {
                contents = ContainerUtilities.EncryptOuterContainerDKF(Settings.ContainerOuter, Settings.ContainerInner, password);
            }
            catch (Exception ex)
            {
                MessageBox.Show(string.Format("Failed to encrypt container: {0}", ex.Message), "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            try
            {
                File.WriteAllBytes(path, contents);
            }
            catch (Exception ex)
            {
                MessageBox.Show(string.Format("Failed to write file: {0}", ex.Message), "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            Settings.ContainerSaved = true;

            UpdateContainerText();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="path"></param>
        private void ContainerWrite(string path)
        {
            byte[] contents;

            try
            {
                contents = ContainerUtilities.EncryptOuterContainer(Settings.ContainerOuter, Settings.ContainerInner, Settings.ContainerKey);
            }
            catch (Exception ex)
            {
                MessageBox.Show(string.Format("Failed to encrypt container: {0}", ex.Message), "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            try
            {
                File.WriteAllBytes(path, contents);
            }
            catch (Exception ex)
            {
                MessageBox.Show(string.Format("Failed to write file: {0}", ex.Message), "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            Settings.ContainerPath = path;
            Settings.ContainerSaved = true;

            UpdateContainerText();
        }

        /// <summary>
        /// 
        /// </summary>
        private void ContainerSaveAs()
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Encrypted Key Container (*.ekc)|*.ekc";

            if (saveFileDialog.ShowDialog() == true)
                ContainerWrite(saveFileDialog.FileName);
        }

        private void ContainerSave()
        {
            if (Settings.ContainerPath != string.Empty)
                ContainerWrite(Settings.ContainerPath);
            else
                ContainerSaveAs();
        }

        /// <summary>
        /// 
        /// </summary>
        private void ContainerClose()
        {
            Settings.ContainerOpen = false;
            Settings.ContainerSaved = false;
            Settings.ContainerPath = string.Empty;
            Settings.ContainerKey = null;
            Settings.ContainerOuter = null;
            Settings.ContainerInner = null;

            UpdateContainerText();

            SetMenuStates(false);
            containerEdit.Clear();
        }

        private void StartAppDet()
        {
            AppDet.Start();
        }

        /// <summary>
        /// 
        /// </summary>
        private void StopAppDet()
        {
            AppDet.Stop();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CheckConnectedDevices(object sender, EventArgs e)
        {
            // needed to access UI elements from different thread
            this.Dispatcher.Invoke(() =>
            {
                List<string> ports = AppDet.Devices;

                // sort ports lowest to highest (COM6,COM12,COM26)
                ports.Sort();

                DeviceMenu.Items.Clear();

                // no devices detected
                if (ports.Count == 0)
                {
                    Settings.SelectedDevice.TwiKfdtoolDevice.ComPort = string.Empty;

                    lblSelectedDevice.Text = string.Format(
                        "TWI ({0}) - None",
                        Settings.SelectedDevice.KfdDeviceType.ToString()
                    );

                    MenuItem item = new MenuItem();

                    item.Header = "No devices found";
                    item.IsCheckable = false;
                    item.IsEnabled = false;

                    DeviceMenu.Items.Add(item);
                }

                // one or more devices detected
                foreach (string port in ports)
                {
                    MenuItem item = new MenuItem();

                    item.Name = port;
                    item.Header = port;
                    item.IsCheckable = true;
                    item.Click += Device_MenuItem_Click;

                    DeviceMenu.Items.Add(item);
                }
            });
        }

        /// <summary>
        /// 
        /// </summary>
        private void ResetTwiDeviceInfo()
        {
            lblSelectedDevice.Text = string.Format(
                "TWI ({0}) - None",
                Settings.SelectedDevice.KfdDeviceType.ToString()
            );
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="mi"></param>
        private void SwitchType(MenuItem mi)
        {
            foreach (MenuItem item in TypeMenu.Items)
                item.IsChecked = false;

            mi.IsChecked = true;

            if (mi.Name == "TypeTwiKfdShield")
            {
                DeviceMenu.Items.Clear();

                Settings.SelectedDevice.DeviceType = BaseDevice.DeviceTypeOptions.TwiKfdDevice;
                Settings.SelectedDevice.KfdDeviceType = TwiKfdDevice.KfdShield;

                ResetTwiDeviceInfo();
                StartAppDet();
            }
            else if (mi.Name == "TypeDliIp")
            {
                StopAppDet();

                DeviceMenu.Items.Clear();

                Settings.SelectedDevice.DeviceType = BaseDevice.DeviceTypeOptions.DliIp;

                MenuItem DliIpEdit = new MenuItem();
                DliIpEdit.Header = "_[Edit]";
                DliIpEdit.Click += DliIpEdit_MenuItem_Click;

                DeviceMenu.Items.Add(DliIpEdit);

                UpdateDeviceDliIp();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="mi"></param>
        private void SelectDevice(MenuItem mi)
        {
            if (mi != null)
            {
                foreach (MenuItem item in DeviceMenu.Items)
                    item.IsChecked = false;

                mi.IsChecked = true;

                Settings.SelectedDevice.TwiKfdtoolDevice.ComPort = mi.Name;

                string apVerStr = string.Empty;

                // Save new selection
                Settings.SaveSettings();

                try
                {
                    apVerStr = Interact.ReadAdapterProtocolVersion(Settings.SelectedDevice);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(string.Format("Error -- {0}", ex.Message), "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                Version apVersion = new Version(apVerStr);

                string fwVersion = string.Empty;
                string uniqueId = string.Empty;
                string model = string.Empty;
                string hwRev = string.Empty;
                string serialNum = string.Empty;

                try
                {
                    fwVersion = Interact.ReadFirmwareVersion(Settings.SelectedDevice);
                    uniqueId = Interact.ReadUniqueId(Settings.SelectedDevice);
                    model = Interact.ReadModel(Settings.SelectedDevice);
                    hwRev = Interact.ReadHardwareRevision(Settings.SelectedDevice);
                    serialNum = Interact.ReadSerialNumber(Settings.SelectedDevice);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(string.Format("Error -- {0}", ex.Message), "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                lblSelectedDevice.Text = string.Format(
                    "TWI ({0}) - {1}, Model {2}, HW {3}, Serial {4}, UID {5}, FW {6}",
                    Settings.SelectedDevice.KfdDeviceType.ToString(),
                    Settings.SelectedDevice.TwiKfdtoolDevice.ComPort,
                    model,
                    hwRev,
                    serialNum,
                    uniqueId,
                    fwVersion
                );
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void UpdateDeviceDliIp()
        {
            lblSelectedDevice.Text = string.Format(
                "DLI (IP) - {0}:{1}, {2}, Variant: {3}",
                Settings.SelectedDevice.DliIpDevice.Hostname,
                Settings.SelectedDevice.DliIpDevice.Port.ToString(),
                Settings.SelectedDevice.DliIpDevice.Protocol.ToString(),
                Settings.SelectedDevice.DliIpDevice.Variant.ToString()
            );

            // Save config
            Settings.SaveSettings();
        }

        /*
        ** Form Menu Options
        */

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Exit_MenuItem_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Container_New_Click(object sender, RoutedEventArgs e)
        {
            if (Settings.ContainerOpen)
            {
                MessageBox.Show("A container is already open", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            ContainerSetPassword containerSetPassword = new ContainerSetPassword();
            containerSetPassword.Style = Window.GetWindow(this).Style;
            containerSetPassword.Owner = this; // for centering in parent window
            containerSetPassword.ShowDialog();

            if (containerSetPassword.PasswordSet)
            {
                string password = containerSetPassword.PasswordText;

                Settings.ContainerOpen = true;
                Settings.ContainerSaved = false;
                Settings.ContainerPath = string.Empty;
                (Settings.ContainerOuter, Settings.ContainerKey) = ContainerUtilities.CreateOuterContainer(password);
                Settings.ContainerInner = ContainerUtilities.CreateInnerContainer();

                UpdateContainerText();
            }

            containerEdit.Refresh();
            SetMenuStates(true);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Container_Open_Click(object sender, RoutedEventArgs e)
        {
            if (Settings.ContainerOpen)
            {
                MessageBox.Show("A container is already open", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Encrypted Key Container (*.ekc)|*.ekc|All files (*.*)|*.*";

            if (openFileDialog.ShowDialog() == true)
            {
                string filePath = openFileDialog.FileName;

                if (filePath.Equals(string.Empty))
                {
                    MessageBox.Show("No file selected", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                ContainerEnterPassword containerEnterPassword = new ContainerEnterPassword();
                containerEnterPassword.Style = Window.GetWindow(this).Style;
                containerEnterPassword.Owner = this; // for centering in parent window
                containerEnterPassword.ShowDialog();

                if (containerEnterPassword.PasswordSet)
                {
                    string password = containerEnterPassword.PasswordText;

                    byte[] fileContents;

                    try
                    {
                        fileContents = File.ReadAllBytes(filePath);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(string.Format("Failed to read file: {0}", ex.Message), "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    OuterContainer outerContainer;
                    InnerContainer innerContainer;
                    byte[] key;

                    try
                    {
                        (outerContainer, innerContainer, key) = ContainerUtilities.DecryptOuterContainer(fileContents, password);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(string.Format("Failed to decrypt container: {0}", ex.Message), "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    Settings.ContainerOpen = true;
                    Settings.ContainerSaved = true;
                    Settings.ContainerPath = filePath;
                    Settings.ContainerKey = key;
                    Settings.ContainerOuter = outerContainer;
                    Settings.ContainerInner = innerContainer;

                    UpdateContainerText();
                }

                containerEdit.Refresh();
                SetMenuStates(true);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Container_Change_Password_Click(object sender, RoutedEventArgs e)
        {
            if (Settings.ContainerOpen)
            {
                ContainerSetPassword containerSetPassword = new ContainerSetPassword();
                containerSetPassword.Style = Window.GetWindow(this).Style;
                containerSetPassword.Owner = this; // for centering in parent window
                containerSetPassword.ShowDialog();

                if (containerSetPassword.PasswordSet)
                {
                    string password = containerSetPassword.PasswordText;

                    (Settings.ContainerOuter, Settings.ContainerKey) = ContainerUtilities.CreateOuterContainer(password);

                    Settings.ContainerSaved = false;

                    UpdateContainerText();

                    MessageBox.Show("Password Changed", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            else
                MessageBox.Show("No container open", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Container_Export_DKF_Click(object sender, RoutedEventArgs e)
        {
            if (Settings.ContainerOpen)
            {
                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.Filter = "Distribution Key File (*.dkf)|*.dkf";

                if (saveFileDialog.ShowDialog() == true)
                {
                    ContainerSetPassword containerSetPassword = new ContainerSetPassword();
                    containerSetPassword.Style = Window.GetWindow(this).Style;
                    containerSetPassword.Owner = this; // for centering in parent window
                    containerSetPassword.ShowDialog();

                    if (containerSetPassword.PasswordSet)
                    {
                        string password = containerSetPassword.PasswordText;
                        ContainerWriteDKF(saveFileDialog.FileName, password);
                    }
                }
            }
            else
                MessageBox.Show("No container open", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Container_Import_From_FNE_Click(object sender, RoutedEventArgs e)
        {
            if (Settings.ContainerOpen)
                Container_Close_Click(sender, e); // close an open container
            else
            {
                ContainerDVMCredentials dvmCredentials = new ContainerDVMCredentials();
                dvmCredentials.Style = Window.GetWindow(this).Style;
                dvmCredentials.Owner = this; // for centering in parent window
                dvmCredentials.ShowDialog();

                if (dvmCredentials.DataSet)
                {
                    this.Cursor = Cursors.Wait;

                    IPEndPoint endpoint = IPEndPoint.Parse(dvmCredentials.DVMFNEIP);
                    FnePeer peer = new FnePeer("KFD EKC", dvmCredentials.DVMFNEPeerID, endpoint);
                    peer.Passphrase = dvmCredentials.DVMFNEPeerPassword;
                    peer.StartWithoutMaintainence();

                    peer.KeyInventory += (object sender, KeyInventoryEvent kie) =>
                    {
                        this.Dispatcher.Invoke(() =>
                        {
                            this.Cursor = Cursors.Arrow;
                            peer.Stop();

                            ContainerEnterPassword containerEnterPassword = new ContainerEnterPassword();
                            containerEnterPassword.Style = Window.GetWindow(this).Style;
                            containerEnterPassword.Owner = this; // for centering in parent window
                            containerEnterPassword.ShowDialog();

                            if (containerEnterPassword.PasswordSet)
                            {
                                string password = containerEnterPassword.PasswordText;

                                OuterContainer outerContainer;
                                InnerContainer innerContainer;
                                byte[] key;

                                try
                                {
                                    (outerContainer, innerContainer, key) = ContainerUtilities.DecryptOuterContainer(kie.Data, password);
                                }
                                catch (Exception ex)
                                {
                                    MessageBox.Show(string.Format("Failed to decrypt container: {0}", ex.Message), "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                                    return;
                                }

                                Settings.ContainerOpen = true;
                                Settings.ContainerSaved = true;
                                Settings.ContainerPath = Path.Combine(new string[] { Path.GetTempPath(), Path.GetTempFileName() });
                                Settings.ContainerKey = key;
                                Settings.ContainerOuter = outerContainer;
                                Settings.ContainerInner = innerContainer;

                                UpdateContainerText();
                            }

                            containerEdit.Refresh();
                            SetMenuStates(true);
                        });
                    };

                    peer.SendMasterKeyInventoryRequest(dvmCredentials.DVMFNERemoteAccessPassword);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Container_Export_To_FNE_Click(object sender, RoutedEventArgs e)
        {
            if (Settings.ContainerOpen)
            {
                // first save the open container
                Container_Save_Click(sender, e);

                ContainerDVMCredentials dvmCredentials = new ContainerDVMCredentials();
                dvmCredentials.Style = Window.GetWindow(this).Style;
                dvmCredentials.Owner = this; // for centering in parent window
                dvmCredentials.ShowDialog();

                if (dvmCredentials.DataSet)
                {
                    this.Cursor = Cursors.Wait;

                    IPEndPoint endpoint = IPEndPoint.Parse(dvmCredentials.DVMFNEIP);
                    FnePeer peer = new FnePeer("KFD EKC", dvmCredentials.DVMFNEPeerID, endpoint);
                    peer.Passphrase = dvmCredentials.DVMFNEPeerPassword;
                    peer.StartWithoutMaintainence();

                    byte[] contents;

                    try
                    {
                        contents = ContainerUtilities.EncryptOuterContainer(Settings.ContainerOuter, Settings.ContainerInner, Settings.ContainerKey);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(string.Format("Failed to encrypt container: {0}", ex.Message), "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    peer.SendMasterKeyUpdateRequest(contents, dvmCredentials.DVMFNERemoteAccessPassword);

                    this.Cursor = Cursors.Arrow;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Container_Save_Click(object sender, RoutedEventArgs e)
        {
            if (Settings.ContainerOpen)
                ContainerSave();
            else
                MessageBox.Show("No container open", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Container_Save_As_Click(object sender, RoutedEventArgs e)
        {
            if (Settings.ContainerOpen)
                ContainerSaveAs();
            else
                MessageBox.Show("No container open", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Container_Close_Click(object sender, RoutedEventArgs e)
        {
            if (Settings.ContainerOpen)
            {
                if (!Settings.ContainerSaved)
                {
                    MessageBoxResult res = MessageBox.Show("Container is unsaved - save before closing?", "Warning", MessageBoxButton.YesNoCancel, MessageBoxImage.Warning);
                    if (res == MessageBoxResult.Yes)
                        ContainerSave();
                    else if (res == MessageBoxResult.Cancel)
                        return;
                }

                ContainerClose();
            }
            else
                MessageBox.Show("No container open", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Type_MenuItem_Click(object sender, RoutedEventArgs e)
        {
            MenuItem mi = sender as MenuItem;
            SwitchType(mi);

            // Save config
            Settings.SaveSettings();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DliIpEdit_MenuItem_Click(object sender, RoutedEventArgs e)
        {
            DliIpDeviceEdit wnd = new DliIpDeviceEdit();
            wnd.Owner = this; // for centering in parent window
            wnd.ShowDialog();

            UpdateDeviceDliIp();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Device_MenuItem_Click(object sender, RoutedEventArgs e)
        {
            MenuItem mi = sender as MenuItem;
            SelectDevice(mi);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UserControl_MenuItem_Click(object sender, RoutedEventArgs e)
        {
            MenuItem mi = sender as MenuItem;

            if (mi != null)
            {
                string item = mi.Name;
                UserControlDialog controlDialog = null;

                if (item == "navigateP25MultipleKeyload")
                    controlDialog = new UserControlDialog(this, new Control.P25MultipleKeyload(), "Keyloader - Key Fill");
                else if (item == "navigateP25KeyErase")
                    controlDialog = new UserControlDialog(this, new Control.P25KeyErase(), "Keyloader - Erase Key");
                else if (item == "navigateP25EraseAllKeys")
                    controlDialog = new UserControlDialog(this, new Control.P25EraseAllKeys(), "Keyloader - Erase All Keys");
                else if (item == "navigateP25ViewKeyInfo")
                    controlDialog = new UserControlDialog(this, new Control.P25ViewKeyInfo(), "Keyloader - View Key Information");
                else if (item == "navigateP25ViewKeysetInfo")
                    controlDialog = new UserControlDialog(this, new Control.P25ViewKeysetInfo(), "Keyloader - View Keyset Information");
                else if (item == "navigateP25ViewRsiConfig")
                    controlDialog = new UserControlDialog(this, new Control.P25ViewRsiConfig(), "Keyloader - View RSI Information");
                else if (item == "navigateP25KmfConfig")
                    controlDialog = new UserControlDialog(this, new Control.P25KmfConfig(), "Keyloader - KMF Configuration");
                else
                    throw new Exception(string.Format("unknown item - {0}", mi.Name));

                controlDialog.ShowDialog();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void About_MenuItem_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(("KFD EKC Editor/Keyloader" +
#if DEBUG
                    " DEBUG " +
#else
                    " " +
#endif
                    $"{Settings.ASSEMBLY_VERSION}\n" +
                    "Copyright (c) 2026 DVMProject (https://github.com/dvmproject) Authors\n\n" +

                    "The KFD EKC Editor/Keyloader is based on on the omahacommsys fork of KFDtool: https://github.com/omahacommsys/KFDtool\n\n" +
                    "Portions Copyright (c) 2019-2020 Ellie Dugger\n" +
                    "Portions Copyright (c) 2021-2023 Natalie Moore\n" +
                    "Portions Copyright (c) 2023-2025 Ilya Smirnov\n" +
                    "Portions Copyright (c) 2023-2024 Patrick McDonnell\n"),
                "About", MessageBoxButton.OK);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void NavigateUtilityChangeThemeDark_Click(object sender, RoutedEventArgs e)
        {
            Settings.SelectedTheme = Settings.ThemeMode.Dark;
            Settings.SaveSettings();

            UpdateWindowTheme();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void NavigateUtilityChangeThemeLight_Click(object sender, RoutedEventArgs e)
        {
            Settings.SelectedTheme = Settings.ThemeMode.Light;
            Settings.SaveSettings();

            UpdateWindowTheme();
        }

        private void NavigateUtilityChangeThemeSystem_Click(object sender, RoutedEventArgs e)
        {
            Settings.SelectedTheme = Settings.ThemeMode.System;
            Settings.SaveSettings();

            UpdateWindowTheme();
        }
    }
}
