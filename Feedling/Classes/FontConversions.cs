/*
Copyright © 2008-2011, Andrew Rowson
All rights reserved.

See LICENSE file for license details.
*/
using System;
using System.Windows;
namespace Feedling
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
            FontStyle result;

            try
            {
                result = (FontStyle)new FontStyleConverter().ConvertFromString((value));
            }
            catch (NotSupportedException)
            {
                result = FontStyles.Normal;
            }
            catch (FormatException)
            {
                result = FontStyles.Normal;
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
            FontWeight result;

            try
            {
                result = (FontWeight)new FontWeightConverter().ConvertFromString((value));
            }
            catch (NotSupportedException)
            {
                result = FontWeights.Normal;
            }
            catch (FormatException)
            {
                result = FontWeights.Normal;
            }

            return result;
        }

     
    }
}
