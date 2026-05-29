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
    /// Interaction logic for ContainerEnterPassword.xaml
    /// </summary>
    public partial class ContainerEnterPassword : Window
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
        public ContainerEnterPassword()
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
        private void Submit_Click(object sender, RoutedEventArgs e)
        {
            if (txtPassword.Password.Length == 0)
            {
                MessageBox.Show("Password is required", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            PasswordSet = true;
            PasswordText = txtPassword.Password;

            Close();
        }
    }
}
