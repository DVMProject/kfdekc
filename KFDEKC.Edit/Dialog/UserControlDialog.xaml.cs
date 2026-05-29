// SPDX-License-Identifier: MIT
/*
* KFDtool Container (EKC) Editor
* MIT Open Source. Use is subject to license terms.
* DO NOT ALTER OR REMOVE COPYRIGHT NOTICES OR THIS FILE HEADER.
*
*/

using System.Windows;
using System.Windows.Controls;

namespace KFDEKC.Edit.Dialog
{
    /// <summary>
    /// Interaction logic for UserControlDialog.xaml
    /// </summary>
    public partial class UserControlDialog : Window
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="owner"></param>
        /// <param name="control"></param>
        /// <param name="title"></param>
        public UserControlDialog(Window owner, UserControl control, string title = "")
        {
            InitializeComponent();
            this.control.Children.Add(control);

            this.Title = title;
            this.Owner = owner;
            this.WindowStartupLocation = WindowStartupLocation.CenterOwner;

            // resize dialog to control
            this.Width = control.Width;
            this.Height = control.Height;
        }
    }
}
