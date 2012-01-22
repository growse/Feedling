using System;
using System.Net;
using Feedling.Properties;
using NLog;

namespace Feedling.Classes
{
    public class HttpProxyHelper
    {
        private static readonly Logger Log = LogManager.GetCurrentClassLogger();
        internal static IWebProxy GetGlobalProxy()
        {
            Log.Debug("Fetching the global proxy");
            IWebProxy proxy = null;
            ProxyType proxytype;
            if (Enum.IsDefined(typeof(ProxyType), Settings.Default.ProxyType))
            {
                proxytype = (ProxyType)Enum.Parse(typeof(ProxyType), Settings.Default.ProxyType);
            }
            else
            {
                return null;
            }
            switch (proxytype)
            {
                case ProxyType.Custom:
                    proxy = new WebProxy(Settings.Default.ProxyHost, Settings.Default.ProxyPort);
                    if (Settings.Default.ProxyAuth)
                    {
                        string user, domain = null;
                        if (Settings.Default.ProxyUser.Contains("\\"))
                        {
                            string[] bits = Settings.Default.ProxyUser.Split("\\".ToCharArray(), 2);
                            user = bits[1];
                            domain = bits[0];
                        }
                        else
                        {
                            user = Settings.Default.ProxyUser;
                        }
                        proxy.Credentials = new NetworkCredential(user, Settings.Default.ProxyPass, domain);
                    }
                    break;
                case ProxyType.System:
                    proxy = WebRequest.GetSystemWebProxy();
                    break;
                case ProxyType.None:
                    break;
                case ProxyType.Global:
                    break;
            }
            return proxy;
        }

        public enum ProxyType
        {
            Global,
            None,
            System,
            Custom
        }
    }
}
