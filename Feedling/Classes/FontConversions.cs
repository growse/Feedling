/*
Copyright © 2008-2011, Andrew Rowson
All rights reserved.

Redistribution and use in source and binary forms, with or without
modification, are permitted provided that the following conditions are met:
    * Redistributions of source code must retain the above copyright
      notice, this list of conditions and the following disclaimer.
    * Redistributions in binary form must reproduce the above copyright
      notice, this list of conditions and the following disclaimer in the
      documentation and/or other materials provided with the distribution.
    * Neither the name of Feedling nor the
      names of its contributors may be used to endorse or promote products
      derived from this software without specific prior written permission.

THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND
ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
DISCLAIMED. IN NO EVENT SHALL <COPYRIGHT HOLDER> BE LIABLE FOR ANY
DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
(INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND
ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
(INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
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
