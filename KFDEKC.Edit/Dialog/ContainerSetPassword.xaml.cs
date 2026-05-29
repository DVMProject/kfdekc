// SPDX-License-Identifier: MIT
/*
* KFDtool Container (EKC) Editor
* MIT Open Source. Use is subject to license terms.
* DO NOT ALTER OR REMOVE COPYRIGHT NOTICES OR THIS FILE HEADER.
*
*/

using System.Windows;

namespace KFDEKC.Edit.Dialog
{
    /// <summary>
    /// Interaction logic for ContainerSetPassword.xaml
    /// </summary>
    public partial class ContainerSetPassword : Window
    {
        /// <summary>
        /// 
        /// </summary>
        public bool PasswordSet { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string PasswordText { get; set; }

        /*
        ** Methods
        */

        /// <summary>
        /// 
        /// </summary>
        public ContainerSetPassword()
        {
            InitializeComponent();

            PasswordSet = false;
            PasswordText = string.Empty;

            txtPassword.Focus(); // focus first password field on load
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Set_Password_Click(object sender, RoutedEventArgs e)
        {
            if (txtPassword.Password != txtPasswordConfirm.Password)
            {
                MessageBox.Show("Passwords do not match", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (txtPassword.Password.Length == 0)
            {
                MessageBox.Show("Password is required", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (txtPassword.Password.Length < 16)
            {
                MessageBoxResult res = MessageBox.Show("This password is weak (under 16 characters in length) - use anyways?", "Warning", MessageBoxButton.YesNo, MessageBoxImage.Warning, MessageBoxResult.No);

                if (res == MessageBoxResult.No)
                {
                    return;
                }
            }

            PasswordSet = true;
            PasswordText = txtPassword.Password;

            Close();
        }
    }
}
