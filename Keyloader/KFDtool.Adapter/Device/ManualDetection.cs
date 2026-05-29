// SPDX-License-Identifier: MIT
/*
* KFDtool Container (EKC) Editor
* MIT Open Source. Use is subject to license terms.
* DO NOT ALTER OR REMOVE COPYRIGHT NOTICES OR THIS FILE HEADER.
*
*/

using System;
using System.Collections.Generic;
using System.Management;
using System.Text.RegularExpressions;

namespace KFDtool.Adapter.Device
{
    /// <summary>
    /// 
    /// </summary>
    public class ManualDetection
    {
        /*
        ** Methods
        */

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static List<string> DetectConnectedAppDevices()
        {
            List<string> devices = new List<string>();
            
            ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT * FROM Win32_PnPEntity WHERE ClassGuid=\"{4d36e978-e325-11ce-bfc1-08002be10318}\"");

            foreach (ManagementObject queryObj in searcher.Get())
            {
                string caption = queryObj["Caption"].ToString();

                // match "COM10" from "KFDtool (COM10)"
                // do not match "KFDtool" which appears before the COM port is assigned
                Regex regex = new Regex(@"\((COM\d+)\)$");

                Match match = regex.Match(caption);

                if (match.Success)
                {
                    string port = match.Groups[1].ToString();
                    devices.Add(port);
                }
            }

            return devices;
        }
    }
}
