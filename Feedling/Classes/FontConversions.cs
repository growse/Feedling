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
                result = (string)(new FontStyleConverter().ConvertToString(value));
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
                result = (string)(new FontWeightConverter().ConvertToString(value));
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

        /// <summary>
        /// Convert FontStretch to string for serialization
        /// </summary>
        public static string FontStretchToString(FontStretch value)
        {
            string result;

            try
            {
                result = (string)(new FontStretchConverter().ConvertToString(value));
            }
            catch (NotSupportedException)
            {
                result = "";
            }

            return result;
        }

        /// <summary>
        /// Convert string to FontStretch for serialization
        /// </summary>
        public static FontStretch FontStretchFromString(string value)
        {
            FontStretch result;

            try
            {
                result = (FontStretch)new FontStretchConverter().ConvertFromString((value));
            }
            catch (NotSupportedException)
            {
                result = FontStretches.Normal;
            }
            catch (FormatException)
            {
                result = FontStretches.Normal;
            }

            return result;
        }
    }
}
