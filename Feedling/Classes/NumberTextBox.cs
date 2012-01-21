/*
Copyright © 2008-2012, Andrew Rowson
All rights reserved.

See LICENSE file for license details.
*/

using System;
using System.Windows.Controls;
using System.Windows.Input;

namespace Feedling.Classes
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
            e.Handled = (e.Key == Key.Space);
            base.OnPreviewKeyDown(e);
        }

        private bool nosymbols;

        public bool NoSymbols
        {
            get { return nosymbols; }
            set { nosymbols = value; }
        }

        bool AreAllValidNumericChars(string str)
        {
            var ret = true;
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
                    return true;
            }

            var l = str.Length;
            for (var i = 0; i < l; i++)
            {
                var ch = str[i];
                ret &= Char.IsDigit(ch);
            }

            return ret;
        }
    }

}
