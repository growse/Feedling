/*
Copyright © 2008-2012, Andrew Rowson
All rights reserved.

See LICENSE file for license details.
*/

using System;
using System.Windows;

namespace Feedling.Classes
{
    public static class FontConversions
    {
        /// <summary>
        /// Convert FontStyle to string for serialization
        /// </summary>
        public static string FontStyleToString(FontStyle value)
        {
            string result;

            try
            {
                result = new FontStyleConverter().ConvertToString(value);
            }
            catch (NotSupportedException)
            {
                result = "";
            }

            return result;
        }

        /// <summary>
        /// Convert string to FontStyle for serialization
        /// </summary>
        public static FontStyle FontStyleFromString(string value)
        {
            var result = FontStyles.Normal;

            try
            {
                var convertFromString = new FontStyleConverter().ConvertFromString((value));
                if (convertFromString != null)
                {
                    result = (FontStyle)convertFromString;
                }
            }
            catch (NotSupportedException)
            {
            }
            catch (FormatException)
            {
            }

            return result;
        }

        /// <summary>
        /// Convert FontWeight to string for serialization
        /// </summary>
        public static string FontWeightToString(FontWeight value)
        {
            string result;

            try
            {
                result = new FontWeightConverter().ConvertToString(value);
            }
            catch (NotSupportedException)
            {
                result = "";
            }

            return result;
        }

        /// <summary>
        /// Convert string to FontWeight for serialization
        /// </summary>
        public static FontWeight FontWeightFromString(string value)
        {
            var result = FontWeights.Normal;

            try
            {
                var convertFromString = new FontWeightConverter().ConvertFromString((value));
                if (convertFromString != null)
                {
                    result = (FontWeight)convertFromString;
                }
            }
            catch (NotSupportedException)
            {
            }
            catch (FormatException)
            {
            }

            return result;
        }


    }
}
