// 
// System.Web.HttpUtility
//
// Authors:
//   Patrik Torstensson (Patrik.Torstensson@labs2.com)
//   Wictor WilÃ©n (decode/encode functions) (wictor@ibizkit.se)
//   Tim Coleman (tim@timcoleman.com)
//   Gonzalo Paniagua Javier (gonzalo@ximian.com)
//
// Copyright (C) 2005-2009 Novell, Inc (http://www.novell.com)
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//

using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.IO;
using System.Security.Permissions;
using System.Text;


namespace System.Web
{
    [Security.SecurityCritical()]
    public sealed class HttpUtility
    {
        sealed class HttpQSCollection : NameValueCollection
        {
            public override string ToString()
            {
                int count = Count;
                if (count == 0)
                    return "";
                StringBuilder sb = new StringBuilder();
                string[] keys = AllKeys;
                for (int i = 0; i < count; i++)
                {
                    sb.AppendFormat("{0}={1}&", keys[i], this[keys[i]]);
                }
                if (sb.Length > 0)
                    sb.Length--;
                return sb.ToString();
            }
        }
        #region Fields

        static Hashtable entities;
        static object lock_ = new object();

        #endregion // Fields

        static Hashtable Entities
        {
            get
            {
                lock (lock_)
                {
                    if (entities == null)
                        InitEntities();

                    return entities;
                }
            }
        }

        #region Constructors

        static void InitEntities()
        {
            // Build the hash table of HTML entity references.  This list comes
            // from the HTML 4.01 W3C recommendation.
            entities = new Hashtable();
            entities.Add("nbsp", '\u00A0');
            entities.Add("iexcl", '\u00A1');
            entities.Add("cent", '\u00A2');
            entities.Add("pound", '\u00A3');
            entities.Add("curren", '\u00A4');
            entities.Add("yen", '\u00A5');
            entities.Add("brvbar", '\u00A6');
            entities.Add("sect", '\u00A7');
            entities.Add("uml", '\u00A8');
            entities.Add("copy", '\u00A9');
            entities.Add("ordf", '\u00AA');
            entities.Add("laquo", '\u00AB');
            entities.Add("not", '\u00AC');
            entities.Add("shy", '\u00AD');
            entities.Add("reg", '\u00AE');
            entities.Add("macr", '\u00AF');
            entities.Add("deg", '\u00B0');
            entities.Add("plusmn", '\u00B1');
            entities.Add("sup2", '\u00B2');
            entities.Add("sup3", '\u00B3');
            entities.Add("acute", '\u00B4');
            entities.Add("micro", '\u00B5');
            entities.Add("para", '\u00B6');
            entities.Add("middot", '\u00B7');
            entities.Add("cedil", '\u00B8');
            entities.Add("sup1", '\u00B9');
            entities.Add("ordm", '\u00BA');
            entities.Add("raquo", '\u00BB');
            entities.Add("frac14", '\u00BC');
            entities.Add("frac12", '\u00BD');
            entities.Add("frac34", '\u00BE');
            entities.Add("iquest", '\u00BF');
            entities.Add("Agrave", '\u00C0');
            entities.Add("Aacute", '\u00C1');
            entities.Add("Acirc", '\u00C2');
            entities.Add("Atilde", '\u00C3');
            entities.Add("Auml", '\u00C4');
            entities.Add("Aring", '\u00C5');
            entities.Add("AElig", '\u00C6');
            entities.Add("Ccedil", '\u00C7');
            entities.Add("Egrave", '\u00C8');
            entities.Add("Eacute", '\u00C9');
            entities.Add("Ecirc", '\u00CA');
            entities.Add("Euml", '\u00CB');
            entities.Add("Igrave", '\u00CC');
            entities.Add("Iacute", '\u00CD');
            entities.Add("Icirc", '\u00CE');
            entities.Add("Iuml", '\u00CF');
            entities.Add("ETH", '\u00D0');
            entities.Add("Ntilde", '\u00D1');
            entities.Add("Ograve", '\u00D2');
            entities.Add("Oacute", '\u00D3');
            entities.Add("Ocirc", '\u00D4');
            entities.Add("Otilde", '\u00D5');
            entities.Add("Ouml", '\u00D6');
            entities.Add("times", '\u00D7');
            entities.Add("Oslash", '\u00D8');
            entities.Add("Ugrave", '\u00D9');
            entities.Add("Uacute", '\u00DA');
            entities.Add("Ucirc", '\u00DB');
            entities.Add("Uuml", '\u00DC');
            entities.Add("Yacute", '\u00DD');
            entities.Add("THORN", '\u00DE');
            entities.Add("szlig", '\u00DF');
            entities.Add("agrave", '\u00E0');
            entities.Add("aacute", '\u00E1');
            entities.Add("acirc", '\u00E2');
            entities.Add("atilde", '\u00E3');
            entities.Add("auml", '\u00E4');
            entities.Add("aring", '\u00E5');
            entities.Add("aelig", '\u00E6');
            entities.Add("ccedil", '\u00E7');
            entities.Add("egrave", '\u00E8');
            entities.Add("eacute", '\u00E9');
            entities.Add("ecirc", '\u00EA');
            entities.Add("euml", '\u00EB');
            entities.Add("igrave", '\u00EC');
            entities.Add("iacute", '\u00ED');
            entities.Add("icirc", '\u00EE');
            entities.Add("iuml", '\u00EF');
            entities.Add("eth", '\u00F0');
            entities.Add("ntilde", '\u00F1');
            entities.Add("ograve", '\u00F2');
            entities.Add("oacute", '\u00F3');
            entities.Add("ocirc", '\u00F4');
            entities.Add("otilde", '\u00F5');
            entities.Add("ouml", '\u00F6');
            entities.Add("divide", '\u00F7');
            entities.Add("oslash", '\u00F8');
            entities.Add("ugrave", '\u00F9');
            entities.Add("uacute", '\u00FA');
            entities.Add("ucirc", '\u00FB');
            entities.Add("uuml", '\u00FC');
            entities.Add("yacute", '\u00FD');
            entities.Add("thorn", '\u00FE');
            entities.Add("yuml", '\u00FF');
            entities.Add("fnof", '\u0192');
            entities.Add("Alpha", '\u0391');
            entities.Add("Beta", '\u0392');
            entities.Add("Gamma", '\u0393');
            entities.Add("Delta", '\u0394');
            entities.Add("Epsilon", '\u0395');
            entities.Add("Zeta", '\u0396');
            entities.Add("Eta", '\u0397');
            entities.Add("Theta", '\u0398');
            entities.Add("Iota", '\u0399');
            entities.Add("Kappa", '\u039A');
            entities.Add("Lambda", '\u039B');
            entities.Add("Mu", '\u039C');
            entities.Add("Nu", '\u039D');
            entities.Add("Xi", '\u039E');
            entities.Add("Omicron", '\u039F');
            entities.Add("Pi", '\u03A0');
            entities.Add("Rho", '\u03A1');
            entities.Add("Sigma", '\u03A3');
            entities.Add("Tau", '\u03A4');
            entities.Add("Upsilon", '\u03A5');
            entities.Add("Phi", '\u03A6');
            entities.Add("Chi", '\u03A7');
            entities.Add("Psi", '\u03A8');
            entities.Add("Omega", '\u03A9');
            entities.Add("alpha", '\u03B1');
            entities.Add("beta", '\u03B2');
            entities.Add("gamma", '\u03B3');
            entities.Add("delta", '\u03B4');
            entities.Add("epsilon", '\u03B5');
            entities.Add("zeta", '\u03B6');
            entities.Add("eta", '\u03B7');
            entities.Add("theta", '\u03B8');
            entities.Add("iota", '\u03B9');
            entities.Add("kappa", '\u03BA');
            entities.Add("lambda", '\u03BB');
            entities.Add("mu", '\u03BC');
            entities.Add("nu", '\u03BD');
            entities.Add("xi", '\u03BE');
            entities.Add("omicron", '\u03BF');
            entities.Add("pi", '\u03C0');
            entities.Add("rho", '\u03C1');
            entities.Add("sigmaf", '\u03C2');
            entities.Add("sigma", '\u03C3');
            entities.Add("tau", '\u03C4');
            entities.Add("upsilon", '\u03C5');
            entities.Add("phi", '\u03C6');
            entities.Add("chi", '\u03C7');
            entities.Add("psi", '\u03C8');
            entities.Add("omega", '\u03C9');
            entities.Add("thetasym", '\u03D1');
            entities.Add("upsih", '\u03D2');
            entities.Add("piv", '\u03D6');
            entities.Add("bull", '\u2022');
            entities.Add("hellip", '\u2026');
            entities.Add("prime", '\u2032');
            entities.Add("Prime", '\u2033');
            entities.Add("oline", '\u203E');
            entities.Add("frasl", '\u2044');
            entities.Add("weierp", '\u2118');
            entities.Add("image", '\u2111');
            entities.Add("real", '\u211C');
            entities.Add("trade", '\u2122');
            entities.Add("alefsym", '\u2135');
            entities.Add("larr", '\u2190');
            entities.Add("uarr", '\u2191');
            entities.Add("rarr", '\u2192');
            entities.Add("darr", '\u2193');
            entities.Add("harr", '\u2194');
            entities.Add("crarr", '\u21B5');
            entities.Add("lArr", '\u21D0');
            entities.Add("uArr", '\u21D1');
            entities.Add("rArr", '\u21D2');
            entities.Add("dArr", '\u21D3');
            entities.Add("hArr", '\u21D4');
            entities.Add("forall", '\u2200');
            entities.Add("part", '\u2202');
            entities.Add("exist", '\u2203');
            entities.Add("empty", '\u2205');
            entities.Add("nabla", '\u2207');
            entities.Add("isin", '\u2208');
            entities.Add("notin", '\u2209');
            entities.Add("ni", '\u220B');
            entities.Add("prod", '\u220F');
            entities.Add("sum", '\u2211');
            entities.Add("minus", '\u2212');
            entities.Add("lowast", '\u2217');
            entities.Add("radic", '\u221A');
            entities.Add("prop", '\u221D');
            entities.Add("infin", '\u221E');
            entities.Add("ang", '\u2220');
            entities.Add("and", '\u2227');
            entities.Add("or", '\u2228');
            entities.Add("cap", '\u2229');
            entities.Add("cup", '\u222A');
            entities.Add("int", '\u222B');
            entities.Add("there4", '\u2234');
            entities.Add("sim", '\u223C');
            entities.Add("cong", '\u2245');
            entities.Add("asymp", '\u2248');
            entities.Add("ne", '\u2260');
            entities.Add("equiv", '\u2261');
            entities.Add("le", '\u2264');
            entities.Add("ge", '\u2265');
            entities.Add("sub", '\u2282');
            entities.Add("sup", '\u2283');
            entities.Add("nsub", '\u2284');
            entities.Add("sube", '\u2286');
            entities.Add("supe", '\u2287');
            entities.Add("oplus", '\u2295');
            entities.Add("otimes", '\u2297');
            entities.Add("perp", '\u22A5');
            entities.Add("sdot", '\u22C5');
            entities.Add("lceil", '\u2308');
            entities.Add("rceil", '\u2309');
            entities.Add("lfloor", '\u230A');
            entities.Add("rfloor", '\u230B');
            entities.Add("lang", '\u2329');
            entities.Add("rang", '\u232A');
            entities.Add("loz", '\u25CA');
            entities.Add("spades", '\u2660');
            entities.Add("clubs", '\u2663');
            entities.Add("hearts", '\u2665');
            entities.Add("diams", '\u2666');
            entities.Add("quot", '\u0022');
            entities.Add("amp", '\u0026');
            entities.Add("lt", '\u003C');
            entities.Add("gt", '\u003E');
            entities.Add("OElig", '\u0152');
            entities.Add("oelig", '\u0153');
            entities.Add("Scaron", '\u0160');
            entities.Add("scaron", '\u0161');
            entities.Add("Yuml", '\u0178');
            entities.Add("circ", '\u02C6');
            entities.Add("tilde", '\u02DC');
            entities.Add("ensp", '\u2002');
            entities.Add("emsp", '\u2003');
            entities.Add("thinsp", '\u2009');
            entities.Add("zwnj", '\u200C');
            entities.Add("zwj", '\u200D');
            entities.Add("lrm", '\u200E');
            entities.Add("rlm", '\u200F');
            entities.Add("ndash", '\u2013');
            entities.Add("mdash", '\u2014');
            entities.Add("lsquo", '\u2018');
            entities.Add("rsquo", '\u2019');
            entities.Add("sbquo", '\u201A');
            entities.Add("ldquo", '\u201C');
            entities.Add("rdquo", '\u201D');
            entities.Add("bdquo", '\u201E');
            entities.Add("dagger", '\u2020');
            entities.Add("Dagger", '\u2021');
            entities.Add("permil", '\u2030');
            entities.Add("lsaquo", '\u2039');
            entities.Add("rsaquo", '\u203A');
            entities.Add("euro", '\u20AC');
        }

        public HttpUtility()
        {
        }

        #endregion // Constructors

        #region Methods

        public static void HtmlAttributeEncode(string value, TextWriter output)
        {
            if (output != null)
            {
                output.Write(HtmlAttributeEncode(value));
            }
        }

        public static string HtmlAttributeEncode(string value)
        {
            if (null == value)
                return null;

            bool needEncode = false;
            for (int i = 0; i < value.Length; i++)
            {
                if (value[i] == '&' || value[i] == '"' || value[i] == '<')
                {
                    needEncode = true;
                    break;
                }
            }

            if (!needEncode)
                return value;

            StringBuilder output = new StringBuilder();
            int len = value.Length;
            for (int i = 0; i < len; i++)
                switch (value[i])
                {
                    case '&':
                        output.Append("&amp;");
                        break;
                    case '"':
                        output.Append("&quot;");
                        break;
                    case '<':
                        output.Append("&lt;");
                        break;
                    default:
                        output.Append(value[i]);
                        break;
                }

            return output.ToString();
        }

        public static string UrlDecode(string value)
        {
            return UrlDecode(value, Encoding.UTF8);
        }

        static char[] GetChars(MemoryStream stream, Encoding encoding)
        {
            return encoding.GetChars(stream.GetBuffer(), 0, (int)stream.Length);
        }

        static void WriteCharBytes(IList buffer, char character, Encoding encoding)
        {
            if (character > 255)
            {
                foreach (byte b in encoding.GetBytes(new char[] { character }))
                    buffer.Add(b);
            }
            else
                buffer.Add((byte)character);
        }

        public static string UrlDecode(string value, Encoding encoding)
        {
            if (null == value)
                return null;

            if (value.IndexOf('%') == -1 && value.IndexOf('+') == -1)
                return value;

            if (encoding == null)
                encoding = Encoding.UTF8;

            long len = value.Length;
            var bytes = new List<byte>();
            int xchar;
            char ch;

            for (int i = 0; i < len; i++)
            {
                ch = value[i];
                if (ch == '%' && i + 2 < len && value[i + 1] != '%')
                {
                    if (value[i + 1] == 'u' && i + 5 < len)
                    {
                        // unicode hex sequence
                        xchar = GetChar(value, i + 2, 4);
                        if (xchar != -1)
                        {
                            WriteCharBytes(bytes, (char)xchar, encoding);
                            i += 5;
                        }
                        else
                            WriteCharBytes(bytes, '%', encoding);
                    }
                    else if ((xchar = GetChar(value, i + 1, 2)) != -1)
                    {
                        WriteCharBytes(bytes, (char)xchar, encoding);
                        i += 2;
                    }
                    else
                    {
                        WriteCharBytes(bytes, '%', encoding);
                    }
                    continue;
                }

                if (ch == '+')
                    WriteCharBytes(bytes, ' ', encoding);
                else
                    WriteCharBytes(bytes, ch, encoding);
            }

            byte[] buf = bytes.ToArray();
            bytes = null;
            return encoding.GetString(buf);

        }

        public static string UrlDecode(byte[] value, Encoding encoding)
        {
            if (value == null)
                return null;

            return UrlDecode(value, 0, value.Length, encoding);
        }

        static int GetInt(byte b)
        {
            char c = (char)b;
            if (c >= '0' && c <= '9')
                return c - '0';

            if (c >= 'a' && c <= 'f')
                return c - 'a' + 10;

            if (c >= 'A' && c <= 'F')
                return c - 'A' + 10;

            return -1;
        }

        static int GetChar(byte[] value, int offset, int length)
        {
            int ret = 0;
            int end = length + offset;
            for (int i = offset; i < end; i++)
            {
                int current = GetInt(value[i]);
                if (current == -1)
                    return -1;
                ret = (ret << 4) + current;
            }

            return ret;
        }

        static int GetChar(string str, int offset, int length)
        {
            int val = 0;
            int end = length + offset;
            for (int i = offset; i < end; i++)
            {
                char c = str[i];
                if (c > 127)
                    return -1;

                int current = GetInt((byte)c);
                if (current == -1)
                    return -1;
                val = (val << 4) + current;
            }

            return val;
        }

        public static string UrlDecode(byte[] value, int offset, int count, Encoding encoding)
        {
            if (value == null)
                return null;
            if (count == 0)
                return String.Empty;

            if (value == null)
                throw new ArgumentNullException("value");

            if (offset < 0 || offset > value.Length)
                throw new ArgumentOutOfRangeException("offset");

            if (count < 0 || offset + count > value.Length)
                throw new ArgumentOutOfRangeException("count");

            StringBuilder output = new StringBuilder();
            using (MemoryStream acc = new MemoryStream())
            {

                int end = count + offset;
                int xchar;
                for (int i = offset; i < end; i++)
                {
                    if (value[i] == '%' && i + 2 < count && value[i + 1] != '%')
                    {
                        if (value[i + 1] == (byte)'u' && i + 5 < end)
                        {
                            if (acc.Length > 0)
                            {
                                output.Append(GetChars(acc, encoding));
                                acc.SetLength(0);
                            }
                            xchar = GetChar(value, i + 2, 4);
                            if (xchar != -1)
                            {
                                output.Append((char)xchar);
                                i += 5;
                                continue;
                            }
                        }
                        else if ((xchar = GetChar(value, i + 1, 2)) != -1)
                        {
                            acc.WriteByte((byte)xchar);
                            i += 2;
                            continue;
                        }
                    }

                    if (acc.Length > 0)
                    {
                        output.Append(GetChars(acc, encoding));
                        acc.SetLength(0);
                    }

                    if (value[i] == '+')
                    {
                        output.Append(' ');
                    }
                    else
                    {
                        output.Append((char)value[i]);
                    }
                }

                if (acc.Length > 0)
                {
                    output.Append(GetChars(acc, encoding));
                }

            }
            return output.ToString();
        }

        public static byte[] UrlDecodeToBytes(byte[] value)
        {
            if (value == null)
                return null;

            return UrlDecodeToBytes(value, 0, value.Length);
        }

        public static byte[] UrlDecodeToBytes(string value)
        {
            return UrlDecodeToBytes(value, Encoding.UTF8);
        }

        public static byte[] UrlDecodeToBytes(string value, Encoding encoding)
        {
            if (value == null)
                return null;

            if (encoding == null)
                throw new ArgumentNullException("e");

            return UrlDecodeToBytes(encoding.GetBytes(value));
        }

        public static byte[] UrlDecodeToBytes(byte[] value, int offset, int count)
        {
            if (value == null)
                return null;
            if (count == 0)
                return new byte[0];

            int len = value.Length;
            if (offset < 0 || offset >= len)
                throw new ArgumentOutOfRangeException("offset");

            if (count < 0 || offset > len - count)
                throw new ArgumentOutOfRangeException("count");

            using (MemoryStream result = new MemoryStream())
            {
                int end = offset + count;
                for (int i = offset; i < end; i++)
                {
                    char c = (char)value[i];
                    if (c == '+')
                    {
                        c = ' ';
                    }
                    else if (c == '%' && i < end - 2)
                    {
                        int xchar = GetChar(value, i + 1, 2);
                        if (xchar != -1)
                        {
                            c = (char)xchar;
                            i += 2;
                        }
                    }
                    result.WriteByte((byte)c);
                }
                return result.ToArray();
            }
        }

        public static string UrlEncode(string value)
        {
            return UrlEncode(value, Encoding.UTF8);
        }

        public static string UrlEncode(string value, Encoding encoding)
        {
            if (value == null)
                return null;

            if (string.IsNullOrEmpty(value))
                return "";

            bool needEncode = false;
            int len = value.Length;
            for (int i = 0; i < len; i++)
            {
                char c = value[i];
                if ((c < '0') || (c < 'A' && c > '9') || (c > 'Z' && c < 'a') || (c > 'z'))
                {
                    if (NotEncoded(c))
                        continue;

                    needEncode = true;
                    break;
                }
            }

            if (!needEncode)
                return value;

            // avoided GetByteCount call
            byte[] bytes = new byte[encoding.GetMaxByteCount(value.Length)];
            int realLen = encoding.GetBytes(value, 0, value.Length, bytes, 0);
            return Encoding.ASCII.GetString(UrlEncodeToBytes(bytes, 0, realLen));
        }

        public static string UrlEncode(byte[] value)
        {
            if (value == null)
                return null;

            if (value.Length == 0)
                return "";

            return Encoding.ASCII.GetString(UrlEncodeToBytes(value, 0, value.Length));
        }

        public static string UrlEncode(byte[] value, int offset, int count)
        {
            if (value == null)
                return null;

            if (value.Length == 0)
                return "";

            return Encoding.ASCII.GetString(UrlEncodeToBytes(value, offset, count));
        }

        public static byte[] UrlEncodeToBytes(string value)
        {
            return UrlEncodeToBytes(value, Encoding.UTF8);
        }

        public static byte[] UrlEncodeToBytes(string value, Encoding encoding)
        {
            if (value == null)
                return null;

            if (string.IsNullOrEmpty(value))
                return new byte[0];

            byte[] bytes = encoding.GetBytes(value);
            return UrlEncodeToBytes(bytes, 0, bytes.Length);
        }

        public static byte[] UrlEncodeToBytes(byte[] value)
        {
            if (value == null)
                return null;

            if (value.Length == 0)
                return new byte[0];

            return UrlEncodeToBytes(value, 0, value.Length);
        }

        static char[] hexChars = "0123456789abcdef".ToCharArray();

        static bool NotEncoded(char c)
        {
            return (c == '!' || c == '\'' || c == '(' || c == ')' || c == '*' || c == '-' || c == '.' || c == '_');
        }

        static void UrlEncodeChar(char c, Stream result, bool isUnicode)
        {
            if (c > 255)
            {
                //FIXME: what happens when there is an internal error?
                //if (!isUnicode)
                //	throw new ArgumentOutOfRangeException ("c", c, "c must be less than 256");
                int idx;
                int i = (int)c;

                result.WriteByte((byte)'%');
                result.WriteByte((byte)'u');
                idx = i >> 12;
                result.WriteByte((byte)hexChars[idx]);
                idx = (i >> 8) & 0x0F;
                result.WriteByte((byte)hexChars[idx]);
                idx = (i >> 4) & 0x0F;
                result.WriteByte((byte)hexChars[idx]);
                idx = i & 0x0F;
                result.WriteByte((byte)hexChars[idx]);
                return;
            }

            if (c > ' ' && NotEncoded(c))
            {
                result.WriteByte((byte)c);
                return;
            }
            if (c == ' ')
            {
                result.WriteByte((byte)'+');
                return;
            }
            if ((c < '0') ||
                (c < 'A' && c > '9') ||
                (c > 'Z' && c < 'a') ||
                (c > 'z'))
            {
                if (isUnicode && c > 127)
                {
                    result.WriteByte((byte)'%');
                    result.WriteByte((byte)'u');
                    result.WriteByte((byte)'0');
                    result.WriteByte((byte)'0');
                }
                else
                    result.WriteByte((byte)'%');

                int idx = ((int)c) >> 4;
                result.WriteByte((byte)hexChars[idx]);
                idx = ((int)c) & 0x0F;
                result.WriteByte((byte)hexChars[idx]);
            }
            else
                result.WriteByte((byte)c);
        }

        public static byte[] UrlEncodeToBytes(byte[] value, int offset, int count)
        {
            if (value == null)
                return null;

            int len = value.Length;
            if (len == 0)
                return new byte[0];

            if (offset < 0 || offset >= len)
                throw new ArgumentOutOfRangeException("offset");

            if (count < 0 || count > len - offset)
                throw new ArgumentOutOfRangeException("count");

            using (MemoryStream result = new MemoryStream(count))
            {
                int end = offset + count;
                for (int i = offset; i < end; i++)
                    UrlEncodeChar((char)value[i], result, false);

                return result.ToArray();
            }
        }

        public static string UrlEncodeUnicode(string value)
        {
            if (value == null)
                return null;

            return Encoding.ASCII.GetString(UrlEncodeUnicodeToBytes(value));
        }

        public static byte[] UrlEncodeUnicodeToBytes(string value)
        {
            if (value == null)
                return null;

            if (String.IsNullOrEmpty(value))
                return new byte[0];

            using (MemoryStream result = new MemoryStream(value.Length))
            {
                foreach (char c in value)
                {
                    UrlEncodeChar(c, result, true);
                }
                return result.ToArray();
            }
        }

        /// <summary>
        /// Decodes an HTML-encoded string and returns the decoded string.
        /// </summary>
        /// <param name="value">The HTML string to decode. </param>
        /// <returns>The decoded text.</returns>
        public static string HtmlDecode(string value)
        {
            if (value == null)
                throw new ArgumentNullException("value");

            if (value.IndexOf('&') == -1)
                return value;

            StringBuilder entity = new StringBuilder();
            StringBuilder output = new StringBuilder();
            int len = value.Length;
            // 0 -> nothing,
            // 1 -> right after '&'
            // 2 -> between '&' and ';' but no '#'
            // 3 -> '#' found after '&' and getting numbers
            int state = 0;
            int number = 0;
            bool have_trailing_digits = false;

            for (int i = 0; i < len; i++)
            {
                char c = value[i];
                if (state == 0)
                {
                    if (c == '&')
                    {
                        entity.Append(c);
                        state = 1;
                    }
                    else
                    {
                        output.Append(c);
                    }
                    continue;
                }

                if (c == '&')
                {
                    state = 1;
                    if (have_trailing_digits)
                    {
                        entity.Append(number.ToString(CultureInfo.InvariantCulture));
                        have_trailing_digits = false;
                    }

                    output.Append(entity.ToString());
                    entity.Length = 0;
                    entity.Append('&');
                    continue;
                }

                if (state == 1)
                {
                    if (c == ';')
                    {
                        state = 0;
                        output.Append(entity.ToString());
                        output.Append(c);
                        entity.Length = 0;
                    }
                    else
                    {
                        number = 0;
                        if (c != '#')
                        {
                            state = 2;
                        }
                        else
                        {
                            state = 3;
                        }
                        entity.Append(c);
                    }
                }
                else if (state == 2)
                {
                    entity.Append(c);
                    if (c == ';')
                    {
                        string key = entity.ToString();
                        if (key.Length > 1 && Entities.ContainsKey(key.Substring(1, key.Length - 2)))
                            key = Entities[key.Substring(1, key.Length - 2)].ToString();

                        output.Append(key);
                        state = 0;
                        entity.Length = 0;
                    }
                }
                else if (state == 3)
                {
                    if (c == ';')
                    {
                        if (number > 65535)
                        {
                            output.Append("&#");
                            output.Append(number.ToString(System.Globalization.CultureInfo.InvariantCulture));
                            output.Append(";");
                        }
                        else
                        {
                            output.Append((char)number);
                        }
                        state = 0;
                        entity.Length = 0;
                        have_trailing_digits = false;
                    }
                    else if (Char.IsDigit(c))
                    {
                        number = number * 10 + ((int)c - '0');
                        have_trailing_digits = true;
                    }
                    else
                    {
                        state = 2;
                        if (have_trailing_digits)
                        {
                            entity.Append(number.ToString(CultureInfo.InvariantCulture));
                            have_trailing_digits = false;
                        }
                        entity.Append(c);
                    }
                }
            }

            if (entity.Length > 0)
            {
                output.Append(entity.ToString());
            }
            else if (have_trailing_digits)
            {
                output.Append(number.ToString(CultureInfo.InvariantCulture));
            }
            return output.ToString();
        }

        /// <summary>
        /// Decodes an HTML-encoded string and sends the resulting output to a TextWriter output stream.
        /// </summary>
        /// <param name="value">The HTML string to decode</param>
        /// <param name="output">The TextWriter output stream containing the decoded string. </param>
        public static void HtmlDecode(string value, TextWriter output)
        {
            if (value != null && output != null)
            {
                output.Write(HtmlDecode(value));
            }
        }

        /// <summary>
        /// HTML-encodes a string and returns the encoded string.
        /// </summary>
        /// <param name="value">The text string to encode. </param>
        /// <returns>The HTML-encoded text.</returns>
        public static string HtmlEncode(string value)
        {
            if (value == null)
                return null;

            bool needEncode = false;
            for (int i = 0; i < value.Length; i++)
            {
                char c = value[i];
                if (c == '&' || c == '"' || c == '<' || c == '>' || c > 159)
                {
                    needEncode = true;
                    break;
                }
            }

            if (!needEncode)
                return value;

            StringBuilder output = new StringBuilder();

            int len = value.Length;
            for (int i = 0; i < len; i++)
                switch (value[i])
                {
                    case '&':
                        output.Append("&amp;");
                        break;
                    case '>':
                        output.Append("&gt;");
                        break;
                    case '<':
                        output.Append("&lt;");
                        break;
                    case '"':
                        output.Append("&quot;");
                        break;
                    default:
                        // MS starts encoding with &# from 160 and stops at 255.
                        // We don't do that. One reason is the 65308/65310 unicode
                        // characters that look like '<' and '>'.
#if TARGET_JVM
					if (s [i] > 159 && s [i] < 256) {
#else
                        if (value[i] > 159)
                        {
#endif
                            output.Append("&#");
                            output.Append(((int)value[i]).ToString(CultureInfo.InvariantCulture));
                            output.Append(";");
                        }
                        else
                        {
                            output.Append(value[i]);
                        }
                        break;
                }
            return output.ToString();
        }

        /// <summary>
        /// HTML-encodes a string and sends the resulting output to a TextWriter output stream.
        /// </summary>
        /// <param name="value">The string to encode. </param>
        /// <param name="output">The TextWriter output stream containing the encoded string. </param>
        public static void HtmlEncode(string value, TextWriter output)
        {
            if (value != null && output != null)
            {
                output.Write(HtmlEncode(value));
            }
        }

        public static string UrlPathEncode(string value)
        {
            if (value == null || value.Length == 0)
                return value;

            using (MemoryStream result = new MemoryStream())
            {
                int length = value.Length;
                for (int i = 0; i < length; i++)
                {
                    UrlPathEncodeChar(value[i], result);
                }
                return Encoding.ASCII.GetString(result.ToArray());
            }
        }

        static void UrlPathEncodeChar(char value, Stream result)
        {
            if (value < 33 || value > 126)
            {
                byte[] bIn = Encoding.UTF8.GetBytes(value.ToString());
                for (int i = 0; i < bIn.Length; i++)
                {
                    result.WriteByte((byte)'%');
                    int idx = ((int)bIn[i]) >> 4;
                    result.WriteByte((byte)hexChars[idx]);
                    idx = ((int)bIn[i]) & 0x0F;
                    result.WriteByte((byte)hexChars[idx]);
                }
            }
            else if (value == ' ')
            {
                result.WriteByte((byte)'%');
                result.WriteByte((byte)'2');
                result.WriteByte((byte)'0');
            }
            else
                result.WriteByte((byte)value);
        }

        public static NameValueCollection ParseQueryString(string query)
        {
            return ParseQueryString(query, Encoding.UTF8);
        }

        public static NameValueCollection ParseQueryString(string query, Encoding encoding)
        {
            if (query == null)
                throw new ArgumentNullException("query");
            if (encoding == null)
                throw new ArgumentNullException("encoding");
            if (query.Length == 0 || (query.Length == 1 && query[0] == '?'))
                return new NameValueCollection();
            if (query[0] == '?')
                query = query.Substring(1);

            NameValueCollection result = new HttpQSCollection();
            ParseQueryString(query, encoding, result);
            return result;
        }

        internal static void ParseQueryString(string query, Encoding encoding, NameValueCollection result)
        {
            if (query.Length == 0)
                return;

            string decoded = HtmlDecode(query);
            int decodedLength = decoded.Length;
            int namePos = 0;
            bool first = true;
            while (namePos <= decodedLength)
            {
                int valuePos = -1, valueEnd = -1;
                for (int q = namePos; q < decodedLength; q++)
                {
                    if (valuePos == -1 && decoded[q] == '=')
                    {
                        valuePos = q + 1;
                    }
                    else if (decoded[q] == '&')
                    {
                        valueEnd = q;
                        break;
                    }
                }

                if (first)
                {
                    first = false;
                    if (decoded[namePos] == '?')
                        namePos++;
                }

                string name, value;
                if (valuePos == -1)
                {
                    name = null;
                    valuePos = namePos;
                }
                else
                {
                    name = UrlDecode(decoded.Substring(namePos, valuePos - namePos - 1), encoding);
                }
                if (valueEnd < 0)
                {
                    namePos = -1;
                    valueEnd = decoded.Length;
                }
                else
                {
                    namePos = valueEnd + 1;
                }
                value = UrlDecode(decoded.Substring(valuePos, valueEnd - valuePos), encoding);

                result.Add(name, value);
                if (namePos == -1)
                    break;
            }
        }
        #endregion // Methods
    }
}

