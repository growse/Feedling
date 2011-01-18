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
using System.Windows.Controls;
using System.Windows.Input;

namespace Feedling
{
    public class NumberTextBox : TextBox
    {
        protected override void OnPreviewTextInput(TextCompositionEventArgs e)
        {
            if (e != null)
            {
                e.Handled = !AreAllValidNumericChars(e.Text);
            }
            base.OnPreviewTextInput(e);
        }

        protected override void OnPreviewKeyDown(KeyEventArgs e)
        {
            if (e != null)
            {
                e.Handled = (e.Key == Key.Space);
            }
            base.OnPreviewKeyDown(e);
        }

        private bool nosymbols = false;

        public bool NoSymbols
        {
            get { return nosymbols; }
            set { nosymbols = value; }
        }

        bool AreAllValidNumericChars(string str)
        {
            bool ret = true;
            if (!nosymbols)
            {
                if (str == System.Globalization.NumberFormatInfo.CurrentInfo.CurrencyDecimalSeparator |
                    str == System.Globalization.NumberFormatInfo.CurrentInfo.CurrencyGroupSeparator |
                    str == System.Globalization.NumberFormatInfo.CurrentInfo.CurrencySymbol |
                    str == System.Globalization.NumberFormatInfo.CurrentInfo.NegativeSign |
                    str == System.Globalization.NumberFormatInfo.CurrentInfo.NegativeInfinitySymbol |
                    str == System.Globalization.NumberFormatInfo.CurrentInfo.NumberDecimalSeparator |
                    str == System.Globalization.NumberFormatInfo.CurrentInfo.NumberGroupSeparator |
                    str == System.Globalization.NumberFormatInfo.CurrentInfo.PercentDecimalSeparator |
                    str == System.Globalization.NumberFormatInfo.CurrentInfo.PercentGroupSeparator |
                    str == System.Globalization.NumberFormatInfo.CurrentInfo.PercentSymbol |
                    str == System.Globalization.NumberFormatInfo.CurrentInfo.PerMilleSymbol |
                    str == System.Globalization.NumberFormatInfo.CurrentInfo.PositiveInfinitySymbol |
                    str == System.Globalization.NumberFormatInfo.CurrentInfo.PositiveSign)
                    return ret;
            }

            int l = str.Length;
            for (int i = 0; i < l; i++)
            {
                char ch = str[i];
                ret &= Char.IsDigit(ch);
            }

            return ret;
        }
    }

}
