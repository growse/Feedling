using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Cache;
using System.Reflection;
using System.Windows.Forms;

namespace Feedling
{
    class ApplicationUpdates
    {
        #region Application Updates
        private static log4net.ILog Log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        internal static void CheckForUpdates(bool failsilently = false)
        {
            try
            {
                Log.Debug("Entering update block");
                Uri applicationupdateuri = new Uri(Properties.Settings.Default.ApplicationUpdateUrl);
                WebRequest request = WebRequest.Create(applicationupdateuri);
                Log.Debug("Downloading version definition");
                WebResponse response = request.GetResponse();
                if (response.ContentLength > 0)
                {
                    StreamReader sr = new StreamReader(response.GetResponseStream());
                    string upgrademeta = sr.ReadToEnd();
                    if (upgrademeta.Contains("|"))
                    {
                        Log.Debug("Received valid version definition");
                        string[] parts = upgrademeta.Split("|".ToCharArray(), 2);
                        Version availableversion = new Version(parts[0]);
                        if (Assembly.GetExecutingAssembly().GetName().Version.CompareTo(availableversion) < 0)
                        {
                            Log.DebugFormat("New version available: {0}",availableversion);
                            DialogResult dr = MessageBox.Show("Updates are available for Desktop Colleague Finder. Do you wish to install them?", "Updates available", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                            if (dr == DialogResult.Yes)
                            {
                                Log.Debug("User requests upgrade to new version");
                                string msipath = Properties.Settings.Default.ApplicationUpdateUrl.Replace(applicationupdateuri.Segments[applicationupdateuri.Segments.Length - 1].ToString(), parts[1]);
                                string savefile = string.Concat(Path.GetTempPath(), Path.DirectorySeparatorChar, "DesktopCFSetup.msi");

                                HttpRequestCachePolicy nocachepolicy = new HttpRequestCachePolicy(HttpRequestCacheLevel.NoCacheNoStore);
                                WebRequest msirequest = WebRequest.Create(msipath);
                                msirequest.CachePolicy = nocachepolicy;
                                Log.DebugFormat("Downloading new MSI: {0}",msipath);
                                WebResponse msiresponse = msirequest.GetResponse();
                                using (Stream responsestream = msiresponse.GetResponseStream())
                                {
                                    Log.DebugFormat("Saving MSI in {0}",savefile);
                                    using (FileStream fs = new FileStream(savefile, FileMode.Create))
                                    {
                                        using (BinaryReader br = new BinaryReader(responsestream))
                                        {
                                            byte[] buff = new byte[1024];
                                            int bytesread = 0;
                                            using (BinaryWriter bw = new BinaryWriter(fs))
                                            {
                                                do
                                                {
                                                    bytesread = br.Read(buff, 0, buff.Length);
                                                    bw.Write(buff, 0, bytesread);
                                                } while (bytesread > 0);
                                            }
                                        }
                                    }
                                }
                                Log.Debug("MSI saved.");
                                ProcessStartInfo psi = new ProcessStartInfo();
                                psi.Arguments = "";
                                psi.FileName = savefile;
                                Log.Debug("Starting installer");
                                Process.Start(psi);
                                Log.Debug("Exiting.");
                                Application.Exit();
                            }
                        }
                        else
                        {
                            if (!failsilently)
                            {
                                MessageBox.Show("No updates available", "No updates available", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            }
                        }
                    }

                }
            }
            catch
            {
                if (!failsilently)
                {
                    MessageBox.Show("There was an error checking for updates. Please try again later.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        #endregion
    }
}
