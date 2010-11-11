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
        internal static void CheckForUpdates()
        {
            try
            {
                Uri applicationupdateuri = new Uri(Properties.Settings.Default.ApplicationUpdateUrl);
                WebRequest request = WebRequest.Create(applicationupdateuri);
                WebResponse response = request.GetResponse();
                if (response.ContentLength > 0)
                {
                    StreamReader sr = new StreamReader(response.GetResponseStream());
                    string upgrademeta = sr.ReadToEnd();
                    if (upgrademeta.Contains("|"))
                    {
                        string[] parts = upgrademeta.Split("|".ToCharArray(), 2);
                        Version availableversion = new Version(parts[0]);
                        if (Assembly.GetExecutingAssembly().GetName().Version.CompareTo(availableversion) < 0)
                        {
                            DialogResult dr = MessageBox.Show("Updates are available for Desktop Colleague Finder. Do you wish to install them?", "Updates available", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                            if (dr == DialogResult.Yes)
                            {
                                string msipath = Properties.Settings.Default.ApplicationUpdateUrl.Replace(applicationupdateuri.Segments[applicationupdateuri.Segments.Length - 1].ToString(), parts[1]);
                                string savefile = string.Concat(Path.GetTempPath(), Path.DirectorySeparatorChar, "DesktopCFSetup.msi");

                                HttpRequestCachePolicy nocachepolicy = new HttpRequestCachePolicy(HttpRequestCacheLevel.NoCacheNoStore);
                                WebRequest msirequest = WebRequest.Create(msipath);
                                msirequest.CachePolicy = nocachepolicy;
                                WebResponse msiresponse = msirequest.GetResponse();
                                using (Stream responsestream = msiresponse.GetResponseStream())
                                {
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

                                ProcessStartInfo psi = new ProcessStartInfo();
                                psi.Arguments = "";
                                psi.FileName = savefile;
                                Process.Start(psi);
                                Application.Exit();
                            }
                        }
                        else
                        {
                            MessageBox.Show("No updates available", "No updates available", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                    }

                }
            }
            catch
            {
                MessageBox.Show("There was an error checking for updates. Please try again later.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        #endregion
    }
}
