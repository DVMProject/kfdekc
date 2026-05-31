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
using KFDtool.P25.TransferConstructs;

namespace KFDEKC.Edit.Control
{
    /// <summary>
    /// Interaction logic for ContainerViewKeyInfo.xaml
    /// </summary>
    public partial class ContainerViewKeyInfo : UserControl
    {
        /// <summary>
        /// 
        /// </summary>
        public ContainerViewKeyInfo(Window parent)
        {
            InitializeComponent();

            KeyItems.ItemsSource = null; // clear table
            KeyItems.ItemsSource = Settings.ContainerInner.Keys;

            KeyCount.Text = $"{Settings.ContainerInner.Keys.Count} Key(s)";
        }
    }
}
