// SPDX-License-Identifier: MIT
/*
* KFDtool Container (EKC) Editor
* MIT Open Source. Use is subject to license terms.
* DO NOT ALTER OR REMOVE COPYRIGHT NOTICES OR THIS FILE HEADER.
*
*/

using System;
using System.Globalization;
using System.Windows.Data;

using KFDEKC.P25;

namespace KFDtool.Gui.Converters
{
    /// <summary>
    /// 
    /// </summary>
    public class AlgorithmFormatConverter : IValueConverter
    {
        /*
        ** Methods
        */

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int intValue = (int)value;

            if (Enum.IsDefined(typeof(AlgorithmId), (byte)intValue))
                return string.Format("{0} (0x{0:X}) {1}", intValue, ((AlgorithmId)intValue).ToString());
            else
                return string.Format("{0} (0x{0:X}) UNKNOWN", intValue);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
