// SPDX-License-Identifier: MIT
/*
* KFDtool Container (EKC) Editor
* MIT Open Source. Use is subject to license terms.
* DO NOT ALTER OR REMOVE COPYRIGHT NOTICES OR THIS FILE HEADER.
*
*/

using System;
using System.Diagnostics;
using System.Windows;

namespace KFDtool.Gui
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        /*
        ** Methods
        */

        /// <summary>
        /// 
        /// </summary>
        public App()
        {
            AppDomain currentDomain = AppDomain.CurrentDomain;
            currentDomain.UnhandledException += CurrentDomain_UnhandledException;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Exception ex = (Exception)e.ExceptionObject;
            Trace.WriteLine("UnhandledException caught: {0}", ex.Message);
            Trace.WriteLine("UnhandledException StackTrace: {0}", ex.StackTrace);
            Trace.WriteLine("Runtime terminating: {0}", e.IsTerminating.ToString());
        }
    }
}
