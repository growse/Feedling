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
using System.Windows;
using NLog;

namespace Feedling
{
    /// <summary>
    /// Interaction logic for AutoUpdate.xaml
    /// </summary>
    public partial class AutoUpdate : Window
    {
        public AutoUpdate()
        {
            InitializeComponent();
        }
        private Logger Log = LogManager.GetCurrentClassLogger();
        private Uri applicationupdateuri = new Uri(Properties.Settings.Default.ApplicationUpdateUrl);
        private string remoteMsiPath;
        internal void CheckForUpdates(bool silent = false)
        {
            if (!silent)
            {
                this.Visibility = Visibility.Visible;
            }
            try
            {
                Log.Info("Entering update block");

                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(applicationupdateuri);
                request.UserAgent = string.Format("Feedling v{0}", Assembly.GetExecutingAssembly().GetName().Version.ToString(3));
                    

                Log.Debug("Downloading version definition");
                WebResponse response = request.GetResponse();
                if (response.ContentLength > 0)
                {
                    StreamReader sr = new StreamReader(response.GetResponseStream());
                    string upgrademeta = sr.ReadToEnd();
                    if (upgrademeta.Contains("|"))
                    {
                        Log.Info("Received valid version definition");
                        string[] parts = upgrademeta.Split("|".ToCharArray(), 3);
                        remoteMsiPath = Properties.Settings.Default.ApplicationUpdateUrl.Replace(applicationupdateuri.Segments[applicationupdateuri.Segments.Length - 1].ToString(), parts[1]).Trim();
                        Version availableversion = new Version(parts[0]);
                        if (Assembly.GetExecutingAssembly().GetName().Version.CompareTo(availableversion) < 0)
                        {
                            Log.Debug("New version available: {0}", availableversion);
                            this.Visibility = Visibility.Visible;
                            ApplyBtn.IsEnabled = true;
                            UpdateDescription.Text = string.Format(Properties.Resources.UpdatesAvailableText, availableversion, parts[2]);
                        }
                        else
                        {
                            UpdateDescription.Text = Properties.Resources.NoUpdatesAvailableText;
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
                if (!silent)
                {
                    UpdateDescription.Text = string.Format(Properties.Resources.UpdatesErrorCheckText, ex.Message);
                }
            }
            if (silent && this.Visibility == Visibility.Collapsed)
            {
                this.Close();
            }
        }
        private string localMsiFilePath;
        private void ApplyBtn_Click(object sender, RoutedEventArgs e)
        {
            CloseBtn.IsEnabled = false;
            ApplyBtn.IsEnabled = false;
            try
            {
                Log.Debug("User requests upgrade to new version");
                localMsiFilePath = string.Concat(Path.GetTempPath(), Path.DirectorySeparatorChar, "Feedling.msi");

                HttpRequestCachePolicy nocachepolicy = new HttpRequestCachePolicy(HttpRequestCacheLevel.NoCacheNoStore);

                WebClient wc = new WebClient();

                wc.CachePolicy = nocachepolicy;
                wc.DownloadProgressChanged += new DownloadProgressChangedEventHandler(wc_DownloadProgressChanged);
                wc.DownloadFileCompleted += new System.ComponentModel.AsyncCompletedEventHandler(wc_DownloadFileCompleted);
                wc.DownloadFileAsync(new Uri(remoteMsiPath), localMsiFilePath);

            }
            catch (Exception ex)
            {
                Log.Error("Error Applying update", ex);
                UpdateDescription.Text = string.Format(Properties.Resources.UpdatesErrorApplyText, ex.Message);
                CloseBtn.IsEnabled = true;
            }
        }

        void wc_DownloadFileCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {
            if (e.Error == null)
            {
                Log.Debug("MSI saved.");
                ProcessStartInfo psi = new ProcessStartInfo();
                psi.Arguments = "";
                psi.FileName = localMsiFilePath;
                Log.Info("Starting installer");
                Process.Start(psi);
                Log.Info("Exiting.");
                Application.Current.Shutdown();
            }
            else
            {
                Log.Error("Error Applying update", e.Error);
                UpdateDescription.Text = string.Format(Properties.Resources.UpdatesErrorApplyText, e.Error.Message);
            }
            CloseBtn.IsEnabled = true;
        }

        void wc_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            progressBar.Value = e.ProgressPercentage;
        }

        private void CloseBtn_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
