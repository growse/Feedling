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
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Cache;
using System.Reflection;
using System.Windows.Forms;
using NLog;

namespace Feedling
{
    class ApplicationUpdates
    {
        private ApplicationUpdates()
        {

        }

        #region Application Updates
        private static Logger Log = LogManager.GetCurrentClassLogger();
        internal static void CheckForUpdates(bool failsilently = false)
        {
            try
            {
                Log.Info("Entering update block");
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
                        Log.Info("Received valid version definition");
                        string[] parts = upgrademeta.Split("|".ToCharArray(), 2);
                        Version availableversion = new Version(parts[0]);
                        if (Assembly.GetExecutingAssembly().GetName().Version.CompareTo(availableversion) < 0)
                        {
                            Log.Debug("New version available: {0}", availableversion);
                            DialogResult dr = MessageBox.Show(Properties.Resources.UpdatesAvailableText, Properties.Resources.UpdatesAvailableCaption, MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                            if (dr == DialogResult.Yes)
                            {
                                Log.Debug("User requests upgrade to new version");
                                string msipath = Properties.Settings.Default.ApplicationUpdateUrl.Replace(applicationupdateuri.Segments[applicationupdateuri.Segments.Length - 1].ToString(), parts[1]);
                                string savefile = string.Concat(Path.GetTempPath(), Path.DirectorySeparatorChar, "Feedling.msi");

                                HttpRequestCachePolicy nocachepolicy = new HttpRequestCachePolicy(HttpRequestCacheLevel.NoCacheNoStore);
                                WebRequest msirequest = WebRequest.Create(msipath);
                                msirequest.CachePolicy = nocachepolicy;
                                Log.Debug("Downloading new MSI: {0}", msipath);
                                WebResponse msiresponse = msirequest.GetResponse();
                                using (Stream responsestream = msiresponse.GetResponseStream())
                                {
                                    Log.Info("Saving MSI in {0}", savefile);
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
                                Log.Info("Starting installer");
                                Process.Start(psi);
                                Log.Info("Exiting.");
                                Application.Exit();
                            }
                        }
                        else
                        {
                            if (!failsilently)
                            {
                                MessageBox.Show(Properties.Resources.NoUpdatesAvailableText, Properties.Resources.NoUpdatesAvailableCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
                            }
                        }
                    }

                }
                else
                {
                    Log.Error("There was an error fetching the update definition version. Content Length appeared to be 0");
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                if (!failsilently)
                {
                    MessageBox.Show(Properties.Resources.UpdatesErrorText, Properties.Resources.UpdatesErrorCaption, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        #endregion
    }
}
